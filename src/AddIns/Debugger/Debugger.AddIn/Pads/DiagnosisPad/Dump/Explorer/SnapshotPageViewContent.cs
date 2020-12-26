using Debugger.MiniDump;
using ICSharpCode.SharpDevelop.Workbench;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer
{
	/// <summary>
	/// SnapshotPageViewContent snapshot detail panel
	/// </summary>
	/// <seealso cref="ICSharpCode.SharpDevelop.Workbench.AbstractViewContent" />
	public class SnapshotPageViewContent : AbstractViewContent
	{
		/// <summary>
		/// The dump
		/// </summary>
		private DumpCreator _dump;
		/// <summary>
		/// The content
		/// </summary>
		private SnapshotControl _content;

		/// <summary>
		/// Gets the control.
		/// </summary>
		/// <value>
		/// The control.
		/// </value>
		public override object Control
		{
			get
			{
				return Content;
			}
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		public override void Dispose()
		{
			if (_dump != null)
				_dump.MiniDumpFile.Dispose();

			base.Dispose();
		}

		/// <summary>
		/// Gets or sets the dump.
		/// </summary>
		/// <value>
		/// The dump.
		/// </value>
		public DumpCreator Dump { get => _dump; set => _dump = value; }
		/// <summary>
		/// Gets or sets the content.
		/// </summary>
		/// <value>
		/// The content.
		/// </value>
		public SnapshotControl Content { get => _content; set => _content = value; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SnapshotPageViewContent"/> class.
		/// </summary>
		/// <param name="dump">The dump.</param>
		public SnapshotPageViewContent(DumpCreator dump)
		{
			this._dump = dump;
			Content = new SnapshotControl(dump);
			SetLocalizedTitle(System.IO.Path.GetFileName(dump.DumpFileName));
		}
	}
}