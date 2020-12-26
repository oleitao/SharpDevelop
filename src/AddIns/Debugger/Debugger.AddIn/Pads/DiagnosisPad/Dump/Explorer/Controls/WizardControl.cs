using System;
using System.Windows.Forms;

namespace Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Controls
{
	/// <summary>
	/// WizardControl tabcontrol controler
	/// </summary>
	/// <seealso cref="System.Windows.Forms.TabControl" />
	public class WizardControl : TabControl
    {
		/// <summary>
		/// This member overrides <see cref="M:System.Windows.Forms.Control.WndProc(System.Windows.Forms.Message@)" />.
		/// </summary>
		/// <param name="m">A Windows Message Object.</param>
		protected override void WndProc(ref Message m)
        {
            // Hide tabs by trapping the TCM_ADJUSTRECT message
            if (m.Msg == 0x1328 && !DesignMode) m.Result = (IntPtr)1;
            else base.WndProc(ref m);
        }
    }
}
