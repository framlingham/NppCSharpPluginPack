using NppDemo.PluginInfrastructure;
using NppDemo.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Threading;

namespace NppDemo.Utils
{
	/// <summary>
	/// Extends <see cref="ElementHost"/> to hook up Visible based on WM_SHOWWINDOW, and allows custom handling of messages via <see cref="Handler"/>.
	/// </summary>
	public class ElementHostEx : ElementHost
	{
		public delegate bool WndProcDelegate(ref Message m);
		public WndProcDelegate Handler;

		protected override void WndProc(ref Message m)
		{
			var msg = (WM)m.Msg; // Get the message ID from the Message struct.
			if (msg == WM.GETTEXT || msg == WM.CHAR)
			{
			}

			// Check the base class first, so it can set the result.
			if (true != Handler?.Invoke(ref m))
			{
				base.WndProc(ref m);
			}
			// Now we can override the m.Result if needed, or just log the message.
			// (https://stackoverflow.com/a/18264366/1217612)

			if (msg == WM.SHOWWINDOW)
			{
				Visible = m.WParam != IntPtr.Zero;
			}
			else if (msg == WM.SIZE)
			{
				RefreshVisuals();
			}
			else if (msg == WM.GETDLGCODE)
			{
				// Just found this that seems to describe this situation exactly! https://stackoverflow.com/q/835878/1217612
				// What to put here?
				// Check to see what the Message in m.LParam is?
				m.Result = (IntPtr)((uint)m.Result | (uint)(DLGC.WANTCHARS | DLGC.WANTARROWS));
			}
			
			if (msg != WM.SETFOCUS && msg != WM.KILLFOCUS && msg != WM.GETTEXT /*&& m.Msg != 135*/) // Skip logging these for now.
			{
				SelectionRememberingControl.LogMessage?.Invoke($"{msg} {m.WParam} {m.LParam}");
				if (msg == WM.GETDLGCODE)
				{
					SelectionRememberingControl.LogMessage?.Invoke($"    Result: {(DLGC)m.Result}");
				}
			}
		}

		public void RefreshVisuals()
		{
			// When resizing, I get large blank areas near the resize bar. This minimizes the blanked out area, but there's still a bit missing unless I delay the redraw by putting it in the dispatcher queue.
			Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
			{
				Child?.InvalidateVisual();
				Child?.UpdateLayout();
				Invalidate();
				Update();
			}));
		}
	}
}
