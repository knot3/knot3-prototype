using System;
using System.IO;

namespace TestGame1
{
	public static class Mono
	{
		public static bool IsRunningOnMono ()
		{
			return Type.GetType ("Mono.Runtime") != null;
		}

		public static string SettingsDirectory (string filename)
		{
			string directory;
			if (IsRunningOnMono ()) {
				directory = Environment.GetEnvironmentVariable ("HOME") + "/.knot3/";
			} else {
				directory = Environment.GetFolderPath (System.Environment.SpecialFolder.Personal) + "\\Knot3\\";
			}
			Directory.CreateDirectory (directory);
			return directory + filename;
		}
	}
}

