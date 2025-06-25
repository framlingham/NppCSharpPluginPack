using Kbg.NppPluginNET;
using NppDemo.Forms;
using NppDemo.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace NppDemo.Views
{
	/// <summary>
	/// Interaction logic for SelectionRememberingControl.xaml
	/// </summary>
	public partial class SelectionRememberingControl : UserControl
	{
		private DarkModeTestWindow _darkModeTestWindow;

		public SelectionRememberingControl()
		{
			InitializeComponent();
			//FormStyle.UpdateStyle();
			FormStyle.ApplyStyle(this, Main.settings.use_npp_styling);

			IsVisibleChanged += selectionRememberingControl_IsVisibleChanged;

			LogMessage = (message) =>
			{
				int maxLength = 2000;
				message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}";
				if (message.Length > 200)
				{
					message = message.Substring(0, maxLength) + "...";
				}

				if (Logs == null)
				{
					Logs = message;
				}
				else
				{
					var all = $"{Logs}\n{message}";
					if (all.Length > maxLength)
					{
						Logs = all.Substring(all.Length - maxLength, maxLength);
					}
					else
					{
						Logs += Environment.NewLine + message;
					}
				}
			};
		}

		public static Action<string> LogMessage;


		public string Logs { get => (string)GetValue(LogsProperty); set => SetValue(LogsProperty, value); }
		public static readonly DependencyProperty LogsProperty = DependencyProperty.Register(nameof(Logs), typeof(string), typeof(SelectionRememberingControl), new PropertyMetadata(""));

		public string SelectionText { get => (string)GetValue(SelectionTextProperty); set => SetValue(SelectionTextProperty, value); }
		public static readonly DependencyProperty SelectionTextProperty = DependencyProperty.Register(nameof(SelectionText), typeof(string), typeof(SelectionRememberingControl), new PropertyMetadata(""));

		private void selectionRememberingControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue == false)
			{
				_darkModeTestWindow?.Close();
			}
		}

		private void copySelectionsToStartEndsButton_Click(object sender, RoutedEventArgs e)
		{
			string startEnds = SelectionManager.StartEndListToString(SelectionManager.GetSelectedRanges());
			System.Windows.Clipboard.SetText(startEnds);
		}

		private void setSelectionsFromStartEndsButton_Click(object sender, RoutedEventArgs e)
		{
			var startEndMatches = Regex.Matches(SelectionText, @"\d+,\d+");
			if (startEndMatches.Count == 0)
			{
				MessageBox.Show("Expected a space-separated list of two comma-separated numbers, like \"1,2  3,4   5,6\"\n" +
								$"but no comma-separated number pairs were found ({SelectionText}).",
								"Couldn't find comma-separated numbers",
								MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			var startEndList = new List<string>(startEndMatches.Count);
			foreach (Match m in startEndMatches)
			{
				startEndList.Add(m.Value);
			}
			SelectionManager.SetSelectionsFromStartEnds(startEndList);
		}

		private void saveCurrentSelectionsToFileButton_Click(object sender, RoutedEventArgs e)
		{
			Npp.CreateConfigSubDirectoryIfNotExists();
			var savedSelectionsFilePath = Path.Combine(Main.PluginConfigDirectory, "SavedSelections.txt");

			string startEnds = SelectionManager.StartEndListToString(SelectionManager.GetSelectedRanges(), "\r\n");
			using (var writer = new StreamWriter(savedSelectionsFilePath, false, Encoding.UTF8))
			{
				writer.Write(startEnds);
				writer.Flush();
			}
		}

		private void loadSelectionsFromFileButton_Click(object sender, RoutedEventArgs e)
		{
			if (!Directory.Exists(Main.PluginConfigDirectory))
			{
				MessageBox.Show("No selections were previously saved to file.",
								"No saved selections",
								MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}
			var savedSelectionsFileInfo = new FileInfo(Path.Combine(Main.PluginConfigDirectory, "SavedSelections.txt"));
			string savedSelections;
			using (StreamReader reader = savedSelectionsFileInfo.OpenText())
			{
				savedSelections = reader.ReadToEnd();
			}
			SelectionText = savedSelections;
		}

		private void openDarkModeTestFormButton_Click(object sender, RoutedEventArgs e)
		{
			_darkModeTestWindow = new DarkModeTestWindow() { Owner = Window.GetWindow(this) };
			_darkModeTestWindow.Show();
			_darkModeTestWindow.GrabFocus();
		}

		private void keyDownHandler(object sender, KeyEventArgs e)
		{
			// This fires whether or not I have called NppFormHelper.RegisterControlIfModeless(selectionHost, false);
			// But the text box does not add the pressed key as text when it is called.
			NppFormHelper.GenericKeyDownHandler(sender, e);
		}

		private void keyUpHandler(object sender, KeyEventArgs e)
		{
			NppFormHelper.GenericKeyUpHandler(this, sender, e, false);
		}

		private void buttonClearLogs_Click(object sender, RoutedEventArgs e)
		{
			Logs = string.Empty;
		}
	}
}
