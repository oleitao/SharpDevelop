﻿using System.Windows.Forms;
using Debugger.MiniDump;

namespace Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Views
{
	/// <summary>
	/// MiscInfoView view
	/// </summary>
	/// <seealso cref="Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.Views.BaseViewControl" />
	public partial class MiscInfoView : BaseViewControl
    {
		/// <summary>
		/// The misc information
		/// </summary>
		private MiniDumpMiscInfo _miscInfo;

		/// <summary>
		/// Initializes a new instance of the <see cref="MiscInfoView"/> class.
		/// </summary>
		public MiscInfoView()
        {
            InitializeComponent();
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="MiscInfoView"/> class.
		/// </summary>
		/// <param name="miscInfo">The misc information.</param>
		public MiscInfoView(MiniDumpMiscInfo miscInfo)
            : this()
        {
            _miscInfo = miscInfo;

            AddInfoNode("Flags", _miscInfo.Flags1.ToString());

            if (_miscInfo.Flags1.HasFlag(MiscInfoFlags.MINIDUMP_MISC1_PROCESS_ID))
            {
                AddInfoNode("ProcessId", _miscInfo.ProcessId.ToString());
            }
            else
            {
                AddInfoNode("MINIDUMP_MISC1_PROCESS_ID", "Not available");
            }

            if (_miscInfo.Flags1.HasFlag(MiscInfoFlags.MINIDUMP_MISC1_PROCESS_TIMES))
            {
                AddInfoNode("ProcessCreateTime", _miscInfo.ProcessCreateTime.ToString());
                AddInfoNode("ProcessUserTime", _miscInfo.ProcessUserTime.ToString());
                AddInfoNode("ProcessKernelTime", _miscInfo.ProcessKernelTime.ToString());
            }
            else
            {
                AddInfoNode("MINIDUMP_MISC1_PROCESS_TIMES", "Not available");
            }

            // Check what other level of information is available
            if (_miscInfo.MiscInfoLevel == MiniDumpMiscInfoLevel.MiscInfo4)
            {
                AddMiscInfo2Data((MiniDumpMiscInfo2)miscInfo);
                AddMiscInfo3Data((MiniDumpMiscInfo3)miscInfo);
                AddMiscInfo4Data((MiniDumpMiscInfo4)miscInfo);
            }
            else if (_miscInfo.MiscInfoLevel == MiniDumpMiscInfoLevel.MiscInfo3)
            {
                AddMiscInfo2Data((MiniDumpMiscInfo2)miscInfo);
                AddMiscInfo3Data((MiniDumpMiscInfo3)miscInfo);
            }
            else if (_miscInfo.MiscInfoLevel == MiniDumpMiscInfoLevel.MiscInfo2)
            {
                AddMiscInfo2Data((MiniDumpMiscInfo2)miscInfo);
            }
        }

		/// <summary>
		/// Adds the misc info2 data.
		/// </summary>
		/// <param name="miscInfo2">The misc info2.</param>
		private void AddMiscInfo2Data(MiniDumpMiscInfo2 miscInfo2)
        {
            if (miscInfo2.Flags1.HasFlag(MiscInfoFlags.MINIDUMP_MISC1_PROCESSOR_POWER_INFO))
            {
                AddInfoNode("ProcessorMaxMhz", miscInfo2.ProcessorMaxMhz.ToString());
                AddInfoNode("ProcessorCurrentMhz", miscInfo2.ProcessorCurrentMhz.ToString());
                AddInfoNode("ProcessorMhzLimit", miscInfo2.ProcessorMhzLimit.ToString());
                AddInfoNode("ProcessorMaxIdleState", miscInfo2.ProcessorMaxIdleState.ToString());
                AddInfoNode("ProcessorCurrentIdleState", miscInfo2.ProcessorCurrentIdleState.ToString());
            }
            else
            {
                AddInfoNode("MINIDUMP_MISC1_PROCESSOR_POWER_INFO", "Not available");
            }
        }

		/// <summary>
		/// Adds the misc info3 data.
		/// </summary>
		/// <param name="miscInfo3">The misc info3.</param>
		private void AddMiscInfo3Data(MiniDumpMiscInfo3 miscInfo3)
        {
            // MINIDUMP_MISC3_PROCESS_INTEGRITY isn't actually documented, so I'm assuming it covers ProcessIntegrityLevel
            if (miscInfo3.Flags1.HasFlag(MiscInfoFlags.MINIDUMP_MISC3_PROCESS_INTEGRITY))
            {
                AddInfoNode("ProcessIntegrityLevel", miscInfo3.ProcessIntegrityLevel.ToString());
            }
            else
            {
                AddInfoNode("MINIDUMP_MISC3_PROCESS_INTEGRITY", "Not available");
            }

            // MINIDUMP_MISC3_PROCESS_EXECUTE_FLAGS isn't actually documented, so I'm assuming it covers ProcessExecuteFlags
            if (miscInfo3.Flags1.HasFlag(MiscInfoFlags.MINIDUMP_MISC3_PROCESS_EXECUTE_FLAGS))
            {
                AddInfoNode("ProcessExecuteFlags", miscInfo3.ProcessExecuteFlags.ToString());
            }
            else
            {
                AddInfoNode("MINIDUMP_MISC3_PROCESS_EXECUTE_FLAGS", "Not available");
            }
            
            // MINIDUMP_MISC3_PROTECTED_PROCESS isn't actually documented, so I'm assuming it covers ProtectedProcess
            if (miscInfo3.Flags1.HasFlag(MiscInfoFlags.MINIDUMP_MISC3_PROTECTED_PROCESS))
            {
                AddInfoNode("ProtectedProcess", miscInfo3.ProtectedProcess.ToString());
            }
            else
            {
                AddInfoNode("MINIDUMP_MISC3_PROTECTED_PROCESS", "Not available");
            }

            // MINIDUMP_MISC3_TIMEZONE isn't actually documented, so I'm assuming it covers TimeZoneId & TimeZoneId
            if (miscInfo3.Flags1.HasFlag(MiscInfoFlags.MINIDUMP_MISC3_TIMEZONE))
            {
                AddInfoNode("TimeZoneId", miscInfo3.TimeZoneId.ToString());
                AddInfoNode("TimeZone.Bias", miscInfo3.TimeZone.Bias.ToString());
                AddInfoNode("TimeZone.StandardName", miscInfo3.TimeZone.StandardName);
                AddInfoNode("TimeZone.StandardDate", miscInfo3.TimeZone.StandardDate.ToString());
                AddInfoNode("TimeZone.StandardBias", miscInfo3.TimeZone.StandardBias.ToString());
                AddInfoNode("TimeZone.DaylightName", miscInfo3.TimeZone.DaylightName);
                AddInfoNode("TimeZone.DaylightDate", miscInfo3.TimeZone.DaylightDate.ToString());
                AddInfoNode("TimeZone.DaylightBias", miscInfo3.TimeZone.DaylightBias.ToString());
            }
            else
            {
                AddInfoNode("MINIDUMP_MISC3_TIMEZONE", "Not available");
            }
        }

		/// <summary>
		/// Adds the misc info4 data.
		/// </summary>
		/// <param name="miscInfo4">The misc info4.</param>
		private void AddMiscInfo4Data(MiniDumpMiscInfo4 miscInfo4)
        {
            // MINIDUMP_MISC4_BUILDSTRING isn't actually documented, so I'm assuming it covers BuildString & DbgBldStr
            if (miscInfo4.Flags1.HasFlag(MiscInfoFlags.MINIDUMP_MISC4_BUILDSTRING))
            {
                AddInfoNode("BuildString", miscInfo4.BuildString);
                AddInfoNode("DbgBldStr", miscInfo4.DbgBldStr);
            }
            else
            {
                AddInfoNode("MINIDUMP_MISC4_BUILDSTRING", "Not available");
            }
        }

		/// <summary>
		/// Adds the information node.
		/// </summary>
		/// <param name="label">The label.</param>
		/// <param name="value">The value.</param>
		private void AddInfoNode(string label, string value)
        {
            ListViewItem newItem;
            newItem = new ListViewItem(label);
            newItem.SubItems.Add(value);
            this.listView1.Items.Add(newItem);
        }
    }
}
