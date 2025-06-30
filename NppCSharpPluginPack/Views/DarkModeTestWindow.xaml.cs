using Kbg.NppPluginNET;
using NppDemo.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	/// Interaction logic for DarkModeTestWindow.xaml
	/// </summary>
	public partial class DarkModeTestWindow : Window
	{
		public DarkModeTestWindow()
		{
			InitializeComponent();
			FormStyle.ApplyStyle(this, Main.settings.use_npp_styling);
		}

		internal void GrabFocus()
		{
			textBox1.Focus();
		}



		public ObservableCollection<DGItem> DataGridItems { get => (ObservableCollection<DGItem>)GetValue(DataGridItemsProperty); set => SetValue(DataGridItemsProperty, value); }
		public static readonly DependencyProperty DataGridItemsProperty = DependencyProperty.Register(nameof(DataGridItems), typeof(ObservableCollection<DGItem>), typeof(DarkModeTestWindow), new PropertyMetadata(new ObservableCollection<DGItem>() { new DGItem("Value1", "Should look pretty", "Value3"), new DGItem("Another 1", "So pretty", "Further") }));

		public class DGItem
		{
			public DGItem(string column1, string column2, string column3)
			{
				Column1 = column1;
				Column2 = column2;
				Column3 = column3;
			}

			public string Column1 { get; set; }
			public string Column2 { get; set; }
			public string Column3 { get; set; }
		}

		private void ShowPopupDialogButton_Click(object sender, RoutedEventArgs e)
		{
			var popup = new PopupWindow() { Owner = this };
			popup.ShowDialog();
		}
	}
}
