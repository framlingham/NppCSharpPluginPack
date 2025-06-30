using Kbg.NppPluginNET.PluginInfrastructure;
using Kbg.NppPluginNET;
using NppDemo.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace NppDemo.Views
{
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindowBase : Window
	{
        private static readonly string IniFilePath;

		public SettingsWindowBase()
		{
			InitializeComponent();

			setPropertyValuesToDefault();

			ReadFromIniFile();
		}

		static SettingsWindowBase()
		{
			IniFilePath = Path.Combine(Main.PluginConfigDirectory, Main.PluginName + ".ini");
		}

		public SettingsWindow Copy { get => (SettingsWindow)GetValue(CopyProperty); set => SetValue(CopyProperty, value); }
		public static readonly DependencyProperty CopyProperty = DependencyProperty.Register(nameof(Copy), typeof(SettingsWindow), typeof(SettingsWindowBase), new PropertyMetadata(null));

		public PropertyItem[] Properties { get => (PropertyItem[])GetValue(PropertiesProperty); set => SetValue(PropertiesProperty, value); }
		public static readonly DependencyProperty PropertiesProperty = DependencyProperty.Register(nameof(Properties), typeof(PropertyItem[]), typeof(SettingsWindowBase), new PropertyMetadata(null));

		private void buttonCancel_Click(object sender, RoutedEventArgs e)
		{
			Close(); // without saving changes to the original object or to ini file.
		}

		private void buttonReset_Click(object sender, RoutedEventArgs e)
		{
			setPropertyValuesToDefault();

			SaveToIniFile();
			
			Close();
		}

		private void setPropertyValuesToDefault()
		{
			var type = GetType();
			foreach (var propertyInfo in type.GetProperties().Where(p => p.CanRead && p.CanWrite && p.DeclaringType == type))
			{
				var def = (DefaultValueAttribute)propertyInfo.GetCustomAttributes(typeof(DefaultValueAttribute), false).FirstOrDefault();
				if (def != null)
				{
					propertyInfo.SetValue(this, def.Value, null);
				}
			}
		}

		private void buttonOkay_Click(object sender, RoutedEventArgs e)
		{
			// Copy the changed values to the settings object.
			foreach (var propertyInfo in Properties.Where(pi => pi.anyChange))
			{
				propertyInfo.source.SetValue(this, propertyInfo.Value);
			}

			if (Properties.Any(pi => pi.anyChange))
			{
				SaveToIniFile();
			}

			Close();
		}

		/// <summary>
		/// Reads all (existing) settings from an ini-file
		/// </summary>
		/// <param name="filename">File to write to (default is N++ plugin config)</param>
		public void ReadFromIniFile(string filename = null)
		{
			filename = filename ?? IniFilePath;
			if (!File.Exists(filename))
				return;

			// Load all sections from file
			var loaded = GetType().GetProperties()
				.Select(x => ((CategoryAttribute)x.GetCustomAttributes(typeof(CategoryAttribute), false).FirstOrDefault())?.Category ?? "General")
				.Distinct()
				.ToDictionary(section => section, section => GetKeys(filename, section));

			//var loaded = GetKeys(filename, "General");
			foreach (var propertyInfo in GetType().GetProperties())
			{
				var category = ((CategoryAttribute)propertyInfo.GetCustomAttributes(typeof(CategoryAttribute), false).FirstOrDefault())?.Category ?? "General";
				var name = propertyInfo.Name;
				if (loaded.ContainsKey(category) && loaded[category].ContainsKey(name) && !string.IsNullOrEmpty(loaded[category][name]))
				{
					var rawString = loaded[category][name];
					var converter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);
					if (converter.IsValid(rawString))
					{
						propertyInfo.SetValue(this, converter.ConvertFromString(rawString), null);
					}
				}
			}
		}

		/// <summary>
		/// Saves all settings to an ini-file, under "General" section
		/// </summary>
		/// <param name="filename">File to write to (default is N++ plugin config)</param>
		public void SaveToIniFile(string filename = null)
		{
			filename = filename ?? IniFilePath;
			Npp.CreateConfigSubDirectoryIfNotExists();

			// Win32.WritePrivateProfileSection (that NppPlugin uses) doesn't (or didn't?) work well with non-ASCII characters. So we roll our own.
			using (var fp = new StreamWriter(filename, false, Encoding.UTF8))
			{
				fp.WriteLine("; {0} settings file", Main.PluginName);

				foreach (var section in GetType()
							.GetProperties()
							.GroupBy(x => ((CategoryAttribute)x.GetCustomAttributes(typeof(CategoryAttribute), false)
										.FirstOrDefault())?.Category ?? "General"))
				{
					fp.WriteLine(Environment.NewLine + "[{0}]", section.Key);
					foreach (var propertyInfo in section.OrderBy(x => x.Name))
					{
						if (propertyInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() is DescriptionAttribute description)
						{
							fp.WriteLine("; " + description.Description.Replace(Environment.NewLine, Environment.NewLine + "; "));
						}
						var converter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);
						fp.WriteLine("{0}={1}", propertyInfo.Name, converter.ConvertToInvariantString(propertyInfo.GetValue(this, null)));
					}
				}
			}
		}

		/// <summary>
		/// Read a section from an ini-file
		/// </summary>
		/// <param name="iniFile">Path to ini-file</param>
		/// <param name="category">Section to read</param>
		private Dictionary<string, string> GetKeys(string iniFile, string category)
		{
			var buffer = new byte[8 * 1024];

			Win32.GetPrivateProfileSection(category, buffer, buffer.Length, iniFile);
			var tmp = Encoding.UTF8.GetString(buffer).Trim('\0').Split('\0');
			return tmp.Select(x => x.Split(new[] { '=' }, 2))
				.Where(x => x.Length == 2)
				.ToDictionary(x => x[0], x => x[1]);
		}

		/// <summary> Opens the config file directly in Notepad++ </summary>
		public void OpenFile()
		{
			if (!File.Exists(IniFilePath))
			{
				SaveToIniFile();
			}
			Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DOOPEN, 0, IniFilePath);
		}
	}

	public class PropertyItem : NotifyPropertyChanged
	{
		public PropertyItem(PropertyInfo pi, object item)
		{
			source = pi;
			Name = pi.Name;
			_value = pi.GetValue(item); // Use private backer to avoid triggering change detection.
			PropertyType = pi.PropertyType;
			Category = pi.GetCustomAttributes<CategoryAttribute>(false).FirstOrDefault()?.Category ?? "General";
			Description = pi.GetCustomAttributes<DescriptionAttribute>(false).FirstOrDefault()?.Description ?? "No description provided";
		}

		internal bool anyChange = false;

		internal PropertyInfo source;
		public string Name { get => _name; set => setValue(ref _name, value); }
		private string _name;
		public object Value
		{
			get => _value;
			set
			{
				if (setValue(ref _value, value))
				{
					anyChange = true;
				}
			}
		}
		private object _value;
		public Type PropertyType { get => _propertyType; set => setValue(ref _propertyType, value); }
		private Type _propertyType;

		public string Category { get => _category; set => setValue(ref _category, value); }
		private string _category;

		public string Description { get => _description; set => setValue(ref _description, value); }
		private string _description;
	}

}
