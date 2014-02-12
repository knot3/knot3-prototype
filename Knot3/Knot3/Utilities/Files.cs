using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Knot3.Utilities
{
	public static class Files
	{
		public static IEnumerable<string> ReadFrom (string file)
		{
			string line;
			using (var reader = File.OpenText(file)) {
				while ((line = reader.ReadLine()) != null) {
					yield return line;
				}
			}
		}

		public static char Separator { get { return Path.DirectorySeparatorChar; } }

		public static bool IsPath (string filepath)
		{
			return filepath.Contains (Path.DirectorySeparatorChar) || filepath.Contains (Path.AltDirectorySeparatorChar);
		}

		public static string SettingsDirectory
		{
			get {
				string directory;
				if (Mono.IsRunningOnMono ()) {
					directory = Environment.GetEnvironmentVariable ("HOME") + "/.knot3/";
				}
				else {
					directory = Environment.GetFolderPath (System.Environment.SpecialFolder.Personal) + "\\Knot3\\";
				}
				Directory.CreateDirectory (directory);
				return directory;
			}
		}

		public static string SavegameDirectory
		{
			get {
				string directory = SettingsDirectory + Separator + "Savegames";
				Directory.CreateDirectory (directory);
				return directory;
			}
		}

		private static string baseDirectory = null;

		public static string BaseDirectory
		{
			get {
				if (baseDirectory != null) {
					return baseDirectory;
				}
				else {
					string cwd = Directory.GetCurrentDirectory ();
					string[] binDirectories = new string[] {"Debug", "Release", "bin"};
					foreach (string dir in binDirectories) {
						if (cwd.EndsWith (dir)) {
							cwd = cwd.Substring (0, cwd.Length - dir.Length - 1);
						}
					}
					// Environment.CurrentDirectory = cwd;
					Console.WriteLine (cwd);
					baseDirectory = cwd;
					return cwd;
				}
			}
		}

		public static void SearchFiles (IEnumerable<string> directories, IEnumerable<string> extensions, Action<string> add)
		{
			foreach (string directory in directories) {
				SearchFiles (directory, extensions, add);
			}
		}

		public static void SearchFiles (string directory, IEnumerable<string> extensions, Action<string> add)
		{
			Directory.CreateDirectory (directory);
			var files = Directory.GetFiles (directory, "*.*", SearchOption.AllDirectories)
			            .Where (s => extensions.Any (e => s.EndsWith (e)));
			foreach (string file in files) {
				add (file);
			}
		}
	}
}
