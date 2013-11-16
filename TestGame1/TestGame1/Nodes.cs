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

		public Color Color;

		public int ID { get; private set; }

		private static List<int> UsedIDs = new List<int> ();

		public Node (int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
			Color = DefaultColor;
			do {
				ID = r.Next () % 10000;
			} while (UsedIDs.Contains(ID));
			UsedIDs.Add (ID);
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
			return a.ID == b.ID; //a.X == b.X && a.Y == b.Y && a.Z == b.Z;
		}

		public static bool operator != (Node a, Node b)
		{
			return !(a == b);
		}
		
		public override bool Equals (object obj)
		{
			Node other = obj as Node;
			return ID == other.ID; //X == other.X && Y == other.Y && Z == other.Z;
		}

		public override int GetHashCode ()
		{
			return X * 10000 + Y * 100 + Z;
		}
		
		private static Random r = new Random ();
		public static List<Color> Colors = new List<Color> (){
			Color.Chartreuse, Color.Coral, Color.DeepPink, Color.ForestGreen,
			Color.Gold, Color.MidnightBlue, Color.MediumSpringGreen, Color.Teal
		};
		public static Color DefaultColor = RandomColor ();

		public static Color RandomColor ()
		{
			return Colors[r.Next()%Colors.Count];
			// new Color ((float)r.NextDouble (), (float)r.NextDouble (), (float)r.NextDouble ());
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

		public int IndexOf (Node node)
		{
			int index = list.IndexOf (node);
			if (index >= 0) {
				return index;
			} else {
				throw new ArgumentOutOfRangeException ("node " + node + " does not exist!");
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
}

