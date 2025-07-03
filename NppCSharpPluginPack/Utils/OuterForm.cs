using NppDemo.PluginInfrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NppDemo.Utils
{
	internal class OuterForm : Form
	{
		public Action<bool> NotifyVisibilityChanged;

		protected override void WndProc(ref Message m)
		{
			if ((WM)m.Msg == WM.SHOWWINDOW)
			{
				bool beingShown = m.WParam != IntPtr.Zero;
				NotifyVisibilityChanged?.Invoke(beingShown);
			}
			base.WndProc(ref m);
		}
	}
}
