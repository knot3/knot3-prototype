using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TestGame1
{
	public class Node
	{

		public int X { get; private set; }

		public int Y { get; private set; }

		public int Z { get; private set; }

		public Node (int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public static int Scale { get; set; }

		public static Node operator + (Node a, Node b)
		{
			return new Node (a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}

		public static Node operator + (Node a, Vector3 b)
		{
			return new Node (a.X + (int)b.X, a.Y + (int)b.Y, a.Z + (int)b.Z);
		}

		public Vector3 Vector ()
		{
			return new Vector3 (X * Scale, Y * Scale, Z * Scale);
		}

		public static bool operator == (Node a, Node b)
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
			return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
		}

		public static bool operator != (Node a, Node b)
		{
			return !(a == b);
		}
	}

	public class NodeList
	{
		private List<Node> list = new List<Node> ();

		public Node this [int i] {
			get {
				while (i < 0) {
					i += list.Count;
				}
				return list [i % list.Count];
			}
			set {
				while (i < 0) {
					i += list.Count;
				}
				list [i % list.Count] = value;
			}
		}

		public int Count {
			get {
				return list.Count;
			}
		}

		public void Add (Node node)
		{
			list.Add (node);
		}

		public void InsertAt (int i, Node a)
		{
			list.Insert (i % list.Count, a);
		}

		public void InsertAt (int i, Node[] a)
		{
			if (i < list.Count) {
				list.InsertRange (i, a);
			} else {
				list.AddRange (a);
			}
		}

		public void RemoveAt (int i)
		{
			list.RemoveAt (i % list.Count);
		}

		public void RemoveAt (int[] i)
		{
			for (int n = 0; n < i.Length; ++n) {
				i [n] = i [n] % list.Count;
			}
			for (int n = 0; n < i.Length; ++n) {
				if (list.Count > 0) {
					list.RemoveAt (i [n] % list.Count);
				}
			}
		}

		public void Print ()
		{
			string str = "";
			foreach (Node node in list) {
				if (str.Length > 0)
					str += ", ";
				str += "(" + node.X + "," + node.Y + "," + node.Z + ")";
			}
			Console.WriteLine (str);
		}
	}

	public class Line
	{
		public Node From { get; private set; }

		public Node To { get; private set; }

		public Line (Node from, Node to)
		{
			From = from;
			To = to;
		}
	}

	public class LineList
	{
		private NodeList Nodes;

		public LineList (NodeList nodes)
		{
			Nodes = nodes;
		}

		public Line this [int i] {
			get {
				return new Line (Nodes [i], Nodes [i + 1]);
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

		public Color Color (int i)
		{
			return SelectedLine == i ? Microsoft.Xna.Framework.Color.Red : Microsoft.Xna.Framework.Color.Black;
		}

		private int _SelectedLine = -1;

		public int SelectedLine {
			set {
				_SelectedLine = (Nodes.Count + value) % Nodes.Count;
				Console.WriteLine ("selected line: " + _SelectedLine);
				Nodes.Print ();
			}
			get { return _SelectedLine;}
		}

		public void InsertAt (int i, Vector3 direction)
		{
			Console.WriteLine ("InsertAt: selected=" + i + ", direction=" + direction);
			if (Nodes.Count >= 2) {
				Node a = Nodes [i];
				Node b = Nodes [i + 1];
				Vector3 directionAB = Vector3.Normalize (a.Vector () - b.Vector ());
				if (directionAB != direction && directionAB != -direction) {
					Nodes.InsertAt (i + 1, new Node[]{ a + direction, b + direction });
					SelectedLine = i + 1;
				}
				Nodes.Print ();
				Compact ();
			} else {
				Nodes.Add (new Node (0, 0, 0));
				Nodes.Add (new Node (0, 0, 0) + direction);
			}
		}

		public void RemoveAt (int i)
		{
			// TODO
			Node a = Nodes [i - 1];
			Node b = Nodes [i];
			Node c = Nodes [i + 1];
			Compact ();
			Nodes.Print ();
		}

		public void Compact ()
		{
			bool done = false;
			while (!done) {
				done = true;
				for (int i = 0; i < Nodes.Count; ++i) {
					if (Nodes [i] == Nodes [i + 2] && Nodes.Count >= 4) {
						if (SelectedLine > i)
							SelectedLine -= 2;
						Nodes.RemoveAt (new int[]{i + 2, i + 1});
						done = false;
						break;
					}
					if (Nodes [i] == Nodes [i + 1] && Nodes.Count >= 3) {
						if (SelectedLine > i)
							SelectedLine -= 1;
						Nodes.RemoveAt (new int[]{i + 1});
						done = false;
						break;
					}
				}
			}
		}
	}
}

