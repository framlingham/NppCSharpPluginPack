using Kbg.NppPluginNET.PluginInfrastructure;
using Kbg.NppPluginNET;
using NppDemo.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace NppDemo.Views
{
	public class SettingsWindow : SettingsWindowBase
	{
		public SettingsWindow()
		{
			InitializeComponent();
		}

		public void ShowIt()
		{
			// make sure to check the HTML tag plugin menu item if the setting is currently true
			PluginBase.CheckMenuItem(Main.IdCloseHtmlTag, close_html_tag);

			PropertyItem toPropertyItem(PropertyInfo pi) => new PropertyItem(pi, this);
			Copy = (SettingsWindow)MemberwiseClone();
			var type = GetType();
			var allProps = Copy.GetType().GetProperties().Where(p => p.CanRead && p.CanWrite && p.DeclaringType == type);
			Properties = allProps.Select(toPropertyItem).ToArray();
			base.ShowDialog();
		}

		#region MISCELLANEOUS
		[Description("Specify one of these chars for each toolbar icon you want to show, in the order you want:\r\n" +
					"('a' = about form, 's' = selection remembering form, 'h' = automatically close HTML tags)\r\n" +
					"This setting will take effect the next time you start Notepad++.\r\n" +
					"If you want there to be NO toolbar icons, enter a character that does not represent an icon; do NOT leave this field empty."),
			Category("Miscellaneous"), DefaultValue("ash")]
		public string toolbar_icons { get; set; }

		[Description("Automatically close HTML/XML tags when the current file extension is \"html\" or \"xml\"?"),
			Category("Miscellaneous"), DefaultValue(false)]
		public bool close_html_tag { get; set; }
		#endregion

		#region STYLING
		[Description("Use the same colors as the editor window for this plugin's forms?"),
			Category("Styling"), DefaultValue(true)]
		public bool use_npp_styling { get; set; }
		#endregion
		#region TESTING
		[Description("Ask before running tests, because the test can hijack the user's clipboard"),
			Category("Testing"), DefaultValue(AskUserWhetherToDoThing.ASK_BEFORE_DOING)]
		public AskUserWhetherToDoThing ask_before_testing { get; set; }
		#endregion
	}
}
