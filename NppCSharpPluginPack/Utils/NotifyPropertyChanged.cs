using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NppDemo.Utils
{
	/// <summary>
	/// Enables UI bindings without as much ceremony.
	/// </summary>
	public class NotifyPropertyChanged : INotifyPropertyChanged
	{
		internal bool setValue<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
		{
			if (true != field?.Equals(newValue))
			{
				field = newValue;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
				return true;
			}
			return false;
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
