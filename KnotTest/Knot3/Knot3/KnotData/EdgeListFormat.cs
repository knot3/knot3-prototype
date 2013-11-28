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
				Vector3? vector;
				Color? color;
				ParseLine (lines [i], out vector, out color);
				if (vector.HasValue) {
					Edge edge = new Edge (vector.Value);
					if (color.HasValue)
						edge.Color = color.Value;
					Console.WriteLine(i+"="+vector.Value.X + "," + vector.Value.Y + "," + vector.Value.Z + ";" + color.Value.R + "," + color.Value.G + "," + color.Value.B);
					edges.Add (edge);
				}
			}
		}

		private static void ParseLine (string line, out Vector3? vector, out Color? color)
		{
			string[] parts = line.Split (new char[] {'#',';'}, StringSplitOptions.RemoveEmptyEntries);
			vector = ParseVector3 (ParseIntegers (parts [0]));
			if (vector.HasValue && parts.Length == 2)
				color = ParseColor (ParseIntegers (parts [1]));
			else
				color = Edge.RandomColor ();
		}

		private static Vector3? ParseVector3 (IEnumerable<int> values)
		{
			int[] array = values.ToArray ();
			if (array.Count () == 3)
				return new Vector3 (array [0], array [1], array [2]);
			else
				return null;
		}

		private static Color? ParseColor (IEnumerable<int> values)
		{
			int[] array = values.ToArray ();
			if (array.Count () == 3)
				return new Color (array [0], array [1], array [2]);
			else
				return null;
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
			for (int i = 0; i < knot.Edges.Count; ++i) {
				Vector3 direction = knot.Edges [i].Direction;
				Color color = knot.Edges [i].Color;
				yield return direction.X + "," + direction.Y + "," + direction.Z + ";" + color.R + "," + color.G + "," + color.B;
			}
		}
	}
}

