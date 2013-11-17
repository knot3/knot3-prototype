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
			SelectedEdges = new List<Edge> ();
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
		
		public List<Edge> Interval (Edge a, Edge b)
		{
			int indexA = IndexOf (a);
			int indexB = IndexOf (b);
			int indexFirst = (int)MathHelper.Min (indexA, indexB);
			int indexLast = (int)MathHelper.Max (indexA, indexB);
			if (indexLast < indexFirst)
				indexLast += Nodes.Count;
			if (Nodes.Count - (indexLast - indexFirst) < (indexLast - indexFirst)) {
				Console.WriteLine ("Nodes.Count - diff < diff");
				VectorExtensions.Swap (ref indexLast, ref indexFirst);
				if (indexLast < indexFirst)
					indexLast += Nodes.Count;
			}
			List<Edge> interval = new List<Edge> ();
			Console.Write ("Interval(" + indexFirst + "," + indexLast + ")=");
			for (int i = indexFirst+1; i < indexLast; ++i) {
				interval.Add (this [i]);
				Console.Write (i);
			}
			Console.WriteLine ();
			return interval;
		}

		public int Count {
			get {
				if (Nodes.Count >= 2)
					return Nodes.Count;
				else
					return 0;
			}
		}

		public List<Edge> SelectedEdges { get; private set; }

		public void SelectEdge (Edge selection, bool append = false)
		{
			if (!append)
				SelectedEdges.Clear ();
			SelectedEdges.Add (selection);
		}

		public void SelectEdges (Edge[] selection, bool append = false)
		{
			if (!append)
				SelectedEdges.Clear ();
			SelectedEdges.AddRange (selection);
		}

		public void SelectEdge ()
		{
			SelectedEdges .Clear ();
		}

		public void PrintSelectedEdges ()
		{
			Console.Write ("selected edges: ");
			foreach (Edge edge in SelectedEdges) {
				Console.Write (edge.ID + " ");
			}
			Console.WriteLine ();
			Console.WriteLine ("nodes: " + Nodes);
		}

		public bool InsertAt (List<Edge> selected, Vector3 direction)
		{
			if (selected.Count == 1) {
				return InsertAt (selected [0], direction);
			}
			return false;
		}

		public bool InsertAt (Edge selection, Vector3 direction)
		{
			bool successful = false;
			if (direction != Vector3.Zero) {
				Console.WriteLine ("InsertAt: selected=" + IndexOf (selection) + ", direction=" + direction);
				if (Nodes.Count >= 2) {
					Node a = selection.FromNode;
					Node b = selection.ToNode;
					Vector3 directionAB = selection.Direction ();
					if (directionAB != direction && directionAB != -direction) {
						Nodes.InsertAt (IndexOf (selection) + 1, new Node[] {
							a + direction,
							b + direction
						}
						);
						////SelectedEdges = new List<Edge> (){this[IndexOf (i)  + 1]};
						successful = true;

					} else {
						Console.WriteLine ("before: " + Nodes);
						int fromIndex = IndexOf (selection) + 1;
						int toIndex = -1;
						for (int k = fromIndex; k < Nodes.Count+fromIndex-1; ++k) {
							Vector3 directionK = this [k].Direction ();
							if (directionK == -directionAB) {
								Console.WriteLine ("Nodes[" + k + "] =" + Nodes [k] + " same direction");
								toIndex = k;
								break;
							}
						}
						for (int k = fromIndex; k <= toIndex; ++k) {
							Console.Write ("Nodes[" + k + "] =" + Nodes [k]);
							Nodes [k] = Nodes [k] + direction;
							Console.WriteLine ("=> Nodes[" + k + "] =" + Nodes [k]);
						}
						successful = true;
					}
					Console.WriteLine ("InsertAt => " + Nodes);
					Compact ();
					Console.WriteLine ("compact => " + Nodes);
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
					Vector3 edgeVector = Nodes [i + 1] - Nodes [i];
					if (edgeVector.Length () > 1) {
						Nodes.InsertAt (i + 1, PathTo (Nodes [i], Nodes [i + 1]));
						done = false;
						successful = true;
					}
					if (Nodes [i].Vector () == Nodes [i + 2].Vector () && Nodes.Count >= 4) {
						Nodes.RemoveAt (new int[]{i + 2, i + 1});
						done = false;
						successful = true;
						break;
					}
					if (Nodes [i].Vector () == Nodes [i + 1].Vector () && Nodes.Count >= 3) {
						Nodes.RemoveAt (new int[]{i + 1});
						done = false;
						successful = true;
						break;
					}
				}
			}
			return successful;
		}

		public Node[] PathTo (Node from, Node to)
		{
			List<Node> path = new List<Node> ();
			Vector3 edge = Vector3.Zero;
			Node current = from;
			do {
				edge = to - current;
				Console.WriteLine ("from=" + from + ", to=" + to + ", edge=" + edge);
				if (edge.X != 0) {
					current = current + Vector3.Normalize (new Vector3 (edge.X, 0, 0));
					path.Add (current);
				} else if (edge.Y != 0) {
					current = current + Vector3.Normalize (new Vector3 (0, edge.Y, 0));
					path.Add (current);
				} else if (edge.Z != 0) {
					current = current + Vector3.Normalize (new Vector3 (0, 0, edge.Z));
					path.Add (current);
				}
			} while (edge != Vector3.Zero);
			return path.ToArray ();
		}
	}
}

