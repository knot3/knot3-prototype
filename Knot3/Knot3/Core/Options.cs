using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Knot3.Utilities;

namespace Knot3.Settings
{
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
}

