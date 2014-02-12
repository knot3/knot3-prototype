using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using System.IO;

namespace Knot3.KnotData
{
	public class KnotStringIO
	{
		public string Name;
		private IEnumerable<string> edgeLines;

		public KnotStringIO (string content)
		{
			Content = content;
		}

		public KnotStringIO (Knot knot)
		{
			Name = knot.Name;
			try {
				edgeLines = ToLines (knot);
			}
			catch (Exception ex) {
				Console.WriteLine (ex);
			}
		}

		public string Content
		{
			get {
				return Name + "\n" + string.Join ("\n", edgeLines);
			}
			set {
				if (value.Length >= 2) {
					string[] parts = value.Split (new char[] {'\r','\n'}, StringSplitOptions.RemoveEmptyEntries);
					Name = parts [0];
					edgeLines = parts.Skip (1);
				}
				else if (value.Length == 1) {
					Name = value;
					edgeLines = new string[] {};
				}
			}
		}

		public int CountEdges
		{
			get {
				return edgeLines.Where ((l) => l.Trim ().Length > 0).Count ();
			}
		}

		public IEnumerable<Edge> Edges
		{
			get {
				Console.WriteLine ("KnotStringIO.Edges[get] = " + edgeLines.Count ());
				foreach (string line in edgeLines) {
					Edge edge = DecodeEdge (line [0]);
					edge.Color = DecodeColor (line.Substring (1, 8));
					yield return edge;
				}
			}
			set {
				Console.WriteLine ("KnotStringIO.Edges[set] = #" + value.Count ());
				try {
					edgeLines = ToLines (value);
				}
				catch (Exception ex) {
					Console.WriteLine (ex);
				}
			}
		}

		private static IEnumerable<string> ToLines (IEnumerable<Edge> edges)
		{
			foreach (Edge edge in edges) {
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
				throw new IOException ("Failed to decode Edge: '" + c + "'!");
			}
		}

		private static char EncodeEdge (Edge edge)
		{
			if (edge.Direction == Direction.Right) {
				return 'X';
			}
			else if (edge.Direction == Direction.Left) {
				return  'x';
			}
			else if (edge.Direction == Direction.Up) {
				return  'Y';
			}
			else if (edge.Direction == Direction.Down) {
				return  'y';
			}
			else if (edge.Direction == Direction.Backward) {
				return  'Z';
			}
			else if (edge.Direction == Direction.Forward) {
				return  'z';
			}
			else {
				throw new IOException ("Failed to encode Edge: '" + edge + "'!");
			}
		}

		private static String EncodeColor (Color c)
		{
			return c.R.ToString ("X2") + c.G.ToString ("X2") + c.B.ToString ("X2") + c.A.ToString ("X2");
		}

		private static Color DecodeColor (string hexString)
		{
			if (hexString.StartsWith ("#")) {
				hexString = hexString.Substring (1);
			}
			uint hex = uint.Parse (hexString, System.Globalization.NumberStyles.HexNumber);
			Color color = Color.White;
			if (hexString.Length == 8) {
				color.R = (byte)(hex >> 24);
				color.G = (byte)(hex >> 16);
				color.B = (byte)(hex >> 8);
				color.A = (byte)(hex);
			}
			else if (hexString.Length == 6) {
				color.R = (byte)(hex >> 16);
				color.G = (byte)(hex >> 8);
				color.B = (byte)(hex);
			}
			else {
				throw new IOException ("Invald hex representation of an ARGB or RGB color value.");
			}
			return color;
		}

		public override string ToString ()
		{
			return "KnotStringIO(length=" + Content.Length + ")";
		}
	}
}
