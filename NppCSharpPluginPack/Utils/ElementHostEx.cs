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

			// Check the base class first, so it can add to the result if needed (https://stackoverflow.com/a/18264366/1217612).
			if (true != Handler?.Invoke(ref m))
			{
				base.WndProc(ref m);
			}

			if (msg == WM.SHOWWINDOW)
			{
				Visible = m.WParam != IntPtr.Zero;
			}
			else if (msg == WM.SIZE)
			{
				RefreshVisuals();
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
