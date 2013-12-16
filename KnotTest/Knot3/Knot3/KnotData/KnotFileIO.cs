using System;
using System.Collections.Generic;
using System.IO;

using Knot3.Utilities;

namespace Knot3.KnotData
{
	public class KnotFileIO : FileIO, IKnotIO
	{
		private KnotStringIO parser;

		public KnotFileIO (Knot knot)
		{
			Filename = ConvertToFilename (knot.Name);
			parser = new KnotStringIO (knot);
		}

		public KnotFileIO (string filename)
		{
			Filename = filename;
			string content = string.Join ("\n", Files.ReadFrom (filename));
			parser = new KnotStringIO (content);
		}

		public IEnumerable<Edge> Edges {
			get { return parser.Edges; }
		}

		public int CountEdges {
			get { return parser.CountEdges; }
		}

		public string Name {
			get { return parser.Name; }
		}

		public string Hash {
			get { return parser.Hash; }
		}

		public void Save (Knot knot)
		{
			Console.WriteLine ("KnotFileIO.Save(" + knot + ")");
			if (parser.Name != knot.Name) {
				Filename = ConvertToFilename (knot.Name);
			}
			parser.Save (knot);
			File.WriteAllText (Filename, parser.Content);
		}

		public override string ConvertToFilename (string humanReadableName)
		{
			return Files.SavegameDirectory + Files.Separator + base.ConvertToFilename (humanReadableName) + ".knot";
		}

		public static string[] FileExtensions {
			get {
				return new string[] {
					".knot",
					".knt"
				};
			}
		}

		public override string ToString ()
		{
			return "KnotFileIO(filename=" + Filename + ")";
		}
	}
}

