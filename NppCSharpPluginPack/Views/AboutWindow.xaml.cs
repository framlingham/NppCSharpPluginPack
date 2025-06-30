using Kbg.NppPluginNET;
using NppDemo.Utils;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace NppDemo.Views
{
	/// <summary>
	/// Interaction logic for AboutWindow.xaml
	/// </summary>
	public partial class AboutWindow : Window
	{
		public AboutWindow()
		{
			InitializeComponent();
			Version = Npp.AssemblyVersionString();
			FormStyle.ApplyStyle(this, Main.settings.use_npp_styling);
		}

		public string Version { get => (string)GetValue(VersionProperty); set => SetValue(VersionProperty, value); }
		public static readonly DependencyProperty VersionProperty = DependencyProperty.Register(nameof(Version), typeof(string), typeof(AboutWindow), new PropertyMetadata("X.Y.Z.A"));

		private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			// Wow, this is fiddly! https://stackoverflow.com/questions/12742690/c-sharp-hyperlink-in-textblock-nothing-happens-when-i-click-on-it
			System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
		}
    }
}
