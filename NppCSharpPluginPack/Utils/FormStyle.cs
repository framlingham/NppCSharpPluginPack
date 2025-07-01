using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Forms;
//using System.Windows.Media;
using Brush = System.Windows.Media.SolidColorBrush;
using wColor = System.Windows.Media.Color;

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

		private readonly static string _nppBaseStyleKey = "BaseNppControlStyle";
		private static System.Windows.Style _nppControlStyle;
		private static Brush _nppBackgroundBrush;
		private static Brush _nppForegroundBrush;
        private static Brush _mostlyBackgroundBrush;
		private static wColor[] _darkModeColors;
		public static wColor GetDarkModeColor(int i) => _darkModeColors[i];


        private static wColor interpolateColor(double t, wColor t0, wColor t1)
        {
            return wColor.FromRgb((byte)(t0.R + t * (t1.R - t0.R)),
                                  (byte)(t0.G + t * (t1.G - t0.G)),
                                  (byte)(t0.B + t * (t1.B - t0.B)));
		}

		private static void populateStyles()
		{
			/*
            [0] background
            [1] softerBackground
            [2] hotBackground
            [3] pureBackground
	        [4] errorBackground
	        [5] text
	        [6] darkerText
	        [7] disabledText
	        [8] linkText
	        [9] edge
	        [10] hotEdge
	        [11] disabledEdge
             */
		}

        public static void ApplyStyle(System.Windows.UIElement element, bool useNppStyle)
        {
            if (!Npp.nppVersionAtLeast8)
            {
                useNppStyle = false; // trying to follow editor style looks weird for Notepad++ 7.3.3
            }


			var foreColor = Npp.notepad.GetDefaultForeColor();
			_nppForegroundBrush = makeBrush(foreColor);

			var backColor = Npp.notepad.GetDefaultBackColor();
			_nppBackgroundBrush = makeBrush(backColor);

			var backishColor = interpolateColor(0.25, backColor, foreColor);
			_mostlyBackgroundBrush = makeBrush(backishColor);

			_nppControlStyle = makeStyle(typeof(System.Windows.Controls.Control),
					null,
					(System.Windows.Controls.Control.BackgroundProperty, _nppBackgroundBrush),
					(System.Windows.Controls.Control.ForegroundProperty, _nppForegroundBrush),
					(System.Windows.Controls.Control.BorderBrushProperty, _mostlyBackgroundBrush));
			_darkModeColors = Npp.notepad.GetDarkModeColors();

			bool darkModeIsOn = Npp.notepad.IsDarkModeEnabled();
            var control = (System.Windows.FrameworkElement)element; // Required to put styles in Resources.
            control.Resources.Add(_nppBaseStyleKey, _nppControlStyle); // This doesn't do anything for UserControl...
			control.Style = _nppControlStyle; // ... so attach this one directly.

			var softBg = makeBrush(_darkModeColors[1]);
			var hotBg = makeBrush(_darkModeColors[2]);
			var pureBg = makeBrush(_darkModeColors[3]);

			void makeAndSetStyle(Type type, Action<System.Windows.Style> darkModeBits = null)
            {
                var style = makeStyle(type, _nppControlStyle);
                if (darkModeIsOn)
                {
					// Used to update the style's colors before we add them, so that the style is not frozen yet.
					darkModeBits?.Invoke(style);
                }
                control.Resources.Add(type, style);
			}

            makeAndSetStyle(typeof(System.Windows.Controls.CheckBox));
            makeAndSetStyle(typeof(System.Windows.Controls.ComboBox));
            makeAndSetStyle(typeof(System.Windows.Controls.DataGrid));
            makeAndSetStyle(typeof(System.Windows.Controls.Label));
            makeAndSetStyle(typeof(System.Windows.Controls.ListBox));
            makeAndSetStyle(typeof(System.Windows.Controls.TextBox),
                style =>
                {
                    style.Setters.Add(new System.Windows.Setter(System.Windows.Controls.Control.BackgroundProperty, softBg));
                });
            makeAndSetStyle(typeof(System.Windows.Controls.TreeView));
            makeAndSetStyle(typeof(System.Windows.Controls.Button),
                style =>
                {
                    style.Setters.Add(new System.Windows.Setter(System.Windows.Controls.Control.BackgroundProperty, softBg));

                    #region All this to override the basic WPF Button's mouse-over highlighting color.
                    // I'm glad I had AI to help with this or else this would have taken way longer.
                    var buttonTemplate = new System.Windows.Controls.ControlTemplate(typeof(System.Windows.Controls.Button));
                    var border = new System.Windows.FrameworkElementFactory(typeof(System.Windows.Controls.Border));

                    border.SetValue(System.Windows.Controls.Border.BackgroundProperty, new System.Windows.TemplateBindingExtension(System.Windows.Controls.Border.BackgroundProperty));
                    border.SetValue(System.Windows.Controls.Border.BorderThicknessProperty, new System.Windows.TemplateBindingExtension(System.Windows.Controls.Control.BorderThicknessProperty));
                    border.SetValue(System.Windows.Controls.Border.BorderBrushProperty, new System.Windows.TemplateBindingExtension(System.Windows.Controls.Control.BorderBrushProperty));
                    border.SetValue(System.Windows.Controls.Border.PaddingProperty, new System.Windows.TemplateBindingExtension(System.Windows.Controls.Control.PaddingProperty));

                    var content = new System.Windows.FrameworkElementFactory(typeof(System.Windows.Controls.ContentPresenter));
                    content.SetValue(System.Windows.FrameworkElement.HorizontalAlignmentProperty, new System.Windows.TemplateBindingExtension(System.Windows.Controls.Control.HorizontalContentAlignmentProperty));
                    border.AppendChild(content);
                    buttonTemplate.VisualTree = border;
                    style.Setters.Add(new System.Windows.Setter(System.Windows.Controls.Control.TemplateProperty, buttonTemplate));

                    style.Triggers.Add(new System.Windows.Trigger
                    {
                        Property = System.Windows.UIElement.IsMouseOverProperty,
                        Value = true,
                        Setters =
                        {
                            new System.Windows.Setter(System.Windows.Controls.Control.BackgroundProperty, hotBg)
                        }
                    });
                    #endregion Phew!
                });
        }

		private static Brush makeBrush(wColor color)
        {
            Brush brush = new(color);
            brush.Freeze(); // Make immutable for performance.
            return brush;
		}
        private static System.Windows.Style makeStyle(Type type, System.Windows.Style basedOn = null, params (System.Windows.DependencyProperty, object)[] setterBits)
        {
            var style = new System.Windows.Style(type, basedOn);
            foreach (var (property, value) in setterBits)
            {
                style.Setters.Add(new System.Windows.Setter(property, value));
			}
            return style;
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
