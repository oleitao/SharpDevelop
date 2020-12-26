using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Workbench;
using System.Linq;

namespace Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer
{
	/// <summary>
	/// ShowSnapshotPageCommand class
	/// </summary>
	/// <seealso cref="ICSharpCode.Core.AbstractMenuCommand" />
	public class ShowSnapshotPageCommand : AbstractMenuCommand
	{
		/// <summary>
		/// The dump
		/// </summary>
		private DumpCreator _dump;

		/// <summary>
		/// Gets or sets the dump.
		/// </summary>
		/// <value>
		/// The dump.
		/// </value>
		public DumpCreator Dump { get => _dump; set => _dump = value; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ShowSnapshotPageCommand"/> class.
		/// </summary>
		/// <param name="dump">The dump.</param>
		public ShowSnapshotPageCommand(DumpCreator dump)
		{
			this._dump = dump;
		}

		/// <summary>
		/// Initializes the <see cref="ShowSnapshotPageCommand"/> class.
		/// </summary>
		static ShowSnapshotPageCommand()
		{
			SD.ProjectService.SolutionOpened += delegate {
				// close all start pages when loading a solution
				foreach (IViewContent v in SD.Workbench.ViewContentCollection.ToArray())
				{
					if (v is SnapshotPageViewContent)
					{
						v.WorkbenchWindow.CloseWindow(true);
					}
				}
			};
		}

		/// <summary>
		/// Runs this instance.
		/// </summary>
		public override void Run()
		{
			foreach (IViewContent view in SD.Workbench.ViewContentCollection)
			{
				if (view is ShowSnapshotPageCommand)
				{
					view.WorkbenchWindow.SelectWindow();
					return;
				}
			}
			SD.Workbench.ShowView(new SnapshotPageViewContent(_dump));
		}
	}
}