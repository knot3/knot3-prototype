using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Knot3.Utilities;
using System.Collections;

namespace Knot3.KnotData
{
	/// <summary>
	/// Eine Liste von Kanten (Instanzen der Klasse Edge), die zusätzlich Methoden zum Verschieben der Kanten enthält
	/// und für jede Kante die Start- und End-Knotenpunkte zurückgeben kann.
	/// </summary>
	public class EdgeList : IEnumerable<Edge>
	{
		#region Properties

		private WrapList<Edge> edges;

		public WrapList<Edge> SelectedEdges { get; private set; }

		public Action<EdgeList> EdgesChanged = (list) => {};

		#endregion

		#region Constructors

		public EdgeList ()
		{
			edges = new WrapList<Edge> ();
			SelectedEdges = new WrapList<Edge> ();
		}

		#endregion

		#region Operators

		public Edge this [int i] {
			get {
				return edges [i];
			}
		}

		#endregion

		#region Public Methods

		private int IndexOf (Edge edge)
		{
			int i = edges [edge];
			if (i != -1)
				return i;
			else
				throw new ArgumentOutOfRangeException ("edge does not exist!");
		}
		
		public IEnumerator<Edge> GetEnumerator ()
		{
			return edges.GetEnumerator ();
		}

		// Explicit interface implementation for nongeneric interface
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator (); // Just return the generic version
		}

		public void Add (Edge edge)
		{
			edges.Add (edge);
		}

		public void AddRange (IEnumerable<Edge> edge)
		{
			edges.AddRange (edge);
		}
		
		public List<Edge> Interval (Edge a, Edge b)
		{
			int indexA = IndexOf (a);
			int indexB = IndexOf (b);
			int indexFirst = (int)MathHelper.Min (indexA, indexB);
			int indexLast = (int)MathHelper.Max (indexA, indexB);
			if (indexLast < indexFirst)
				indexLast += edges.Count;
			if (edges.Count - (indexLast - indexFirst) < (indexLast - indexFirst)) {
				Console.WriteLine ("Nodes.Count - diff < diff");
				Vectors.Swap (ref indexLast, ref indexFirst);
				if (indexLast < indexFirst)
					indexLast += edges.Count;
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

		public int Count { get { return edges.Count; } }

		public void PrintSelectedEdges ()
		{
			Console.Write ("selected edges: ");
			foreach (Edge edge in SelectedEdges) {
				Console.Write (edge.ID + " ");
			}
			Console.WriteLine ();
			Console.WriteLine ("nodes: " + edges);
		}

		public bool Move (IEnumerable<Edge> SelectedEdges, Vector3 direction, int times = 1)
		{
			Console.WriteLine ("Move: selection=" + SelectedEdges + ", direction=" + direction);
			Console.WriteLine ("Before Move => " + edges);
			foreach (Edge selectedEdge in SelectedEdges) {
				List<Edge> replacement = new List<Edge> ();
				times.Times (() => replacement.Add (new Edge (direction)));
				replacement.Add (selectedEdge);
				times.Times (() => replacement.Add (new Edge (-direction)));
				edges.Replace (selectedEdge, replacement.ToArray ());
			}
			Console.WriteLine ("After Move => " + edges);
			Compact ();
			Console.WriteLine ("Compact => " + edges);
			EdgesChanged (this);
			return true;
		}

		public bool Move (Edge selection, Vector3 direction)
		{
			return Move (new Edge[]{ selection }, direction);
		}

		public bool Compact ()
		{
			bool successful = false;
			bool done = false;
			while (!done) {
				done = true;
				for (int i = 0; i < edges.Count; ++i) {
					Edge current = edges [i];
					Edge next = edges [i + 1];
					if (current.Direction == -next.Direction && edges.Count >= 4) {
						edges.Remove (new Edge[]{current,next});
						done = false;
						successful = true;
						break;
					}
				}
				if (edges.Count >= 2) {
					Vector3 distance = End () - new Node (0, 0, 0);
					if (distance.Length () > 0) {
						edges.AddRange (PathTo (edges [-1], edges [0], distance));
						done = false;
						successful = true;
					}
				}
			}
			return successful;
		}

		private Node End ()
		{
			float x = 0, y = 0, z = 0;
			foreach (Edge edge in edges) {
				Vector3 v = edge.Direction;
				x += v.X;
				y += v.Y;
				z += v.Z;
			}
			return new Node ((int)x, (int)y, (int)z);
		}

		public Edge[] PathTo (Edge fromEdge, Edge toEdge, Vector3 distance)
		{
			List<Edge> path = new List<Edge> ();
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

