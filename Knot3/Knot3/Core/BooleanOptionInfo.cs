using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Knot3.Utilities;

namespace Knot3.Settings
{
	public class BooleanOptionInfo : DistinctOptionInfo
	{
		public new static string[] ValidValues = new string[] {
			ConfigFile.True,
			ConfigFile.False
		};

		public BooleanOptionInfo (string section, string name, bool defaultValue, Action<bool> onChange = null,
		                          ConfigFile configFile = null)
		: base(section, name, defaultValue?ConfigFile.True:ConfigFile.False, ValidValues, null, configFile)
		{
			if (onChange != null) {
				OnChange = (str) => onChange (str == ConfigFile.True);
			}
		}

		public bool BoolValue
		{
			get {
				return base.Value == ConfigFile.True ? true : false;
			}
			set {
				base.Value = value ? ConfigFile.True : ConfigFile.False;
			}
		}
	}
}
