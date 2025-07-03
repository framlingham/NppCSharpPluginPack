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
	/// Interaction logic for PopupWindow.xaml
	/// </summary>
	public partial class PopupWindow : Window
	{
		public PopupWindow()
		{
			InitializeComponent();
			WpfStyle.ApplyStyle(this, Main.settings.use_npp_styling);
		}

		public string TextFromBox { get => (string)GetValue(TextFromBoxProperty); set => SetValue(TextFromBoxProperty, value); }
		public static readonly DependencyProperty TextFromBoxProperty = DependencyProperty.Register(nameof(TextFromBox), typeof(string), typeof(PopupWindow), new PropertyMetadata(""));

		private void Button1_Click(object sender, RoutedEventArgs e)
		{
			string msg = ComboBox1.IsEnabled
							? $"ComboBox1 selected value = {ComboBox1.Text}"
							: "ComboBox1 is disabled.";
			MessageBox.Show(msg);
		}
	}
}
