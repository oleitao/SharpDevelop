// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Input;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Workbench;
using System.Diagnostics;
using Debugger.AddIn.Pads.DiagnosisPad;
using System.Windows.Media;
using System.Windows.Shapes;
using Debugger.AddIn.Pads.DiagnosisPad.Dump;
using ByteSizeLib;

namespace ICSharpCode.SharpDevelop.Gui.Pads.DiagnosisPad
{
	/// <summary>
	/// DiagnosisPad panel class
	/// </summary>
	/// <seealso cref="ICSharpCode.SharpDevelop.Workbench.AbstractPadContent" />
	public class DiagnosisPad : AbstractPadContent
	{
		#region Variables
		/// <summary>
		/// The grid
		/// </summary>
		Grid grid;

		/// <summary>
		/// The events ListView
		/// </summary>
		ListView eventsListView;
		/// <summary>
		/// The memory usage ListView
		/// </summary>
		ListView memoryUsageListView;

		/// <summary>
		/// The tabcontrol dynamic
		/// </summary>
		TabControl tabcontrolDynamic;
		/// <summary>
		/// The tab summary
		/// </summary>
		TabItem tabSummary;
		/// <summary>
		/// The tab events
		/// </summary>
		TabItem tabEvents;
		/// <summary>
		/// The tab memory
		/// </summary>
		TabItem tabMemory;

		/// <summary>
		/// The stack panel
		/// </summary>
		StackPanel stackPanel;
		/// <summary>
		/// The expander cpu
		/// </summary>
		Expander expanderCPU;
		/// <summary>
		/// The expander memory
		/// </summary>
		Expander expanderMemory;

		/// <summary>
		/// The chart memory
		/// </summary>
		Chart chartMemory;
		/// <summary>
		/// The chart cpu
		/// </summary>
		ChartCPU chartCPU;

		/// <summary>
		/// The BTN snapshot
		/// </summary>
		Button btnSnapshot;
		/// <summary>
		/// The BTN view help
		/// </summary>
		Button btnViewHelp;
		/// <summary>
		/// The BTN delete
		/// </summary>
		Button btnDelete;

		/// <summary>
		/// The plot handle count
		/// </summary>
		List<KeyValuePair<TimeSpan, double>> plotHandleCount;
		/// <summary>
		/// The plot paged memory
		/// </summary>
		List<KeyValuePair<TimeSpan, double>> plotPagedMemory;
		/// <summary>
		/// The plot peak paged memory
		/// </summary>
		List<KeyValuePair<TimeSpan, double>> plotPeakPagedMemory;
		/// <summary>
		/// The plot peak working set
		/// </summary>
		List<KeyValuePair<TimeSpan, double>> plotPeakWorkingSet;
		/// <summary>
		/// The plot working set
		/// </summary>
		List<KeyValuePair<TimeSpan, double>> plotWorkingSet;
		/// <summary>
		/// The plot gc total memory
		/// </summary>
		List<KeyValuePair<TimeSpan, double>> plotGCTotalMemory;
		/// <summary>
		/// The plot cpu
		/// </summary>
		List<KeyValuePair<TimeSpan, double>> plotCPU;

		/// <summary>
		/// The ck gc total memory
		/// </summary>
		CheckBox ckGCTotalMemory;
		/// <summary>
		/// The ck handle count
		/// </summary>
		CheckBox ckHandleCount;
		/// <summary>
		/// The ck paged memory
		/// </summary>
		CheckBox ckPagedMemory;
		/// <summary>
		/// The ck peak paged memory
		/// </summary>
		CheckBox ckPeakPagedMemory;
		/// <summary>
		/// The ck peak working set
		/// </summary>
		CheckBox ckPeakWorkingSet;
		/// <summary>
		/// The ck working set
		/// </summary>
		CheckBox ckWorkingSet;

		/// <summary>
		/// The img gc total memory
		/// </summary>
		Canvas imgGCTotalMemory;
		/// <summary>
		/// The img handle count
		/// </summary>
		Canvas imgHandleCount;
		/// <summary>
		/// The img paged memory
		/// </summary>
		Canvas imgPagedMemory;
		/// <summary>
		/// The img peak paged memory
		/// </summary>
		Canvas imgPeakPagedMemory;
		/// <summary>
		/// The img peak working set
		/// </summary>
		Canvas imgPeakWorkingSet;
		/// <summary>
		/// The img working set
		/// </summary>
		Canvas imgWorkingSet;

		/// <summary>
		/// The resource
		/// </summary>
		CommonResources res = null;

		/// <summary>
		/// The stacked area series
		/// </summary>
		StackedAreaSeries stackedAreaSeries;
		/// <summary>
		/// The performance cpu
		/// </summary>
		PerformanceCounter performanceCPU;
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="DiagnosisPad"/> class.
		/// </summary>
		public DiagnosisPad()
		{
			res = new CommonResources();
			res.InitializeComponent();
			
			grid = new Grid();
			plotHandleCount = new List<KeyValuePair<TimeSpan, double>>();
			plotPagedMemory = new List<KeyValuePair<TimeSpan, double>>();
			plotPeakPagedMemory = new List<KeyValuePair<TimeSpan, double>>();
			plotPeakWorkingSet = new List<KeyValuePair<TimeSpan, double>>();
			plotWorkingSet = new List<KeyValuePair<TimeSpan, double>>();
			plotGCTotalMemory = new List<KeyValuePair<TimeSpan, double>>();
			plotCPU = new List<KeyValuePair<TimeSpan, double>>();

			CreateStackPanel();
			CreateChart();
			CreateTabControl();

			if (!expanderCPU.IsExpanded)
				expanderCPU.IsExpanded = true;

			if (!expanderMemory.IsExpanded)
				expanderMemory.IsExpanded = true;
		}
		#endregion

		#region Properties
		/// <summary>
		/// </summary>
		/// <inheritdoc />
		public override object Control
		{
			get { return stackPanel; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Creates the stack panel.
		/// </summary>
		private void CreateStackPanel()
		{
			stackPanel = new StackPanel();
			stackPanel.Name = "mainStack";
		}
		/// <summary>
		/// Creates the tab control.
		/// </summary>
		private void CreateTabControl()
		{
			tabcontrolDynamic = new TabControl();
			tabcontrolDynamic.Name = "DynamicTabControl";

			tabSummary = new TabItem();
			tabSummary.Name = "tabPage1";
			tabSummary.Header = StringParser.Parse("${res:AddIns.Debugger.Diagnosis.Summary}");
			tabcontrolDynamic.Items.Add(SetSummaryTabItem());

			tabEvents = new TabItem();
			tabEvents.Name = "tabPage2";
			tabEvents.Header = StringParser.Parse("${res:AddIns.Debugger.Diagnosis.Events}");
			tabcontrolDynamic.Items.Add(SetEventsTabItem());

			tabMemory = new TabItem();
			tabMemory.Name = "tabPage3";
			tabMemory.Header = StringParser.Parse("${res:AddIns.Debugger.Diagnosis.MemoryUsage}");
			tabcontrolDynamic.Items.Add(SetMemoryUsageTabItem());

			stackPanel.Children.Add(tabcontrolDynamic);
		}
		/// <summary>
		/// Sets the summary tab item.
		/// </summary>
		/// <returns></returns>
		private TabItem SetSummaryTabItem()
		{
			TabItem tab = new TabItem();
			tab.Header = StringParser.Parse("${res:AddIns.Debugger.Diagnosis.Summary}");

			StackPanel stack = new StackPanel();
			stack.Orientation = Orientation.Vertical;
			stack.Margin = new Thickness(0, 0, 0, 0);

			Label lblEvents = new Label();
			lblEvents.Content = StringParser.Parse("${res:AddIns.Debugger.Diagnosis.Events}");
			lblEvents.FontWeight = FontWeights.Bold;

			Label lblMemory = new Label();
			lblMemory.Content = StringParser.Parse("${res:AddIns.Debugger.Diagnosis.MemoryUsage}");
			lblMemory.FontWeight = FontWeights.Bold;

			Button btnEvents = new Button();
			btnEvents.HorizontalAlignment = HorizontalAlignment.Left;
			btnEvents.BorderThickness = new Thickness(0);
			btnEvents.Background = System.Windows.Media.Brushes.Transparent;
			btnEvents.Click += BtnEvents_Click;
			btnEvents.Content = StringParser.Parse("${res:AddIns.Debugger.Diagnosis.ShowEvents}");

			Button btnMemory = new Button();
			btnMemory.HorizontalAlignment = HorizontalAlignment.Left;
			btnMemory.BorderThickness = new Thickness(0);
			btnMemory.Background = System.Windows.Media.Brushes.Transparent;
			btnMemory.Click += BtnMemory_Click;
			btnMemory.Content = StringParser.Parse("${res:AddIns.Debugger.Diagnosis.TakeSnapshot}");

			stack.Children.Add(lblEvents);
			stack.Children.Add(btnEvents);
			stack.Children.Add(new Separator());
			stack.Children.Add(lblMemory);
			stack.Children.Add(btnMemory);

			tab.Content = stack;

			return tab;
		}
		/// <summary>
		/// Sets the events tab item.
		/// </summary>
		/// <returns></returns>
		private TabItem SetEventsTabItem()
		{
			TabItem tab = new TabItem();
			tab.Header = StringParser.Parse("${res:AddIns.Debugger.Diagnosis.Events}");

			StackPanel stack = new StackPanel();
			stack.Orientation = Orientation.Vertical;
			stack.Margin = new Thickness(0, 0, 0, 0);

			eventsListView = new ListView();
			eventsListView.View = (GridView)res["eventsGridView"];
			eventsListView.ContextMenu = grid.ContextMenu;
			eventsListView.SetValue(GridViewColumnAutoSize.AutoWidthProperty, "70;100%;75;75;75");
			stack.Children.Add(eventsListView);

			WindowsDebugger.RefreshingPads += RefreshEventsPad;
			RefreshEventsPad();

			tab.Content = stack;

			return tab;
		}
		/// <summary>
		/// Sets the memory usage tab item.
		/// </summary>
		/// <returns></returns>
		private TabItem SetMemoryUsageTabItem()
		{
			TabItem tab = new TabItem();
			tab.Header = "MemoryUsage";

			StackPanel stack = new StackPanel();
			stack.Orientation = Orientation.Vertical;
			stack.Margin = new Thickness(0, 0, 0, 0);

			ToolBar toolBar = new ToolBar();
			toolBar.HorizontalAlignment = HorizontalAlignment.Left;
			toolBar.Margin = new Thickness(0, 0, 0, 0);
			toolBar.VerticalAlignment = VerticalAlignment.Top;
			toolBar.Height = 25;

			btnSnapshot = new Button();
			btnSnapshot.Content = StringParser.Parse("${res:AddIns.Debugger.Diagnosis.TakeSnapshot}");
			btnSnapshot.IsEnabled = true;
			btnSnapshot.Click += BtnSnapshot_Click;
			toolBar.Items.Add(btnSnapshot);
			toolBar.Items.Add(new Separator());

			btnViewHelp = new Button();
			btnViewHelp.Content = StringParser.Parse("${res:AddIns.Debugger.Diagnosis.ViewHeap}");
			btnViewHelp.IsEnabled = true;
			btnViewHelp.Click += BtnViewHelp_Click;
			toolBar.Items.Add(btnViewHelp);
			toolBar.Items.Add(new Separator());

			btnDelete = new Button();
			btnDelete.Content = StringParser.Parse("${res:AddIns.Debugger.Diagnosis.Delete}");
			btnDelete.IsEnabled = true;
			btnDelete.Click += BtnDelete_Click;
			toolBar.Items.Add(btnDelete);
			toolBar.Items.Add(new Separator());

			stack.Children.Add(toolBar);


			memoryUsageListView = new ListView();
			memoryUsageListView.View = (GridView)res["memoryUsageGridView"];
			memoryUsageListView.ContextMenu = grid.ContextMenu;
			memoryUsageListView.MouseDoubleClick += MemoryUsageListView_MouseDoubleClick;
			memoryUsageListView.SetValue(GridViewColumnAutoSize.AutoWidthProperty, "50;50;150;150");
			stack.Children.Add(memoryUsageListView);

			WindowsDebugger.RefreshingPads += RefreshMemoryPad;
			RefreshMemoryPad();

			tab.Content = stack;

			return tab;
		}
		/// <summary>
		/// Creates the chart.
		/// </summary>
		private void CreateChart()
		{
			Style styleLegand = new Style { TargetType = typeof(Control) };
			styleLegand.Setters.Add(new Setter(Legend.WidthProperty, 0d));
			styleLegand.Setters.Add(new Setter(Legend.HeightProperty, 0d));
			styleLegand.Setters.Add(new Setter(Legend.VisibilityProperty, Visibility.Collapsed));
			styleLegand.Setters.Add(new Setter(Legend.HeightProperty, 0d));


			//Memory chart
			expanderMemory = new Expander();
			expanderMemory.Margin = new Thickness(0, 0, 0, 0);
			expanderMemory.Collapsed += ExpMemory_Collapsed;
			expanderMemory.Expanded += ExpMemory_Expanded;
			expanderMemory.Header = StringParser.Parse("${res:AddIns.Debugger.Diagnosis.MemoryUsage}");
			expanderMemory.Visibility = Visibility.Visible;

			StackPanel stackMemory = new StackPanel();

			WrapPanel stackmemoryFilters = new WrapPanel();
			stackmemoryFilters.Orientation = Orientation.Horizontal;

			ckHandleCount = new CheckBox();
			ckHandleCount.IsChecked = false;
			ckHandleCount.Width = 150;
			ckHandleCount.Checked += CkHandleCount_Checked;
			ckHandleCount.Name = "ckHandleCount";
			ckHandleCount.Content = StringParser.Parse("${res:AddIns.Debugger.Diagnosis.HandleCount}");
			imgHandleCount = new Canvas();
			imgHandleCount.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255,0,0));
			imgHandleCount.Height = imgHandleCount.Width = 15;
			imgHandleCount.HorizontalAlignment = HorizontalAlignment.Stretch;

			StackPanel stackHandleCountPanel = new StackPanel();
			stackHandleCountPanel.Orientation = Orientation.Horizontal;
			stackHandleCountPanel.Children.Add(imgHandleCount);
			stackHandleCountPanel.Children.Add(ckHandleCount);
			stackmemoryFilters.Children.Add(stackHandleCountPanel);


			ckPagedMemory = new CheckBox();
			ckPagedMemory.Name = "ckPagedMemory";
			ckPagedMemory.IsChecked = false;
			ckPagedMemory.Checked += CkPagedMemory_Checked;
			ckPagedMemory.Width = 150;
			ckPagedMemory.Content = StringParser.Parse("${res:AddIns.Debugger.Diagnosis.PagedMemory}");
			imgPagedMemory = new Canvas();
			imgPagedMemory.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 0));
			imgPagedMemory.Height = imgPagedMemory.Width = 15;
			imgPagedMemory.HorizontalAlignment = HorizontalAlignment.Stretch;

			StackPanel stackPagedMemoryPanel = new StackPanel();
			stackPagedMemoryPanel.Orientation = Orientation.Horizontal;
			stackPagedMemoryPanel.Children.Add(imgPagedMemory);
			stackPagedMemoryPanel.Children.Add(ckPagedMemory);
			stackmemoryFilters.Children.Add(stackPagedMemoryPanel);


			ckPeakPagedMemory = new CheckBox();
			ckPeakPagedMemory.IsChecked = false;
			ckPeakPagedMemory.Checked += CkPeakPagedMemory_Checked;
			ckPeakPagedMemory.Width = 150;
			ckPeakPagedMemory.Name = "ckPeakPagedMemory";
			ckPeakPagedMemory.Content = StringParser.Parse("${res:AddIns.Debugger.Diagnosis.PeakPagedMemory}");
			imgPeakPagedMemory = new Canvas();
			imgPeakPagedMemory.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 255));
			imgPeakPagedMemory.Height = imgPeakPagedMemory.Width = 15;
			imgPeakPagedMemory.HorizontalAlignment = HorizontalAlignment.Stretch;

			StackPanel stackPeakPagedMemoryPanel = new StackPanel();
			stackPeakPagedMemoryPanel.Orientation = Orientation.Horizontal;
			stackPeakPagedMemoryPanel.Children.Add(imgPeakPagedMemory);
			stackPeakPagedMemoryPanel.Children.Add(ckPeakPagedMemory);
			stackmemoryFilters.Children.Add(stackPeakPagedMemoryPanel);


			ckGCTotalMemory = new CheckBox();
			ckGCTotalMemory.IsChecked = true;
			ckGCTotalMemory.Checked += CkGCTotalMemory_Checked;
			ckGCTotalMemory.Width = 150;
			ckGCTotalMemory.Name = "ckGCTotalMemory";
			ckGCTotalMemory.Content = StringParser.Parse("${res:AddIns.Debugger.Diagnosis.TotalMemory}");
			imgGCTotalMemory = new Canvas();
			imgGCTotalMemory.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 255));
			imgGCTotalMemory.Height = imgGCTotalMemory.Width = 15;
			imgGCTotalMemory.HorizontalAlignment = HorizontalAlignment.Stretch;

			StackPanel stackTotalMemoryPanel = new StackPanel();
			stackTotalMemoryPanel.Orientation = Orientation.Horizontal;
			stackTotalMemoryPanel.Children.Add(imgGCTotalMemory);
			stackTotalMemoryPanel.Children.Add(ckGCTotalMemory);
			stackmemoryFilters.Children.Add(stackTotalMemoryPanel);


			ckPeakWorkingSet = new CheckBox();
			ckPeakWorkingSet.IsChecked = false;
			ckPeakWorkingSet.Checked += CkPeakWorkingSet_Checked;
			ckPeakWorkingSet.Width = 150;
			ckPeakWorkingSet.Name = "ckPeakWorkingSet";
			ckPeakWorkingSet.Content = StringParser.Parse("${res:AddIns.Debugger.Diagnosis.PeakWorkingSet}");
			imgPeakWorkingSet = new Canvas();
			imgPeakWorkingSet.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 255, 0));
			imgPeakWorkingSet.Height = imgPeakWorkingSet.Width = 15;
			imgPeakWorkingSet.HorizontalAlignment = HorizontalAlignment.Stretch;

			StackPanel stackPeakWorkingSetPanel = new StackPanel();
			stackPeakWorkingSetPanel.Orientation = Orientation.Horizontal;
			stackPeakWorkingSetPanel.Children.Add(imgPeakWorkingSet);
			stackPeakWorkingSetPanel.Children.Add(ckPeakWorkingSet);
			stackmemoryFilters.Children.Add(stackPeakWorkingSetPanel);


			ckWorkingSet = new CheckBox();
			ckWorkingSet.IsChecked = false;
			ckWorkingSet.Checked += CkWorkingSet_Checked;
			ckWorkingSet.Width = 145;
			ckWorkingSet.Name = "ckWorkingSet";
			ckWorkingSet.Content = StringParser.Parse("${res:AddIns.Debugger.Diagnosis.WorkingSet}");
			imgWorkingSet = new Canvas();
			imgWorkingSet.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(125, 254, 0));
			imgWorkingSet.Height = imgWorkingSet.Width = 15;
			imgWorkingSet.HorizontalAlignment = HorizontalAlignment.Stretch;

			StackPanel stackWorkingSetPanel = new StackPanel();
			stackWorkingSetPanel.Orientation = Orientation.Horizontal;
			stackWorkingSetPanel.Children.Add(imgWorkingSet);
			stackWorkingSetPanel.Children.Add(ckWorkingSet);
			stackmemoryFilters.Children.Add(stackWorkingSetPanel);

			stackMemory.Children.Add(stackmemoryFilters);


			chartMemory = new Chart();
			chartMemory.Name = "chartMemory";
			chartMemory.VerticalAlignment = VerticalAlignment.Top;
			chartMemory.Margin = new Thickness(0);
			chartMemory.Padding = new Thickness(0);
			chartMemory.Height = (System.Windows.SystemParameters.WorkArea.Height / 5);
			chartMemory.LegendStyle = styleLegand;

			DateTimeAxis dta = new DateTimeAxis();
			dta.IntervalType = DateTimeIntervalType.Milliseconds;
			dta.Orientation = AxisOrientation.X;
			dta.Minimum = DateTime.Now;

			LinearAxis yaxisMemory = new LinearAxis();
			yaxisMemory.Title = StringParser.Parse("${res:AddIns.Debugger.Diagnosis.MemoryMB}");
			yaxisMemory.Orientation = AxisOrientation.Y;

			chartMemory.Axes.Add(yaxisMemory);

			stackMemory.Children.Add(chartMemory);

			WindowsDebugger.RefreshingPads += RefreshMemoryPad;
			RefreshMemoryPad();

			expanderMemory.Content = stackMemory;


			//CPU chart
			expanderCPU = new Expander();
			expanderCPU.Collapsed += ExpCPU_Collapsed;
			expanderCPU.Expanded += ExpCPU_Expanded;
			expanderCPU.Header = StringParser.Parse("${res:AddIns.Debugger.Diagnosis.CPUUsage}");
			expanderCPU.Visibility = Visibility.Visible;

			StackPanel stackCPU = new StackPanel();

			chartCPU = new ChartCPU();
			chartCPU.Height = (System.Windows.SystemParameters.WorkArea.Height / 5);

			LinearAxis yaxisCPU = new LinearAxis();
			yaxisCPU.Title = StringParser.Parse("${res:AddIns.Debugger.Diagnosis.CPUPercentage}");
			yaxisCPU.Minimum = 0;
			yaxisCPU.Maximum = 100;
			yaxisCPU.Orientation = AxisOrientation.Y;
			chartCPU.cpuChart.Axes.Add(yaxisCPU);

			stackCPU.Children.Add(chartCPU);

			WindowsDebugger.RefreshingPads += RefreshCPUPad;
			RefreshCPUPad();

			expanderCPU.Content = stackCPU;

			
			stackPanel.Children.Add(expanderMemory);
			stackPanel.Children.Add(expanderCPU);
		}
		/// <summary>
		/// Refreshes the events pad.
		/// </summary>
		void RefreshEventsPad()
		{
			Debugger.Process process = WindowsDebugger.CurrentProcess;

			if (process == null || process.IsRunning)
			{
				eventsListView.ItemsSource = null;
			}
			else
			{
				eventsListView.ItemsSource = WindowsDebugger.CurrentEventTracker;
			}
		}
		/// <summary>
		/// Refreshes the cpu pad.
		/// </summary>
		void RefreshCPUPad()
		{
			Debugger.Process process = WindowsDebugger.CurrentProcess;

			if (process == null || process.IsRunning)
			{
				chartCPU.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate () { chartCPU.ChartLineSeriesCPU.ItemsSource = Enumerable.Reverse(plotCPU).Take(10).Reverse().ToList(); }));
			} 
			else {

				if (performanceCPU == null)
					performanceCPU = ProcessCpuCounter.GetPerfCounterForProcessId(Convert.ToInt32(WindowsDebugger.CurrentProcess.Id));

				//System.Threading.Thread.Sleep(100);

				var cpu = performanceCPU.NextValue() / (float)Environment.ProcessorCount;
				plotCPU.Add(new KeyValuePair<TimeSpan, double>(DateTime.Now.TimeOfDay, cpu));
				chartCPU.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate () { chartCPU.ChartLineSeriesCPU.ItemsSource = Enumerable.Reverse(plotCPU).Take(10).Reverse().ToList(); }));
			}

			if (process == null || process.HasExited)
			{
				plotCPU.Clear();
				chartCPU.ChartLineSeriesCPU.ItemsSource = null;
			}
		}
		/// <summary>
		/// Refreshes the memory pad.
		/// </summary>
		void RefreshMemoryPad()
		{
			Debugger.Process process = WindowsDebugger.CurrentProcess;
			
			if (process == null || process.IsRunning)
			{
				chartMemory.DataContext = null;
			}
			else
			{
				ProcessExplorer processExplorer = new ProcessExplorer(System.Diagnostics.Process.GetProcessById(Convert.ToInt32(WindowsDebugger.CurrentProcess.Id)));
				processExplorer.Process.Refresh();

				chartMemory.Series.Clear();

				stackedAreaSeries = new StackedAreaSeries();				

				if (ckWorkingSet.IsChecked.Equals(true))
				{
					SolidColorBrush solidBrushWorkingSet = new SolidColorBrush(Color.FromRgb(0,255,0));

					Style dataShapeStyleWorkingSet = new Style { TargetType = typeof(Shape) };
					dataShapeStyleWorkingSet.Setters.Add(new Setter(Shape.FillProperty, solidBrushWorkingSet));
					dataShapeStyleWorkingSet.Setters.Add(new Setter(Shape.StrokeProperty, solidBrushWorkingSet));

					plotWorkingSet.Add(new KeyValuePair<TimeSpan, double>(DateTime.Now.TimeOfDay, Convert.ToDouble(processExplorer.WorkingSet) / 1024 / 1024));

					SeriesDefinition workingSetLine = new SeriesDefinition();					
					workingSetLine.Title = ckWorkingSet.Content;
					workingSetLine.DependentValuePath = "Value";
					workingSetLine.IndependentValuePath = "Key";
					workingSetLine.DataShapeStyle = dataShapeStyleWorkingSet;
					workingSetLine.ItemsSource = Enumerable.Reverse(plotWorkingSet).Take(10).Reverse().ToList();

					stackedAreaSeries.SeriesDefinitions.Add(workingSetLine);
					chartMemory.Series.Add(stackedAreaSeries);
				}

				if (ckPeakWorkingSet.IsChecked.Equals(true))
				{
					SolidColorBrush solidPeakWorkingSet = new SolidColorBrush(Color.FromRgb(42, 138, 187));

					Style dataShapeStylePeakWorkingSet = new Style { TargetType = typeof(Shape) };
					dataShapeStylePeakWorkingSet.Setters.Add(new Setter(Shape.FillProperty, solidPeakWorkingSet));
					dataShapeStylePeakWorkingSet.Setters.Add(new Setter(Shape.StrokeProperty, solidPeakWorkingSet));

					plotPeakWorkingSet.Add(new KeyValuePair<TimeSpan, double>(DateTime.Now.TimeOfDay, Convert.ToDouble(processExplorer.PeakWorkingSet) / 1024 / 1024));

					SeriesDefinition peakWorkingSet = new SeriesDefinition();
					peakWorkingSet.Title = ckWorkingSet.Content;
					peakWorkingSet.DependentValuePath = "Value";
					peakWorkingSet.IndependentValuePath = "Key";
					peakWorkingSet.DataShapeStyle = dataShapeStylePeakWorkingSet;
					peakWorkingSet.ItemsSource = Enumerable.Reverse(plotPeakWorkingSet).Take(10).Reverse().ToList();

					stackedAreaSeries.SeriesDefinitions.Add(peakWorkingSet);
					chartMemory.Series.Add(stackedAreaSeries);
				}

				if (ckPeakPagedMemory.IsChecked.Equals(true))
				{
					SolidColorBrush solidPeakPagedMemory = new SolidColorBrush(Color.FromRgb(211, 138, 187));

					Style dataShapeStylePeakPagedMemory = new Style { TargetType = typeof(Shape) };
					dataShapeStylePeakPagedMemory.Setters.Add(new Setter(Shape.FillProperty, solidPeakPagedMemory));
					dataShapeStylePeakPagedMemory.Setters.Add(new Setter(Shape.StrokeProperty, solidPeakPagedMemory));

					plotPeakPagedMemory.Add(new KeyValuePair<TimeSpan, double>(DateTime.Now.TimeOfDay, Convert.ToDouble(processExplorer.PeakPagedMemory) / 1024 / 1024));

					SeriesDefinition peakPagedMemoryLine = new SeriesDefinition();
					peakPagedMemoryLine.Title = ckWorkingSet.Content;
					peakPagedMemoryLine.DependentValuePath = "Value";
					peakPagedMemoryLine.IndependentValuePath = "Key";
					peakPagedMemoryLine.DataShapeStyle = dataShapeStylePeakPagedMemory;
					peakPagedMemoryLine.ItemsSource = Enumerable.Reverse(plotPeakPagedMemory).Take(10).Reverse().ToList();

					stackedAreaSeries.SeriesDefinitions.Add(peakPagedMemoryLine);
					chartMemory.Series.Add(stackedAreaSeries);
				}
				
				if (ckGCTotalMemory.IsChecked.Equals(true))
				{
					SolidColorBrush solidGCTotalMemory = new SolidColorBrush(Color.FromRgb(0, 3, 253));
					
					Style dataShapeStyleGCTotalMemory = new Style { TargetType = typeof(Shape) };
					dataShapeStyleGCTotalMemory.Setters.Add(new Setter(Shape.FillProperty, solidGCTotalMemory));
					dataShapeStyleGCTotalMemory.Setters.Add(new Setter(Shape.StrokeProperty, solidGCTotalMemory));

					plotGCTotalMemory.Add(new KeyValuePair<TimeSpan, double>(DateTime.Now.TimeOfDay, Convert.ToDouble(processExplorer.PrivateMemory) / 1024 / 1024));

					SeriesDefinition gctotalMemoryLine = new SeriesDefinition();
					gctotalMemoryLine.Title = ckWorkingSet.Content;
					gctotalMemoryLine.DependentValuePath = "Value";
					gctotalMemoryLine.IndependentValuePath = "Key";
					gctotalMemoryLine.DataShapeStyle = dataShapeStyleGCTotalMemory;
					gctotalMemoryLine.ItemsSource = Enumerable.Reverse(plotGCTotalMemory).Take(10).Reverse().ToList();

					stackedAreaSeries.SeriesDefinitions.Add(gctotalMemoryLine);
					chartMemory.Series.Add(stackedAreaSeries);
				}

				if (ckPagedMemory.IsChecked.Equals(true))
				{
					SolidColorBrush solidPagedMemory = new SolidColorBrush(Color.FromRgb(216, 216, 9));

					Style dataShapeStylePagedMemory = new Style { TargetType = typeof(Shape) };
					dataShapeStylePagedMemory.Setters.Add(new Setter(Shape.FillProperty, solidPagedMemory));
					dataShapeStylePagedMemory.Setters.Add(new Setter(Shape.StrokeProperty, solidPagedMemory));

					plotPagedMemory.Add(new KeyValuePair<TimeSpan, double>(DateTime.Now.TimeOfDay, Convert.ToDouble(processExplorer.PagedMemory) / 1024 / 1024));

					SeriesDefinition pagedMemoryLine = new SeriesDefinition();
					pagedMemoryLine.Title = ckWorkingSet.Content;
					pagedMemoryLine.DependentValuePath = "Value";
					pagedMemoryLine.IndependentValuePath = "Key";
					pagedMemoryLine.DataShapeStyle = dataShapeStylePagedMemory;
					pagedMemoryLine.ItemsSource = Enumerable.Reverse(plotPagedMemory).Take(10).Reverse().ToList();

					stackedAreaSeries.SeriesDefinitions.Add(pagedMemoryLine);
					chartMemory.Series.Add(stackedAreaSeries);
				}

				if (ckHandleCount.IsChecked.Equals(true))
				{
					SolidColorBrush solidHandleCount = new SolidColorBrush(Color.FromRgb(252, 0, 0));

					Style dataShapeStyleHandleCount = new Style { TargetType = typeof(Shape) };
					dataShapeStyleHandleCount.Setters.Add(new Setter(Shape.FillProperty, solidHandleCount));
					dataShapeStyleHandleCount.Setters.Add(new Setter(Shape.StrokeProperty, solidHandleCount));

					plotHandleCount.Add(new KeyValuePair<TimeSpan, double>(DateTime.Now.TimeOfDay, Convert.ToDouble(processExplorer.HandleCount) / 1024 / 1024));

					SeriesDefinition handleCountLine = new SeriesDefinition();
					handleCountLine.Title = ckWorkingSet.Content;
					handleCountLine.DependentValuePath = "Value";
					handleCountLine.IndependentValuePath = "Key";
					handleCountLine.DataShapeStyle = dataShapeStyleHandleCount;
					handleCountLine.ItemsSource = Enumerable.Reverse(plotHandleCount).Take(10).Reverse().ToList();

					stackedAreaSeries.SeriesDefinitions.Add(handleCountLine);
					chartMemory.Series.Add(stackedAreaSeries);
				}
			}

			if (process == null || process.HasExited)
			{
				plotWorkingSet.Clear();
				plotPeakWorkingSet.Clear();
				plotPeakPagedMemory.Clear();
				plotGCTotalMemory.Clear();
				plotPagedMemory.Clear();
				plotHandleCount.Clear();

				chartMemory.DataContext = null;
				chartMemory.Series.Clear();
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// Handles the Click event of the BtnMemory control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
		private void BtnMemory_Click(object sender, RoutedEventArgs e)
		{
			tabcontrolDynamic.SelectedIndex = 2;
		}
		/// <summary>
		/// Handles the Click event of the BtnEvents control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
		private void BtnEvents_Click(object sender, RoutedEventArgs e)
		{
			tabcontrolDynamic.SelectedIndex = 1;
		}
		/// <summary>
		/// Handles the Checked event of the CkWorkingSet control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
		private void CkWorkingSet_Checked(object sender, RoutedEventArgs e)
		{
			if (ckWorkingSet.IsChecked.Equals(true))
			{
				ckPeakWorkingSet.IsChecked = ckGCTotalMemory.IsChecked = ckPeakPagedMemory.IsChecked = ckPagedMemory.IsChecked = ckHandleCount.IsChecked = false;
			}
		}
		/// <summary>
		/// Handles the Checked event of the CkPeakWorkingSet control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
		private void CkPeakWorkingSet_Checked(object sender, RoutedEventArgs e)
		{
			if (ckPeakWorkingSet.IsChecked.Equals(true))
			{
				ckWorkingSet.IsChecked = ckGCTotalMemory.IsChecked = ckPeakPagedMemory.IsChecked = ckPagedMemory.IsChecked = ckHandleCount.IsChecked = false;
			}
		}
		/// <summary>
		/// Handles the Checked event of the CkGCTotalMemory control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
		private void CkGCTotalMemory_Checked(object sender, RoutedEventArgs e)
		{
			if (ckGCTotalMemory.IsChecked.Equals(true))
			{
				ckWorkingSet.IsChecked = ckPeakWorkingSet.IsChecked = ckPeakPagedMemory.IsChecked = ckPagedMemory.IsChecked = ckHandleCount.IsChecked = false;
			}
		}
		/// <summary>
		/// Handles the Checked event of the CkPeakPagedMemory control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
		private void CkPeakPagedMemory_Checked(object sender, RoutedEventArgs e)
		{
			if (ckPeakPagedMemory.IsChecked.Equals(true))
			{
				ckWorkingSet.IsChecked = ckPeakWorkingSet.IsChecked = ckGCTotalMemory.IsChecked = ckPagedMemory.IsChecked = ckHandleCount.IsChecked = false;
			}
		}
		/// <summary>
		/// Handles the Checked event of the CkPagedMemory control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
		private void CkPagedMemory_Checked(object sender, RoutedEventArgs e)
		{
			if (ckPagedMemory.IsChecked.Equals(true))
			{
				ckWorkingSet.IsChecked = ckPeakWorkingSet.IsChecked = ckGCTotalMemory.IsChecked = ckPeakPagedMemory.IsChecked = ckHandleCount.IsChecked = false;
			}
		}
		/// <summary>
		/// Handles the Checked event of the CkHandleCount control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
		private void CkHandleCount_Checked(object sender, RoutedEventArgs e)
		{
			if (ckHandleCount.IsChecked.Equals(true))
			{
				ckWorkingSet.IsChecked = ckPeakWorkingSet.IsChecked = ckGCTotalMemory.IsChecked = ckPeakPagedMemory.IsChecked = ckPagedMemory.IsChecked = false;
			}			
		}
		/// <summary>
		/// Handles the Click event of the BtnDelete control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
		private void BtnDelete_Click(object sender, RoutedEventArgs e)
		{
			if (WindowsDebugger.CurrentProcess != null)
			{
				if (WindowsDebugger.CurrentMemorySnap.Count > 0)
				{
					try
					{
						foreach (MemorySnap item in WindowsDebugger.CurrentMemorySnap)
						{
							if (item.Dump.Equals((memoryUsageListView.SelectedItem as MemorySnap).Dump))
							{
								string tempFolder = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"Pads\DiagnosisPad\Dump\" + @"temp\");
								if (!File.Exists(tempFolder))
								{
									try
									{
										WindowsDebugger.CurrentMemorySnap.Remove((memoryUsageListView.SelectedItem as MemorySnap));
										memoryUsageListView.ItemsSource = WindowsDebugger.CurrentMemorySnap;
										memoryUsageListView.Items.Refresh();
									}
									catch
									{
									
									}
								}
							}
						}
					}
					catch
					{ 
					
					}
				}
			}
			else
			{
				MessageService.ShowError(StringParser.Parse("${res:AddIns.Debugger.Diagnosis.DeleteSnapshot}"));
			}
		}
		/// <summary>
		/// Handles the Click event of the BtnViewHelp control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
		private void BtnViewHelp_Click(object sender, RoutedEventArgs e)
		{
			if (WindowsDebugger.CurrentProcess != null)
			{
				if (WindowsDebugger.CurrentMemorySnap.Count > 0)
				{
					DumpCreator dump = (memoryUsageListView.SelectedItem as MemorySnap).Dump;
					new Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.ShowSnapshotPageCommand(dump).Run();
				}
			}
			else
			{
				MessageService.ShowError(StringParser.Parse("${res:AddIns.Debugger.Diagnosis.DeleteSnapshot}"));
			}
		}
		/// <summary>
		/// Handles the Click event of the BtnSnapshot control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
		private void BtnSnapshot_Click(object sender, RoutedEventArgs e)
		{
			if (WindowsDebugger.CurrentProcess != null)
			{
				if (WindowsDebugger.CurrentProcess.IsPaused)
				{
					//snapshots
					EventsTracker tracker = WindowsDebugger.CurrentEventTracker.LastOrDefault();							
					
					string dumpFile;
					int nMemoryRange = 0;
					ulong nMemorySize = 0;

					DumpCreator dumpCreator = new DumpCreator(false, true);
					bool isDumped = dumpCreator.Dump(WindowsDebugger.CurrentProcess.Id.ToString(), out dumpFile, out nMemoryRange, out nMemorySize);
					
					if (isDumped)
					{
						WindowsDebugger.CurrentMemorySnap.Add(new MemorySnap(WindowsDebugger.CurrentMemorySnap.Count + 1, tracker.Time, nMemoryRange.ToString(), string.Format("{0} {1}", ByteSize.FromBytes(nMemorySize).KiloBytes, "KB"), dumpCreator));
						memoryUsageListView.ItemsSource = WindowsDebugger.CurrentMemorySnap;
						memoryUsageListView.Items.Refresh();
					}
				}
			}
			else
			{
				MessageService.ShowError(StringParser.Parse("${res:AddIns.Debugger.Diagnosis.DeleteSnapshot}"));
			}
		}
		/// <summary>
		/// Handles the Expanded event of the ExpCPU control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
		private void ExpCPU_Expanded(object sender, RoutedEventArgs e)
		{
			if (stackPanel != null)
				expanderCPU.IsExpanded = true;
		}
		/// <summary>
		/// Handles the Expanded event of the ExpMemory control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
		private void ExpMemory_Expanded(object sender, RoutedEventArgs e)
		{
			if (stackPanel != null)
				expanderMemory.IsExpanded = true;
		}
		/// <summary>
		/// Handles the Collapsed event of the ExpCPU control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
		private void ExpCPU_Collapsed(object sender, RoutedEventArgs e)
		{
			if (stackPanel != null)
				expanderCPU.IsExpanded = false;
		}
		/// <summary>
		/// Handles the Collapsed event of the ExpMemory control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
		private void ExpMemory_Collapsed(object sender, RoutedEventArgs e)
		{
			if (stackPanel != null)
				expanderMemory.IsExpanded = false;
		}
		/// <summary>
		/// Handles the MouseDoubleClick event of the MemoryUsageListView control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
		private void MemoryUsageListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (WindowsDebugger.CurrentProcess != null)
			{
				if (WindowsDebugger.CurrentMemorySnap.Count > 0)
				{
					DumpCreator dump = (memoryUsageListView.SelectedItem as MemorySnap).Dump;
					new Debugger.AddIn.Pads.DiagnosisPad.Dump.Explorer.ShowSnapshotPageCommand(dump).Run();
				}
			}
			else
			{
				MessageService.ShowError(StringParser.Parse("${res:AddIns.Debugger.Diagnosis.DeleteSnapshot}"));
			}
		}
		#endregion
	}
}
