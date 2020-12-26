using Debugger.MiniDump;
using System;
using System.Windows.Forms;

namespace Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Views
{
	/// <summary>
	/// ThreadInfoListView view
	/// </summary>
	/// <seealso cref="Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Views.BaseViewControl" />
	public partial class ThreadInfoListView : BaseViewControl
    {
		/// <summary>
		/// The thread information list
		/// </summary>
		private MiniDumpThreadInfo[] _threadInfoList;

		/// <summary>
		/// Initializes a new instance of the <see cref="ThreadInfoListView"/> class.
		/// </summary>
		public ThreadInfoListView()
        {
            InitializeComponent();
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="ThreadInfoListView"/> class.
		/// </summary>
		/// <param name="threadInfoList">The thread information list.</param>
		public ThreadInfoListView(MiniDumpThreadInfo[] threadInfoList)
            : this()
        {
            _threadInfoList = threadInfoList;

            if (_threadInfoList.Length == 0)
            {
                this.listView1.Items.Add("No data found for stream");
            }
            else
            {
                foreach (MiniDumpThreadInfo thread in _threadInfoList)
                {
                    ListViewItem newItem = new ListViewItem("0x" + thread.ThreadId.ToString("x8") + " (" + thread.ThreadId + ")");
                    newItem.SubItems.Add(thread.DumpFlags.ToString());
                    newItem.SubItems.Add("0x" + thread.DumpError.ToString("x8"));
                    newItem.SubItems.Add((thread.ExitStatus == MiniDumpThreadInfo.STILL_ACTIVE) ? "STILL_ACTIVE" : thread.ExitStatus.ToString());
                    newItem.SubItems.Add(thread.CreateTime.ToString());

                    if (thread.ExitTime == DateTime.MinValue)
                        newItem.SubItems.Add("");
                    else
                        newItem.SubItems.Add(thread.ExitTime.ToString());

                    newItem.SubItems.Add(FormattedTimeSpan(thread.KernelTime));
                    newItem.SubItems.Add(FormattedTimeSpan(thread.UserTime));
                    newItem.SubItems.Add("0x" + thread.StartAddress.ToString("x8"));
                    newItem.SubItems.Add(thread.Affinity.ToString());

                    newItem.Tag = thread;

                    this.listView1.Items.Add(newItem);
                }
            }
        }

		/// <summary>
		/// Formatteds the time span.
		/// </summary>
		/// <param name="timeSpan">The time span.</param>
		/// <returns></returns>
		private string FormattedTimeSpan(TimeSpan timeSpan)
        {
            return timeSpan.ToString();

            //if (timeSpan.TotalMilliseconds < 1000)
            //    return String.Format("{0}ms", timeSpan.TotalMilliseconds);
            //else if (timeSpan.TotalSeconds < 60)
            //    return String.Format("{0}.{1}s", timeSpan.Seconds, timeSpan.Milliseconds);
            //else if (timeSpan.TotalMinutes < 60)
            //    return String.Format("{0}:{1}min", timeSpan.Minutes, timeSpan.Seconds);
            //else
            //    return timeSpan.ToString();
        }
    }
}
