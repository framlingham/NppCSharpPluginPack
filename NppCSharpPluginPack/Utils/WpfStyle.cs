using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NppDemo.Utils
{
	public class WpfStyle
	{

		private readonly static string _nppBaseStyleKey = "BaseNppControlStyle";
		private static Style _nppControlStyle;
		private static SolidColorBrush _nppBackgroundBrush;
		private static SolidColorBrush _nppForegroundBrush;
		private static SolidColorBrush _mostlyBackgroundBrush;
		private static Color[] _darkModeColors;
		public static Color GetDarkModeColor(int i) => _darkModeColors[i];


		private static Color interpolateColor(double t, Color t0, Color t1)
		{
			return Color.FromRgb((byte)(t0.R + t * (t1.R - t0.R)),
								  (byte)(t0.G + t * (t1.G - t0.G)),
								  (byte)(t0.B + t * (t1.B - t0.B)));
		}
		private static byte toGreyscale(Color color) => (byte)((color.R + color.G + color.B + 1) / 3);

		public static void ApplyStyle(UIElement element, bool useNppStyle)
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

			_nppControlStyle = makeStyle(typeof(Control),
					null,
					(Control.BackgroundProperty, _nppBackgroundBrush),
					(Control.ForegroundProperty, _nppForegroundBrush),
					(Control.BorderBrushProperty, _mostlyBackgroundBrush));
			_darkModeColors = Npp.notepad.GetDarkModeColors();
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

			bool darkModeIsOn = Npp.notepad.IsDarkModeEnabled() || toGreyscale(backColor) < 128;
			var control = (FrameworkElement)element; // Required to put styles in Resources.
			control.Resources[_nppBaseStyleKey] = _nppControlStyle; // This doesn't do anything for UserControl...
			control.Style = _nppControlStyle; // ... so attach this one directly.

			var softBg = makeBrush(_darkModeColors[1]);
			var hotBg = makeBrush(_darkModeColors[2]);
			var pureBg = makeBrush(_darkModeColors[3]);

			void makeAndSetStyle(Type type, Style baseStyle, Action<Style> darkModeBits = null)
			{
				var style = makeStyle(type, baseStyle);
				if (darkModeIsOn)
				{
					// Used to update the style's colors before we add them, so that the style is not frozen yet.
					darkModeBits?.Invoke(style);
				}
				control.Resources[type] = style;
			}

			makeAndSetStyle(typeof(CheckBox), _nppControlStyle);
			makeAndSetStyle(typeof(ComboBox), _nppControlStyle);
			makeAndSetStyle(typeof(DataGrid), _nppControlStyle);
			makeAndSetStyle(typeof(Label), _nppControlStyle);
			makeAndSetStyle(typeof(ListBox), _nppControlStyle);
			makeAndSetStyle(typeof(TextBlock), null, style =>
			{
				style.Setters.Add(new Setter(TextBlock.ForegroundProperty, _nppForegroundBrush));
			});
			makeAndSetStyle(typeof(TextBox), _nppControlStyle, style =>
			{
				style.Setters.Add(new Setter(Control.BackgroundProperty, softBg));
			});
			makeAndSetStyle(typeof(TreeView), _nppControlStyle);
			makeAndSetStyle(typeof(Button), _nppControlStyle, style =>
			{
				style.Setters.Add(new Setter(Control.BackgroundProperty, softBg));

				#region All this to override the basic WPF Button's mouse-over highlighting color.
				// I'm glad I had AI to help with this or else this would have taken way longer.
				var buttonTemplate = new ControlTemplate(typeof(Button));
				var border = new FrameworkElementFactory(typeof(Border));

				border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Border.BackgroundProperty));
				border.SetValue(Border.BorderThicknessProperty, new TemplateBindingExtension(Control.BorderThicknessProperty));
				border.SetValue(Border.BorderBrushProperty, new TemplateBindingExtension(Control.BorderBrushProperty));
				border.SetValue(Border.PaddingProperty, new TemplateBindingExtension(Control.PaddingProperty));

				var content = new FrameworkElementFactory(typeof(ContentPresenter));
				content.SetValue(FrameworkElement.HorizontalAlignmentProperty, new TemplateBindingExtension(Control.HorizontalContentAlignmentProperty));
				border.AppendChild(content);
				buttonTemplate.VisualTree = border;
				style.Setters.Add(new Setter(Control.TemplateProperty, buttonTemplate));

				style.Triggers.Add(new Trigger
				{
					Property = UIElement.IsMouseOverProperty,
					Value = true,
					Setters =
					{
						new Setter(Control.BackgroundProperty, hotBg),
					}
				});
				#endregion Phew!
			});
		}

		private static SolidColorBrush makeBrush(Color color)
		{
			SolidColorBrush brush = new(color);
			brush.Freeze(); // Make immutable for performance.
			return brush;
		}

		private static Style makeStyle(Type type, Style basedOn = null, params (DependencyProperty, object)[] setterBits)
		{
			var style = basedOn != null ? new Style(type, basedOn)
										: new Style(type);
			foreach (var (property, value) in setterBits)
			{
				style.Setters.Add(new Setter(property, value));
			}
			return style;
		}
	}
}
