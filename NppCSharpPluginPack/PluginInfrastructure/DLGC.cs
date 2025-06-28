using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NppDemo.PluginInfrastructure
{
	/// <summary>
	/// The return values for WM_GETDLGCODE, created from 
	/// https://learn.microsoft.com/en-us/windows/win32/dlgbox/wm-getdlgcode
	/// Is using https://github.com/microsoft/CsWin32 better? This is quite concise, so I'll keep this way for now.
	/// </summary>
	[Flags]
	public enum DLGC : uint
	{
		/// <summary>
		/// Button.
		/// </summary>
		BUTTON = 0x2000,

		/// <summary>
		/// Default push button.
		/// </summary>
		DEFPUSHBUTTON = 0x0010,

		/// <summary>
		/// EM_SETSEL messages.
		/// </summary>
		HASSETSEL = 0x0008,

		/// <summary>
		/// Radio button.
		/// </summary>
		RADIOBUTTON = 0x0040,

		/// <summary>
		/// Static control.
		/// </summary>
		STATIC = 0x0100,

		/// <summary>
		/// Non-default push button.
		/// </summary>
		UNDEFPUSHBUTTON = 0x0020,

		/// <summary>
		/// All keyboard input.
		/// </summary>
		WANTALLKEYS = 0x0004,

		/// <summary>
		/// Direction keys.
		/// </summary>
		WANTARROWS = 0x0001,

		/// <summary>
		/// WM_CHAR messages.
		/// </summary>
		WANTCHARS = 0x0080,

		/// <summary>
		/// All keyboard input (the application passes this message in the MSG structure to the control).
		/// </summary>
		WANTMESSAGE = 0x0004,

		/// <summary>
		/// TAB key.
		/// </summary>
		WANTTAB = 0x0002,
	}
}
