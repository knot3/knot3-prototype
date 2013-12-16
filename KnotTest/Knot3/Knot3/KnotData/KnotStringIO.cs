using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Knot3.KnotData
{
	public class KnotStringIO : IKnotIO
	{
		public string Content { get; set; }

		private IKnotIO parentIO;

		public KnotStringIO (string content)
		{
			Content = content;
			parentIO = this;
		}

		public KnotStringIO (string content, IKnotIO parentIO)
		{
			Content = content;
			this.parentIO = parentIO;
		}

		private IEnumerable<string> lines {
			get {
				return Content.Split (new char[] {'\r','\n'}, StringSplitOptions.RemoveEmptyEntries);
			}
			set {
				Content = string.Join("\n", value);
			}
		}

		public KnotMetaData MetaData {
			get {
				string name = null;
				int edgeCount = 0;
				foreach (string line in lines) {
					// first line is the name
					if (name == null)
						name = line.Trim ();
					// every non-empty line afterwards is a node coordinate
					else if (name.Trim ().Length > 0)
						++edgeCount;
				}
				// create a new info object
				return new KnotMetaData (name, edgeCount, parentIO);
			}
		}

		public string Name {
			get { return MetaData.Name; }
		}

		public void Save (Knot knot)
		{
			Console.WriteLine ("KnotStringIO.Save(" + knot + ")");
			try {
				lines = ToLines (knot);
			} catch (Exception ex) {
				Console.WriteLine (ex);
			}
		}

		public IEnumerable<Edge> Edges {
			get {
				int i = 0;
				foreach (string line in lines) {
					if (i >= 1) {
						Edge edge = DecodeEdge (line [0]);
						edge.Color = DecodeColor (line.Substring (1, 8));
						yield return edge;
					}
					++i;
				}
			}
		}

		private static IEnumerable<string> ToLines (Knot knot)
		{
			yield return knot.Name;
			foreach (Edge edge in knot) {
				yield return EncodeEdge (edge) + EncodeColor (edge.Color);
			}
		}

		private static Edge DecodeEdge (char c)
		{
			switch (c) {
			case 'X':
				return Edge.Right;
			case 'x':
				return Edge.Left;
			case 'Y':
				return Edge.Up;
			case 'y':
				return Edge.Down;
			case 'Z':
				return Edge.Backward;
			case 'z':
				return Edge.Forward;
			default:
				throw new FormatException ("Failed to decode Edge: '" + c + "'!");
			}
		}

		private static char EncodeEdge (Edge edge)
		{
			if (edge.Direction == Direction.Right)
				return 'X';
			else if (edge.Direction == Direction.Left)
				return  'x';
			else if (edge.Direction == Direction.Up)
				return  'Y';
			else if (edge.Direction == Direction.Down)
				return  'y';
			else if (edge.Direction == Direction.Backward)
				return  'Z';
			else if (edge.Direction == Direction.Forward)
				return  'z';
			else
				throw new FormatException ("Failed to encode Edge: '" + edge + "'!");
		}

		private static String EncodeColor (Color c)
		{
			return c.R.ToString ("X2") + c.G.ToString ("X2") + c.B.ToString ("X2") + c.A.ToString ("X2");
		}

		private static Color DecodeColor (string hexString)
		{
			if (hexString.StartsWith ("#"))
				hexString = hexString.Substring (1);
			uint hex = uint.Parse (hexString, System.Globalization.NumberStyles.HexNumber);
			Color color = Color.White;
			if (hexString.Length == 8) {
				color.R = (byte)(hex >> 24);
				color.G = (byte)(hex >> 16);
				color.B = (byte)(hex >> 8);
				color.A = (byte)(hex);
			} else if (hexString.Length == 6) {
				color.R = (byte)(hex >> 16);
				color.G = (byte)(hex >> 8);
				color.B = (byte)(hex);
			} else {
				throw new FormatException ("Invald hex representation of an ARGB or RGB color value.");
			}
			return color;
		}

		public override string ToString ()
		{
			return "KnotStringIO(length=" + Content.Length + ")";
		}
	}
}

