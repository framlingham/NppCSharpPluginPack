using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NppDemo.Utils
{
	[ValueConversion(typeof(object), typeof(Type))]
	public class TheType : IValueConverter
	{
		/// <summary>
		/// Get this in XAML by using {x:Static util:TheType.Of}
		/// </summary>
		public static TheType Of { get; } = new TheType();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value?.GetType() ?? Binding.DoNothing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(Type), typeof(IEnumerable<Enum>))]
	public class EnumType : IValueConverter
	{
		public static EnumType ToRange { get; } = new EnumType();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Type enumType && enumType.IsEnum)
			{
				return Enum.GetValues(enumType);
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(object), typeof(string))]
	public class StringTo : IValueConverter
	{
		public static StringTo HumanReadable { get; } = new StringTo();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null)
			{
				return ToHumanReadable(value as string ?? value.ToString());
			}
			return Binding.DoNothing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public static string ToHumanReadable(string text)
		{
			// Make easier to read. For example, replace underscores with spaces and capitalize the first letter of each word.
			text = text.ToLower();
			text = text.Replace("npp", "NPP");
			text = text.Replace("html", "HTML");
			text = text.Replace("_", " ");
			text = Regex.Replace(text, @"\b\w", match => match.Value.ToUpper());
			text = text.Replace("Dont", "Don't");

			// Some words should always be lower case:
			text = text.Replace(" Do ", " do ");
			return text;
		}
	}



}
