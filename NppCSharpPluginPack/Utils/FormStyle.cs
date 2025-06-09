using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
//using System.Windows.Media;

namespace NppDemo.Utils
{
    public static class FormStyle
    {
        static FormStyle()
        {
            populateStyles();
        }


		public static Color SlightlyDarkControl = Color.FromArgb(
            3 * SystemColors.Control.R / 4 + SystemColors.ControlDark.R / 4,
            3 * SystemColors.Control.G / 4 + SystemColors.ControlDark.G / 4,
            3 * SystemColors.Control.B / 4 + SystemColors.ControlDark.B / 4
        );

        public static void ApplyStyle(System.Windows.UIElement element, bool useNppStyle)
		{
            if (element == null)
            {
                return;
            }
            if (!Npp.nppVersionAtLeast8)
            {
                useNppStyle = false; // trying to follow editor style looks weird for Notepad++ 7.3.3
            }

            // The rest from the original code below.
			if (element is System.Windows.Controls.Control control)
			{
                // Update the style's colors


                // Attach it!
                control.Style = _nppControlStyle;
			}
		}

        private static System.Windows.Style _nppControlStyle;
        private static System.Windows.Media.SolidColorBrush _nppBackgroundBrush;
        private static System.Windows.Media.SolidColorBrush _nppForegroundBrush;


		private static void populateStyles()
        {
            if (_nppBackgroundBrush == null)
            {
                _nppBackgroundBrush = new System.Windows.Media.SolidColorBrush(Npp.notepad.GetDefaultBackColor());
                _nppBackgroundBrush.Freeze();
            }
			if (_nppForegroundBrush == null)
			{
				_nppForegroundBrush = new System.Windows.Media.SolidColorBrush(Npp.notepad.GetDefaultForeColor());
				_nppForegroundBrush.Freeze();
			}
			if (_nppControlStyle == null)
            {
				_nppControlStyle = new System.Windows.Style(typeof(System.Windows.Controls.Control));
				_nppControlStyle.Setters.Add(new System.Windows.Setter(System.Windows.Controls.Control.BackgroundProperty,
					_nppBackgroundBrush));
				_nppControlStyle.Setters.Add(new System.Windows.Setter(System.Windows.Controls.Control.ForegroundProperty,
					_nppForegroundBrush));
			}
        }

        public static void UpdateStyle(System.Windows.Window window)
        {
            
		}

		/// <summary>
		/// Changes the background and text color of the form
		/// and any child forms to match the editor window.<br></br>
		/// Fires when the form is first opened
		/// and also whenever the style is changed.<br></br>
		/// Heavily based on CsvQuery (https://github.com/jokedst/CsvQuery)
		/// </summary>
		public static void ApplyStyle(Control ctrl, bool useNppStyle, bool darkMode = false)
        {
            if (ctrl == null || ctrl.IsDisposed) return;
            if (!Npp.nppVersionAtLeast8)
                useNppStyle = false; // trying to follow editor style looks weird for Notepad++ 7.3.3
            if (ctrl is Form form)
            {
                foreach (Form childForm in form.OwnedForms)
                {
                    ApplyStyle(childForm, useNppStyle, darkMode);
                }
            }
            Color backColor = Npp.notepad.GetDefaultBackgroundColor();
            if (!useNppStyle || (
                backColor.R > 240 &&
                backColor.G > 240 &&
                backColor.B > 240))
            {
                // if the background is basically white,
                // use the system defaults because they
                // look best on a white or nearly white background
                ctrl.BackColor = SystemColors.Control;
                ctrl.ForeColor = SystemColors.ControlText;
                foreach (Control child in ctrl.Controls)
                {
                    if (child is GroupBox)
                        ApplyStyle(child, useNppStyle, darkMode);
                    // controls containing text
                    else if (child is TextBox || child is ListBox || child is ComboBox || child is TreeView)
                    {
                        child.BackColor = SystemColors.Window; // white background
                        child.ForeColor = SystemColors.WindowText;
                    }
                    else if (child is DataGridView dgv)
                    {
                        dgv.EnableHeadersVisualStyles = true;
                        dgv.BackgroundColor = SystemColors.ControlDark;
                        dgv.ForeColor = SystemColors.ControlText;
                        dgv.GridColor = SystemColors.ControlLight;
                        dgv.RowsDefaultCellStyle.ForeColor = SystemColors.ControlText;
                        dgv.RowsDefaultCellStyle.BackColor = SystemColors.Window;
                    }
                    else
                    {
                        // buttons should be a bit darker but everything else is the same color as the background
                        child.BackColor = (child is Button) ? SlightlyDarkControl : SystemColors.Control;
                        child.ForeColor = SystemColors.ControlText;
                        if (child is LinkLabel llbl)
                        {
                            llbl.LinkColor = Color.Blue;
                            llbl.ActiveLinkColor = Color.Red;
                            llbl.VisitedLinkColor = Color.Purple;
                        }
                    }
                }
                return;
            }
            Color foreColor = Npp.notepad.GetDefaultForegroundColor();
            ctrl.BackColor = backColor;
            Color InBetween = Color.FromArgb(
                foreColor.R / 4 + 3 * backColor.R / 4,
                foreColor.G / 4 + 3 * backColor.G / 4,
                foreColor.B / 4 + 3 * backColor.B / 4
            );
            foreach (Control child in ctrl.Controls)
            {
                child.BackColor = backColor;
                child.ForeColor = foreColor;
                if (child is GroupBox)
                    ApplyStyle(child, useNppStyle, darkMode);
                if (child is LinkLabel llbl)
                {
                    llbl.LinkColor = foreColor;
                    llbl.ActiveLinkColor = foreColor;
                    llbl.VisitedLinkColor = foreColor;
                }
                else if (child is DataGridView dgv)
                {
                    dgv.EnableHeadersVisualStyles = false;
                    dgv.BackgroundColor = InBetween;
                    dgv.ForeColor = foreColor;
                    dgv.GridColor = foreColor;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = foreColor;
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = backColor;
                    dgv.RowHeadersDefaultCellStyle.ForeColor = foreColor;
                    dgv.RowHeadersDefaultCellStyle.BackColor = backColor;
                    dgv.RowsDefaultCellStyle.ForeColor = foreColor;
                    dgv.RowsDefaultCellStyle.BackColor = backColor;
                }
            }
        }
    }
}
