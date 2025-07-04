﻿using Kbg.NppPluginNET;
using NppDemo.Forms;
using NppDemo.PluginInfrastructure;
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
using System.Windows.Input;
using System.Windows.Interop;
using Path = System.IO.Path;
using Form = System.Windows.Forms.Form;
using DockStyle = System.Windows.Forms.DockStyle;

namespace NppDemo.Views
{
	public partial class SelectionRememberingControl : UserControl
	{
		/// <summary>
		/// This gets your modeless WPF dialog prepared just right, so you can type in it and see it.
		/// https://community.notepad-plus-plus.org/topic/26930/hi-and-i-m-working-on-a-wpf-fork-of-nppcsharppluginpack/16
		/// </summary>
		public static Form MakeModelessDialog(string title)
		{
			// The inner WPF content.
			var innerWpf = new SelectionRememberingControl();

			// The glue layer. ElementHost allows Forms to contain WPF.
			var middleHost = new ElementHostEx() { Child = innerWpf, Dock = DockStyle.Fill };

			// The outer WinForms Form that allows responding to GETDLGCODE messages.
			var outerForm = new OuterForm
			{
				Text = title,
				Dock = DockStyle.Fill,
				NotifyVisibilityChanged = innerWpf.handleVisibilityChanged, // WPF's IsVisibleChanged doesn't fire off.
			};
			
			outerForm.Controls.Add(middleHost);

			// Calling this enables keyboard commands like Ctrl+C and Ctrl+V, but prevents typing into modeless dialog TextBoxes unless you call a method like ChildHwndSourceHook in your dialog's Loaded callback.
			// Pieced together from: https://npp-user-manual.org/docs/plugin-communication/#2036nppm_modelessdialog
			// and https://stackoverflow.com/q/835878/1217612.
			NppFormHelper.RegisterFormIfModeless(outerForm, false);

			return outerForm;
		}

		private void handleVisibilityChanged(bool beingShown)
		{
			if (beingShown)
			{
				WpfStyle.ApplyStyle(this, Main.settings.use_npp_styling);
			}
			else
			{
				_darkModeTestWindow?.Close();
			}
		}

		private DarkModeTestWindow _darkModeTestWindow;

		public SelectionRememberingControl()
		{
			InitializeComponent();
			WpfStyle.ApplyStyle(this, Main.settings.use_npp_styling);

			// Because https://stackoverflow.com/q/835878/1217612 says we need to, to get typing in TextBoxes. Super! Not needed for separate WPF windows, AFAICT.
			Loaded += selectionRememberingControl_Loaded;
		}

		private void selectionRememberingControl_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= selectionRememberingControl_Loaded;

			var s = PresentationSource.FromVisual(this) as HwndSource;
			s?.AddHook(new HwndSourceHook(ChildHwndSourceHook));
		}

		IntPtr ChildHwndSourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			var id = (WM)msg;
			if (id == WM.GETDLGCODE && IsVisible)
			{
				handled = true; // This method enables typing in TextBoxes. Hooray!
				return (IntPtr)(DLGC.WANTALLKEYS); // better than arrows and chars when you want to accept Return key.
			}
			return IntPtr.Zero;
		}




		public string SelectionText { get => (string)GetValue(SelectionTextProperty); set => SetValue(SelectionTextProperty, value); }
		public static readonly DependencyProperty SelectionTextProperty = DependencyProperty.Register(nameof(SelectionText), typeof(string), typeof(SelectionRememberingControl), new PropertyMetadata(""));

		private void copySelectionsToStartEndsButton_Click(object sender, RoutedEventArgs e)
		{
			string startEnds = SelectionManager.StartEndListToString(SelectionManager.GetSelectedRanges());
			Clipboard.SetText(startEnds);
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
	}
}
