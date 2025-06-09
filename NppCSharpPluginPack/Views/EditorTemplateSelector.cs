using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace NppDemo.Views
{
	public class EditorTemplateSelector : DataTemplateSelector
	{
		public DataTemplate TextBoxTemplate { get; set; }
		public DataTemplate ComboBoxTemplate { get; set; }
		public DataTemplate BoolTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item is PropertyItem property)
			{
				if (property.PropertyType == typeof(string))
				{
					return TextBoxTemplate;
				}
				else if (property.PropertyType == typeof(bool))
				{
					return BoolTemplate;
				}
				else if (property.PropertyType.IsEnum)
				{
					return ComboBoxTemplate;
				}
			}
			return base.SelectTemplate(item, container);
		}
	}
}
