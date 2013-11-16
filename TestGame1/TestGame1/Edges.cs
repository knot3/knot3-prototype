using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TestGame1
{
	public class Edge
	{
		public Node FromNode { get; private set; }

		public Node ToNode { get; private set; }

		public Edge (Node from, Node to)
		{
			FromNode = from;
			ToNode = to;
		}

		public Color Color {
			get { return FromNode.Color; }
			set { FromNode.Color = value; }
		}

		public string ID {
			get { return FromNode.ID + "-" + ToNode.ID; }
		}

		public Vector3 Direction ()
		{
			return Vector3.Normalize (FromNode.Vector () - ToNode.Vector ());
		}

		public static bool operator == (Edge a, Edge b)
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
			return a.FromNode == b.FromNode && a.ToNode == b.ToNode;
		}

		public static bool operator != (Edge a, Edge b)
		{
			return !(a == b);
		}
		
		public override bool Equals (object obj)
		{
			Edge other = obj as Edge;
			return FromNode == other.FromNode && ToNode == other.ToNode;
		}

		public override int GetHashCode ()
		{
			return FromNode.GetHashCode () * 10 + ToNode.GetHashCode ();
		}
	}

	public class EdgeList
	{
		private NodeList Nodes;

		public Action LinesChanged { get; set; }

		public EdgeList (NodeList nodes)
		{
			Nodes = nodes;
		}

		public Edge this [int i] {
			get {
				return At (i);
			}
		}

		public Edge At (int i)
		{
			return new Edge (Nodes [i], Nodes [i + 1]);
		}

		/*public Edge this [string id] {
			get {
				for (int i = 0; i < Nodes.Count; ++i) {
					Edge edge = new Edge (Nodes [i], Nodes [i + 1]);
					if (edge.ID == id) {
						return edge;
					}
				}
				throw new ArgumentOutOfRangeException ("edge does not exist!");
			}
		}

		/// <summary>
		/// Return an identifier for the specified edge
		/// </summary>
		/// <param name='line'>
		/// Line.
		/// </param>
		public string this [Edge edge] {
			get {
				for (int i = 0; i < Nodes.Count; ++i) {
					if (Nodes [i] == edge.FromNode && Nodes [i + 1] == edge.ToNode) {
						return edge.ID;
					}
				}
				throw new ArgumentOutOfRangeException ("edge does not exist!");
			}
		}*/

		public int IndexOf (Edge edge)
		{
			for (int i = 0; i < Nodes.Count; ++i) {
				if (Nodes [i] == edge.FromNode && Nodes [i + 1] == edge.ToNode) {
					return i;
				}
			}
			throw new ArgumentOutOfRangeException ("edge does not exist!");
		}

		public int Count {
			get {
				if (Nodes.Count >= 2)
					return Nodes.Count;
				else
					return 0;
			}
		}

		private List<Edge> _SelectedEdges = new List<Edge> ();

		public List<Edge> SelectedEdges {
			set {
				//_SelectedEdges = new List<int> ();
				//foreach (Edge sel in value) {
				//	_SelectedEdges.Add ((Nodes.Count + sel) % Nodes.Count);
				//}
				_SelectedEdges = value;
				Console.Write ("selected lines: ");
				foreach (Edge edge in _SelectedEdges) {
					Console.Write (edge.ID + " ");
				}
				Console.WriteLine ();
				Nodes.Print ();
			}
			get {
				return _SelectedEdges;
			}
		}

		public bool InsertAt (List<Edge> selected, Vector3 direction)
		{
			if (selected.Count == 1) {
				return InsertAt (selected [0], direction);
			}
			return false;
		}

		public bool InsertAt (Edge i, Vector3 direction)
		{
			bool successful = false;
			if (direction != Vector3.Zero) {
				Console.WriteLine ("InsertAt: selected=" + IndexOf(i) + ", direction=" + direction);
				if (Nodes.Count >= 2) {
					Node a = i.FromNode;
					Node b = i.ToNode;
					Vector3 directionAB = i.Direction ();
					if (directionAB != direction && directionAB != -direction) {
						Nodes.InsertAt (IndexOf (i) + 1, new Node[] {
							a + direction,
							b + direction
						}
						);
						////SelectedEdges = new List<Edge> (){this[IndexOf (i)  + 1]};
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
					if (Nodes [i].Vector() == Nodes [i + 2].Vector() && Nodes.Count >= 4) {
						////for (int j = 0; j < SelectedEdges.Count; ++j) {
						////	if (IndexOf(SelectedEdges [j]) > i)
						////		SelectedEdges [j] -= 2;
						////}
						Nodes.RemoveAt (new int[]{i + 2, i + 1});
						done = false;
						successful = true;
						break;
					}
					if (Nodes [i].Vector() == Nodes [i + 1].Vector() && Nodes.Count >= 3) {
						////for (int j = 0; j < SelectedLines.Count; ++j) {
						////	if (SelectedLines [j] > i)
						////		SelectedLines [j] -= 1;
						////}
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

