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
			using (StreamWriter w = File.AppendText(Filename)) {
			}
			ini = new IniFile (Filename);
		}

		public bool this [string section, string option, bool defaultValue = false]
		{
			get {
				return this [section, option, defaultValue ? True : False] == True ? true : false;
			}
			set {
				this [section, option, defaultValue ? True : False] = value ? True : False;
			}
		}

		public string this [string section, string option, string defaultValue = null]
		{
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
}
