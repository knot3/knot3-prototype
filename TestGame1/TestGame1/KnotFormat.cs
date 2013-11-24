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

namespace TestGame1
{
	public class KnotFormat
	{
		public string Filename;

		public KnotFormat (string filename)
		{
			Filename = filename;
		}

		public KnotInfo Info {
			// load knot info
			get {
				string name = null;
				int edgeCount = 0;
				foreach (string line in Files.ReadFrom (Filename)) {
					// first line is the name
					if (name == null)
						name = line.Trim ();
					// every non-empty line afterwards is a node coordinate
					else if (name.Trim ().Length > 0)
						++edgeCount;
				}
				// create a new info object
				return new KnotInfo {
					Filename = Filename,
					Name = name,
					EdgeCount = () => edgeCount,
					IsValid = name != null && name.Length > 0 && edgeCount >= 2
				};
			}
		}

		public Knot Knot {
			// load knot from file
			get {
				// load knot info
				KnotInfo info = Info;
				if (info.IsValid) {
					// create a knot object
					Knot knot = new Knot (info, (k) => new KnotFormat (Filename).Knot = k);
					// read lines
					WrapList<string> lines = new WrapList<string> (Files.ReadFrom (Filename));
					// remove the line containing the name of the knot
					lines.Remove (new []{ lines [0] });
					// edges and colors
					ParseLines (lines, knot.Edges);

					return knot;

				} else {
					// file has too few lines
					return null;
				}
			}

			// save knot to 
			set {
				try {
					string content = string.Join (System.Environment.NewLine, ToLines (value)) + System.Environment.NewLine;
					File.WriteAllText (value.Info.Filename, content);
				} catch (Exception ex) {
					Console.WriteLine (ex);
				}
			}
		}

		private static void ParseLines (WrapList<string> lines, EdgeList edges)
		{
			Node? previousNode = null;
			for (int i = 0; i <= lines.Count(); ++i) {
				Node? node;
				Color? color;
				ParseLine (lines [i], out node, out color);
				if (node.HasValue && previousNode.HasValue) {
					Edge edge = new Edge (node.Value - previousNode.Value);
					if (color.HasValue)
						edge.Color = color.Value;
					edges.Add (edge);
				}
				previousNode = node;
			}
		}

		private static void ParseLine (string line, out Node? node, out Color? color)
		{
			string[] parts = line.Split ('#');
			node = ParseNode (ParseIntegers (parts [0]));
			if (node.HasValue && parts.Length == 2)
				color = ParseColor (ParseIntegers (parts [1]));
			else
				color = Edge.RandomColor ();
		}

		private static Node? ParseNode (IEnumerable<int> values)
		{
			int[] array = values.ToArray ();
			if (array.Count () == 3)
				return new Node (array [0], array [1], array [2]);
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
			string[] parts = str.Split (':');
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
				Node node = knot.Edges.FromNode (knot.Edges [i]);
				Color color = knot.Edges [i].Color;
				yield return node.X + ":" + node.Y + ":" + node.Z + "#" + color.R + ":" + color.G + ":" + color.B;
			}
		}
	}
}

