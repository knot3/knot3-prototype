using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TestGame1
{
	

	public class Line
	{
		public Node From { get; private set; }

		public Node To { get; private set; }

		public Line (Node from, Node to)
		{
			From = from;
			To = to;
		}

		public Color Color {
			get { return From.Color; }
			set { From.Color = value; }
		}

		public static bool operator == (Line a, Line b)
		{
			// If both are null, or both are same instance, return true.
			if (System.Object.ReferenceEquals (a, b)) {
				return true;
			}

			// If one is null, but not both, return false.
			if (((object)a == null) || ((object)b == null)) {
				return false;
			}

			// Return true if the fields match:
			return a.From == b.From && a.To == b.To;
		}

		public static bool operator != (Line a, Line b)
		{
			return !(a == b);
		}
		
		public override bool Equals (object obj)
		{
			Line other = obj as Line;
			return From == other.From && To == other.To;
		}

		public override int GetHashCode ()
		{
			return From.GetHashCode () * 10 + To.GetHashCode ();
		}
	}

	public class LineList
	{
		private NodeList Nodes;

		public Action LinesChanged { get; set; }

		public LineList (NodeList nodes)
		{
			Nodes = nodes;
		}

		public Line this [int i] {
			get {
				return new Line (Nodes [i], Nodes [i + 1]);
			}
		}

		public int this [Line line] {
			get {
				for (int i = 0; i < Nodes.Count; ++i) {
					if (Nodes [i] == line.From && Nodes [i + 1] == line.To) {
						return i;
					}
				}
				throw new ArgumentOutOfRangeException ("line does not exist!");
			}
		}

		public int Count {
			get {
				if (Nodes.Count >= 2)
					return Nodes.Count;
				else
					return 0;
			}
		}

		private List<int> _SelectedLines = new List<int> ();

		public List<int> SelectedLines {
			set {
				_SelectedLines = new List<int>();
				foreach (int sel in value) {
					_SelectedLines.Add ((Nodes.Count + sel) % Nodes.Count);
				}
				Console.WriteLine ("selected lines: " + " ".Join(_SelectedLines));
				Nodes.Print ();
			}
			get {
				return _SelectedLines;
			}
		}

		public bool InsertAt (List<int> i, Vector3 direction)
		{
			if (i.Count == 1) {
				return InsertAt (i [0], direction);
			}
			return false;
		}

		public bool InsertAt (int i, Vector3 direction)
		{
			bool successful = false;
			if (direction != Vector3.Zero) {
				Console.WriteLine ("InsertAt: selected=" + i + ", direction=" + direction);
				if (Nodes.Count >= 2) {
					Node a = Nodes [i];
					Node b = Nodes [i + 1];
					Vector3 directionAB = Vector3.Normalize (a.Vector () - b.Vector ());
					if (directionAB != direction && directionAB != -direction) {
						Nodes.InsertAt (i + 1, new Node[]{ a + direction, b + direction });
						SelectedLines = new List<int> (){i + 1};
						successful = true;
					}
					Nodes.Print ();
					Compact ();
				} else {
					Nodes.Add (new Node (0, 0, 0));
					Nodes.Add (new Node (0, 0, 0) + direction);
					successful = true;
				}
				if (successful) {
					LinesChanged ();
				}
			}
			return successful;
		}

		public void RemoveAt (int i)
		{
			// TODO
			/*
			 * Node a = Nodes [i - 1];
			 * Node b = Nodes [i];
			 * Node c = Nodes [i + 1];
			 * Compact ();
			 * Nodes.Print ();
			 */
		}

		public bool Compact ()
		{
			bool successful = false;
			bool done = false;
			while (!done) {
				done = true;
				for (int i = 0; i < Nodes.Count; ++i) {
					if (Nodes [i] == Nodes [i + 2] && Nodes.Count >= 4) {
						for (int j = 0; j < SelectedLines.Count; ++j) {
							if (SelectedLines [j] > i)
								SelectedLines [j] -= 2;
						}
						Nodes.RemoveAt (new int[]{i + 2, i + 1});
						done = false;
						successful = true;
						break;
					}
					if (Nodes [i] == Nodes [i + 1] && Nodes.Count >= 3) {
						for (int j = 0; j < SelectedLines.Count; ++j) {
							if (SelectedLines [j] > i)
								SelectedLines [j] -= 1;
						}
						Nodes.RemoveAt (new int[]{i + 1});
						done = false;
						successful = true;
						break;
					}
				}
			}
			return successful;
		}
	}
}

