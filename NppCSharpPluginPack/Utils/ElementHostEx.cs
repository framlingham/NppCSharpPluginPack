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
			const int WM_SHOWWINDOW = 0x0018;
			const int WM_SIZE = 0x0005;
			if (m.Msg == WM_SHOWWINDOW)
			{
				Visible = m.WParam != IntPtr.Zero;
			}
			else if (m.Msg == WM_SIZE)
			{
				RefreshVisuals();
			}
			if (true != Handler?.Invoke(ref m))
			{
				base.WndProc(ref m);
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
