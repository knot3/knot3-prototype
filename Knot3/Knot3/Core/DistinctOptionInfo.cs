using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Knot3.Utilities;

namespace Knot3.Settings
{
	public class DistinctOptionInfo : OptionInfo
	{
		public HashSet<string> ValidValues { get; private set; }

		public DistinctOptionInfo (string section, string name, string defaultValue, string[] validValues,
		                           Action<string> onChange = null, ConfigFile configFile = null)
		: base(section, name, defaultValue, onChange, configFile)
		{
			ValidValues = new HashSet<string> (validValues);
			ValidValues.Add (defaultValue);
		}

		public override string Value
		{
			get {
				return base.Value;
			}
			set {
				if (ValidValues.Contains (value)) {
					base.Value = value;
				}
				else {
					base.Value = DefaultValue;
				}
			}
		}
	}
}
