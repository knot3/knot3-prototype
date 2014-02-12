using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Knot3.Utilities;

namespace Knot3.Settings
{
	public class OptionInfo
	{
		public string Section { get; private set; }

		public string Name { get; private set; }

		public string DefaultValue { get; private set; }

		public Action<string> OnChange { get; protected set; }

		public virtual string Value
		{
			get {
				Console.WriteLine ("OptionInfo: " + Section + "." + Name + " => " + ConfigFile [Section, Name, DefaultValue]);
				return ConfigFile [Section, Name, DefaultValue];
			}
			set {
				Console.WriteLine ("OptionInfo: " + Section + "." + Name + " <= " + value);
				ConfigFile [Section, Name, DefaultValue] = value;
				OnChange (value);
			}
		}

		private ConfigFile ConfigFile;

		public OptionInfo (string section, string name, string defaultValue, Action<string> onChange = null,
		                   ConfigFile configFile = null)
		{
			Section = section;
			Name = name;
			DefaultValue = defaultValue;
			ConfigFile = configFile != null ? configFile : Options.Default;
			OnChange = onChange != null ? onChange : (str) => {};
		}
	}
}
