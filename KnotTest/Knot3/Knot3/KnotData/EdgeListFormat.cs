using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using Knot3.Utilities;

namespace Knot3.KnotData
{
	/// <summary>
	/// Implementiert ein Austauschformat x,y,z;r,g,b auf Basis der Kanten-Richtungen.
	/// </summary>
	public class EdgeListFormat : IKnotFormat
	{
		// savegame files
		public string[] FileExtensions { get { return new string[] {".knot", ".knt"}; } }

		public EdgeListFormat ()
		{
		}

		/// <summary>
		/// Loads knot info.
		/// </summary>
		/// <returns>
		/// The knot info.
		/// </returns>
		/// <param name='filename'>
		/// Filename.
		/// </param>
		public KnotInfo LoadInfo (string filename)
		{
			string name = null;
			int edgeCount = 0;
			foreach (string line in Files.ReadFrom (filename)) {
				// first line is the name
				if (name == null)
					name = line.Trim ();
				// every non-empty line afterwards is a node coordinate
				else if (name.Trim ().Length > 0)
					++edgeCount;
			}
			// create a new info object
			return new KnotInfo {
				Filename = filename,
				Name = name,
				EdgeCount = () => edgeCount,
				IsValid = name != null && name.Length > 0 && edgeCount >= 2
			};
		}

		/// <summary>
		/// Loads a knot from a file.
		/// </summary>
		/// <returns>
		/// The knot.
		/// </returns>
		/// <param name='filename'>
		/// Filename.
		/// </param>
		public Knot LoadKnot (string filename)
		{
			// load knot info
			KnotInfo info = LoadInfo (filename);
			if (info.IsValid) {
				// create a knot object
				Knot knot = new Knot (info, this);
				// read lines
				List<string> lines = new List<string> (Files.ReadFrom (filename));
				// remove the line containing the name of the knot
				lines.RemoveAt (0);
				// edges and colors
				ParseLines (lines, knot.Edges);

				return knot;

			} else {
				// file has too few lines
				return null;
			}
		}

		/// <summary>
		/// Saves the knot to a file.
		/// </summary>
		/// <returns>
		/// The knot.
		/// </returns>
		/// <param name='filename'>
		/// The filename.
		/// </param>
		public void SaveKnot (Knot knot)
		{
			try {
				string content = string.Join (System.Environment.NewLine,
				                              ToLines (knot)) + System.Environment.NewLine;
				string filepath = knot.Info.Filename;
				if (!Files.IsPath (filepath)) {
					filepath = Files.SavegameDirectory + Files.Separator + filepath;
				}
				File.WriteAllText (filepath, content);
			} catch (Exception ex) {
				Console.WriteLine (ex);
			}
		}
		
		/// <summary>
		/// Finds the filename for a specified knot name.
		/// </summary>
		/// <returns>
		/// The filename.
		/// </returns>
		/// <param name='knotName'>
		/// Knot name.
		/// </param>
		public string FindFilename (string knotName)
		{
			return Files.ValidFilename (knotName) + FileExtensions [0];
		}

		private static void ParseLines (List<string> lines, EdgeList edges)
		{
			for (int i = 0; i < lines.Count(); ++i) {
				Edge edge;
				Color color;
				if (ParseLine (lines [i], out edge, out color)) {
					Console.WriteLine (lines [i] + ": edge=" + edge + ", color=" + color);
					edge.Color = color;
					edges.Add (edge);
				}
			}
		}

		private static bool ParseLine (string line, out Edge edge, out Color color)
		{
			try {
				edge = DecodeEdge (line [0]);
				color = DecodeColor (line.Substring (1, 8));
				return true;
			} catch (FormatException ex) {
				Console.WriteLine (ex.ToString ());
				edge = null;
				color = Edge.RandomColor ();
				return false;
			}
		}

		private static IEnumerable<int> ParseIntegers (string str)
		{
			string[] parts = str.Split (new char[] {':',','}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string s in parts) {
				int i;
				if (Int32.TryParse (s, out i)) {
					yield return i;
				}
			}
		}

		private static IEnumerable<string> ToLines (Knot knot)
		{
			yield return knot.Info.Name;
			knot.Edges.Compact ();
			foreach (Edge edge in knot.Edges) {
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
				throw new FormatException ("Failed to decode Edge!");
			}
		}

		private static char EncodeEdge (Edge edge)
		{
			if (edge.Direction == Vector3.Right)
				return 'X';
			else if (edge.Direction == Vector3.Left)
				return  'x';
			else if (edge.Direction == Vector3.Up)
				return  'Y';
			else if (edge.Direction == Vector3.Down)
				return  'y';
			else if (edge.Direction == Vector3.Backward)
				return  'Z';
			else if (edge.Direction == Vector3.Forward)
				return  'z';
			else
				throw new FormatException ("Failed to encode Edge!");
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
	}
}

