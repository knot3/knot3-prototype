using System;
using System.Collections.Generic;
using System.IO;

using Knot3.Utilities;

namespace Knot3.KnotData
{
	public class KnotFileIO : IKnotIO
	{

		public Knot Load (string filename)
		{
			KnotStringIO parser = new KnotStringIO (string.Join ("\n", Files.ReadFrom (filename)));
			return new Knot (
				new KnotMetaData (parser.Name, () => parser.CountEdges, this, filename),
				parser.Edges
			);
		}

		public KnotMetaData LoadMetaData (string filename)
		{
			KnotStringIO parser = new KnotStringIO (string.Join ("\n", Files.ReadFrom (filename)));
			return new KnotMetaData (parser.Name, () => parser.CountEdges, this, filename);
		}

		public void Save (Knot knot)
		{
			KnotStringIO parser = new KnotStringIO (knot);
			Console.WriteLine ("KnotFileIO.Save(" + knot + ") = #" + parser.Content.Length);
			if (knot.MetaData.Filename == null) {
				throw new IOException ("Error! knot has no filename: " + knot);
			} else {
				File.WriteAllText (knot.MetaData.Filename, parser.Content);
			}
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
			return "KnotFileIO()";
		}
	}
}

