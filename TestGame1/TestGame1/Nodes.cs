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

		public Vector3 Vector ()
		{
			return new Vector3 (X * Scale, Y * Scale, Z * Scale);
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
			return SelectedLine == i ? Microsoft.Xna.Framework.Color.Red : Microsoft.Xna.Framework.Color.White;
		}

		private int _SelectedLine = -1;

		public int SelectedLine {
			set {
				_SelectedLine = (Nodes.Count + value) % Nodes.Count;
				Console.WriteLine ("selected line: " + _SelectedLine);
			}
			get { return _SelectedLine;}
		}
	}
}

