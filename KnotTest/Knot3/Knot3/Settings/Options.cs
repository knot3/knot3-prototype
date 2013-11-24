using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Knot3.Utilities;

namespace Knot3.Settings
{
	public class ConfigFile
	{
		private string Filename;
		private IniFile ini;

		public ConfigFile (string filename)
		{
			// load ini file
			Filename = filename;
			
			// create a new ini parser
            using (StreamWriter w = File.AppendText(Filename))
            { }
			ini = new IniFile (Filename);
		}

		public bool this [string section, string option, bool defaultValue = false] {
			get {
				return this [section, option, defaultValue ? True : False] == True ? true : false;
			}
			set {
				this [section, option, defaultValue ? True : False] = value ? True : False;
			}
		}

		public string this [string section, string option, string defaultValue = null] {
			get {
				return ini [section, option, defaultValue];
			}
			set {
				ini [section, option] = value;
			}
		}

		public static string True = "on";
		public static string False = "off";
	}

	public static class Options
	{
		private static ConfigFile _default;

		public static ConfigFile Default {
			get {
				if (_default == null)
					_default = new ConfigFile (Files.SettingsDirectory + Files.Separator + "knot3.ini");
				return _default;
			}
		}
	}

	public class OptionInfo
	{
		public string Section { get; private set; }

		public string Name { get; private set; }

		public string DefaultValue { get; private set; }

		public Action<string> OnChange { get; protected set; }

		public virtual string Value {
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

	public class DistinctOptionInfo : OptionInfo
	{
		public HashSet<string> ValidValues { get; private set; }
		
		public DistinctOptionInfo (string section, string name, string defaultValue, string[] validValues,
		                           Action<string> onChange = null, ConfigFile configFile = null)
			: base(section,name,defaultValue,onChange,configFile)
		{
			ValidValues = new HashSet<string> (validValues);
			ValidValues.Add (defaultValue);
		}

		public override string Value {
			get {
				return base.Value;
			}
			set {
				if (ValidValues.Contains (value))
					base.Value = value;
				else
					base.Value = DefaultValue;
			}
		}
	}

	public class BooleanOptionInfo : DistinctOptionInfo
	{
		public BooleanOptionInfo (string section, string name, bool defaultValue, Action<bool> onChange = null,
		                          ConfigFile configFile = null)
			: base(section, name, defaultValue?ConfigFile.True:ConfigFile.False,
			       new string[]{ConfigFile.True,ConfigFile.False}, (str)=>{}, configFile)
		{
			if (onChange != null) {
				OnChange = (str) => onChange (str == ConfigFile.True);
			}
		}

		public bool BoolValue {
			get {
				return base.Value == ConfigFile.True ? true : false;
			}
			set {
				base.Value = value ? ConfigFile.True : ConfigFile.False;
			}
		}
	}
}

