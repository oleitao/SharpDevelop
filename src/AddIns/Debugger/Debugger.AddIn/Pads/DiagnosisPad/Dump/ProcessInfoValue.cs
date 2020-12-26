using System;
using System.Windows.Controls.DataVisualization.Charting;

namespace Debugger.AddIn.Pads.DiagnosisPad.Dump
{
	/// <summary>
	/// ProcessInfoValue class
	/// </summary>
	public class ProcessInfoValue
	{
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get; private set; }
		/// <summary>
		/// Gets the alias.
		/// </summary>
		/// <value>
		/// The alias.
		/// </value>
		public string Alias { get; private set; }
		/// <summary>
		/// Gets the name of the group.
		/// </summary>
		/// <value>
		/// The name of the group.
		/// </value>
		public string GroupName { get; private set; }
		/// <summary>
		/// Gets the format.
		/// </summary>
		/// <value>
		/// The format.
		/// </value>
		public string Format { get; }
		/// <summary>
		/// Gets the value getter.
		/// </summary>
		/// <value>
		/// The value getter.
		/// </value>
		public Func<ProcessExplorer, object> ValueGetter { get; }
		/// <summary>
		/// Gets or sets the series.
		/// </summary>
		/// <value>
		/// The series.
		/// </value>
		public AreaSeries Series { get; set; }
		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <value>
		/// The value.
		/// </value>
		public object Value { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessInfoValue"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="alias">The alias.</param>
		/// <param name="groupName">Name of the group.</param>
		/// <param name="valueGetter">The value getter.</param>
		/// <param name="format">The format.</param>
		public ProcessInfoValue(string name, string alias, string groupName, Func<ProcessExplorer, object> valueGetter, string format)
		{
			Name = name;
			Alias = alias;
			GroupName = groupName;
			ValueGetter = valueGetter;
			Format = format;
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <param name="proc">The proc.</param>
		/// <returns></returns>
		public object GetValue(ProcessExplorer proc)
		{
			try
			{
				var o = ValueGetter(proc);
				Value = o;
				var d = string.Format(Format, o);
				return d;
			}
			catch
			{
				return "Err";
			}
		}

		/// <summary>
		/// Resets this instance.
		/// </summary>
		public void Reset()
		{
			Series = null;
		}
	}
}