using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Knot3.Utilities;

namespace Knot3.KnotData
{
	public class EdgeList
	{
		#region Properties

		private WrapList<Edge> Edges;

		public WrapList<Edge> SelectedEdges { get; private set; }

		public Action<EdgeList> EdgesChanged = (list) => {};
		private Dictionary<int,Node> NodeCache;

		#endregion

		#region Constructors

		public EdgeList ()
		{
			Edges = new WrapList<Edge> ();
			SelectedEdges = new WrapList<Edge> ();
			NodeCache = new Dictionary<int, Node> ();
		}

		#endregion

		#region Operators

		public Edge this [int i] {
			get {
				return Edges [i];
			}
		}

		#endregion

		#region Public Methods

		public int IndexOf (Edge edge)
		{
			int i = Edges [edge];
			if (i != -1)
				return i;
			else
				throw new ArgumentOutOfRangeException ("edge does not exist!");
		}

		public Node FromNode (int index)
		{
			if (NodeCache.ContainsKey (index)) {
				return NodeCache [index];
			} else {
				Node node = new Node (0, 0, 0);
				for (int i = 0; i < index; ++i) {
					node += Edges [i].Direction;
				}
				NodeCache [index] = node;
				return node;
			}
		}

		public Node FromNode (Edge edge)
		{
			return FromNode (IndexOf (edge));
		}

		public Node ToNode (int index)
		{
			return FromNode (index + 1);
		}

		public Node ToNode (Edge edge)
		{
			return ToNode (IndexOf (edge));
		}

		public void Add (Edge edge)
		{
			Edges.Add (edge);
		}

		public void AddRange (IEnumerable<Edge> edge)
		{
			Edges.AddRange (edge);
		}
		
		public List<Edge> Interval (Edge a, Edge b)
		{
			int indexA = IndexOf (a);
			int indexB = IndexOf (b);
			int indexFirst = (int)MathHelper.Min (indexA, indexB);
			int indexLast = (int)MathHelper.Max (indexA, indexB);
			if (indexLast < indexFirst)
				indexLast += Edges.Count;
			if (Edges.Count - (indexLast - indexFirst) < (indexLast - indexFirst)) {
				Console.WriteLine ("Nodes.Count - diff < diff");
				Vectors.Swap (ref indexLast, ref indexFirst);
				if (indexLast < indexFirst)
					indexLast += Edges.Count;
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

		public int Count { get { return Edges.Count; } }

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
			Console.WriteLine ("nodes: " + Edges);
		}

		public bool Move (IEnumerable<Edge> selectedEdges, Vector3 direction)
		{
			Console.WriteLine ("Move: selection=" + selectedEdges + ", direction=" + direction);
			Console.WriteLine ("Before Move => " + Edges);
			foreach (Edge selectedEdge in SelectedEdges) {
				Edges.Replace (selectedEdge, new Edge[] {
					new Edge (direction),
					selectedEdge,
					new Edge (-direction)
				}
				);
			}
			Console.WriteLine ("After Move => " + Edges);
			Compact ();
			Console.WriteLine ("Compact => " + Edges);
			EdgesChanged (this);
			return true;
		}

		public bool Move (Edge selection, Vector3 direction)
		{
			return Move (new Edge[]{ selection }, direction);
		}

		public bool Compact ()
		{
			NodeCache.Clear ();
			bool successful = false;
			bool done = false;
			while (!done) {
				done = true;
				for (int i = 0; i < Edges.Count; ++i) {
					Edge current = Edges [i];
					Edge next = Edges [i + 1];
					if (current.Direction == -next.Direction && Edges.Count >= 4) {
						Edges.Remove (new Edge[]{current,next});
						done = false;
						successful = true;
						break;
					}
				}
				if (Edges.Count >= 2) {
					Vector3 distance = ToNode (Edges [-1]) - FromNode (Edges [0]);
					if (distance.Length () > 0) {
						Edges.AddRange (PathTo (Edges [-1], Edges [0]));
						done = false;
						successful = true;
					}
				}
			}
			NodeCache.Clear ();
			return successful;
		}

		public Edge[] PathTo (Edge fromEdge, Edge toEdge)
		{
			List<Edge> path = new List<Edge> ();
			Vector3 distance = FromNode (toEdge) - ToNode (fromEdge);
			do {
				Console.WriteLine ("distance=" + distance);
				if (distance.X != 0) {
					Vector3 edge = Vector3.Normalize (new Vector3 (distance.X, 0, 0));
					path.Add (new Edge (edge));
					distance -= edge;
				} else if (distance.Y != 0) {
					Vector3 edge = Vector3.Normalize (new Vector3 (0, distance.Y, 0));
					path.Add (new Edge (edge));
					distance -= edge;
				} else if (distance.Z != 0) {
					Vector3 edge = Vector3.Normalize (new Vector3 (0, 0, distance.Z));
					path.Add (new Edge (edge));
					distance -= edge;
				}
			} while (distance != Vector3.Zero);
			return path.ToArray ();
		}

		public static IEnumerable<Edge> FromNodes (IEnumerable<Node> _nodes, IEnumerable<Color> _colors = null)
		{
			Node[] nodes = _nodes.ToArray ();
			Color[] colors = _colors != null ? _colors.ToArray () : null;
			for (int i = 0; i+1 < nodes.Count(); ++i) {
				Vector3 direction = nodes [i + 1] - nodes [i];
				if (direction.Length () == 1) {
					Edge edge = new Edge (direction.PrimaryDirection ());
					if (i < colors.Count ())
						edge.Color = colors [i];
					yield return edge;
				}
			}
		}

		#endregion
	}
}

