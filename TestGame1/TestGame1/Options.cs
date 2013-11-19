using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace TestGame1
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
				;
			ini = new IniFile (Filename);
		}

		public void Save ()
		{
			// save a new ini file
			ini.UpdateFile ();
		}

		public string this [string section, string option, string defaultValue = null] {
			get {
				string value = ini.ReadString (section, option);
				if (value.Length == 0) {
					ini.WriteString (section, option, defaultValue);
					value = defaultValue;
					Save ();
				}
				return value;
			}
			set {
				ini.WriteString (section, option, value);
				Save ();
			}
		}
	}

	public static class Options
	{
		private static ConfigFile _default;

		public static ConfigFile Default {
			get {
				if (_default == null)
					_default = new ConfigFile (Mono.SettingsDirectory ("knot3.ini"));
				return _default;
			}
		}
	}

	public class OptionInfo
	{
		public string Section { get; private set; }

		public string Name { get; private set; }

		public string DefaultValue { get; private set; }

		public virtual string Value {
			get {
				Console.WriteLine("OptionInfo: "+Section+"."+Name+" => "+ConfigFile [Section, Name, DefaultValue]);
				return ConfigFile [Section, Name, DefaultValue];
			}
			set {
				Console.WriteLine("OptionInfo: "+Section+"."+Name+" <= "+value);
				ConfigFile [Section, Name] = value;
			}
		}

		private ConfigFile ConfigFile;

		public OptionInfo (string section, string name, string defaultValue, ConfigFile configFile = null)
		{
			Section = section;
			Name = name;
			DefaultValue = defaultValue;
			ConfigFile = configFile != null ? configFile : Options.Default;
		}
	}

	public class DistinctOptionInfo : OptionInfo
	{
		public HashSet<string> ValidValues { get; private set; }
		
		public DistinctOptionInfo (string section, string name, string defaultValue, string[] validValues,
		    		ConfigFile configFile = null)
			: base(section,name,defaultValue,configFile)
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
		private static string True = "on";
		private static string False = "off";
		
		public BooleanOptionInfo (string section, string name, bool defaultValue, ConfigFile configFile = null)
			: base(section,name,defaultValue?True:False,new string[]{True,False},configFile)
		{
		}

		public bool BoolValue {
			get {
				return base.Value == True ? true : false;
			}
			set {
				base.Value = value ? True : False;
			}
		}
	}
}

