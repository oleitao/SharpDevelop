using Debugger.MiniDump;
using System;
using System.Windows.Forms;

namespace Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Views
{
	/// <summary>
	/// ExceptionStreamView view
	/// </summary>
	/// <seealso cref="Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Views.BaseViewControl" />
	public partial class ExceptionStreamView : BaseViewControl
    {
		/// <summary>
		/// The exception stream
		/// </summary>
		private MiniDumpExceptionStream _exceptionStream;

		/// <summary>
		/// Initializes a new instance of the <see cref="ExceptionStreamView"/> class.
		/// </summary>
		public ExceptionStreamView()
        {
            InitializeComponent();
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="ExceptionStreamView"/> class.
		/// </summary>
		/// <param name="exceptionStream">The exception stream.</param>
		public ExceptionStreamView(MiniDumpExceptionStream exceptionStream)
            : this()
        {
            _exceptionStream = exceptionStream;

            if (exceptionStream == null)
            {
                this.treeView1.Nodes[0].Text = "No data found for stream";
            }
            else
            {
                AddExceptionNode(this.treeView1.Nodes[0], exceptionStream.ExceptionRecord);

                this.treeView1.ExpandAll();
            }
        }

		/// <summary>
		/// Adds the exception node.
		/// </summary>
		/// <param name="exceptionNode">The exception node.</param>
		/// <param name="exception">The exception.</param>
		private void AddExceptionNode(TreeNode exceptionNode, MiniDumpException exception)
        {
            exceptionNode.Nodes.Add(String.Format("ExceptionCode: 0x{0:x8}", exception.ExceptionCode));

            if (exception.ExceptionFlags == MiniDumpException.EXCEPTION_NONCONTINUABLE)
                exceptionNode.Nodes.Add("ExceptionFlags: EXCEPTION_NONCONTINUABLE");
            else
                exceptionNode.Nodes.Add(String.Format("ExceptionFlags: {0}", exception.ExceptionFlags));

            exceptionNode.Nodes.Add(String.Format("ExceptionRecord: 0x{0:x8}", exception.ExceptionRecordRaw));
            exceptionNode.Nodes.Add(String.Format("ExceptionAddress: 0x{0:x8}", exception.ExceptionAddress));
            exceptionNode.Nodes.Add(String.Format("NumberParameters: {0}", exception.NumberParameters));

            TreeNode informationNode = exceptionNode.Nodes.Add("ExceptionInformation");

            for (int i = 0; i < exception.ExceptionInformation.Length; i++)
                informationNode.Nodes.Add(String.Format("[{0}]: 0x{1:x8}", i, exception.ExceptionInformation[i]));
        }
    }
}
