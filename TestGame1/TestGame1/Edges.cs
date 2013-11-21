using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TestGame1
{
	public class Edge
	{
		#region Properties

		public Vector3 Direction { get; private set; }

		public Color Color { get; set; }

		public int ID { get; private set; }

		private static List<int> UsedIDs = new List<int> (); 

		#endregion

		#region Constructors

		public Edge (int x, int y, int z)
		{
			Direction = new Vector3 (x, y, z);
			Color = DefaultColor;
			ID = RandomID ();
		}

		public Edge (Vector3 v)
		{
			Direction = v.PrimaryDirection ();
			Color = DefaultColor;
			ID = RandomID ();
		}

		#endregion

		#region Operators

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
			return a.ID == b.ID;
		}

		public static bool operator != (Edge a, Edge b)
		{
			return !(a == b);
		}
		
		public override bool Equals (object obj)
		{
			Edge other = obj as Edge;
			return this.ID == other.ID;
		}

		public override int GetHashCode ()
		{
			return ID;
		}

		public override string ToString ()
		{
			return Direction.Print ();
		}

		#endregion

		#region Helper Methods

		private static Random r = new Random ();

		private int RandomID ()
		{
			int id;
			do {
				id = r.Next () % 10000;
			} while (UsedIDs.Contains(id));
			UsedIDs.Add (id);
			return id;
		}

		public static Color RandomColor ()
		{
			return Colors [r.Next () % Colors.Count];
		}

		public static Color RandomColor (GameTime gameTime)
		{
			return Colors [(int)gameTime.TotalGameTime.TotalSeconds % Colors.Count];
		}

		public static Edge RandomEdge ()
		{
			int i = r.Next () % 6;
			return i == 0 ? Left : i == 1 ? Right : i == 2 ? Up : i == 3 ? Down : i == 4 ? Forward : Backward;
		}

		#endregion
		
		#region Static Properties
		
		public static List<Color> Colors = new List<Color> (){
			Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Orange
		};
		public static Color DefaultColor = RandomColor ();

		public static Edge Zero { get { return new Edge (0, 0, 0); } }

		public static Edge UnitX { get { return new Edge (1, 0, 0); } }

		public static Edge UnitY { get { return new Edge (0, 1, 0); } }

		public static Edge UnitZ { get { return new Edge (0, 0, 1); } }

		public static Edge Up { get { return new Edge (0, 1, 0); } }

		public static Edge Down { get { return new Edge (0, -1, 0); } }

		public static Edge Right { get { return new Edge (1, 0, 0); } }

		public static Edge Left { get { return new Edge (-1, 0, 0); } }

		public static Edge Forward { get { return new Edge (0, 0, -1); } }

		public static Edge Backward { get { return new Edge (0, 0, 1); } }

		#endregion
	}

	public class EdgeList
	{
		#region Properties

		private WrapList<Edge> Edges;

		public WrapList<Edge> SelectedEdges { get; private set; }

		public Action LinesChanged { get; set; }

		#endregion

		#region Constructors

		public EdgeList ()
		{
			Edges = new WrapList<Edge> ();
			SelectedEdges = new WrapList<Edge> ();
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
			Node node = new Node (0, 0, 0);
			for (int i = 0; i < index; ++i) {
				node += Edges [i].Direction;
			}
			return node;
		}

		public Node FromNode (Edge edge)
		{
			return FromNode (IndexOf (edge));
		}

		public Node ToNode (int index)
		{
			Node node = new Node (0, 0, 0);
			for (int i = 0; i < index + 1; ++i) {
				node += Edges [i].Direction;
			}
			return node;
		}

		public Node ToNode (Edge edge)
		{
			return ToNode (IndexOf (edge));
		}

		public void Add (Edge edge)
		{
			Edges.Add (edge);
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
				VectorExtensions.Swap (ref indexLast, ref indexFirst);
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
			//Console.WriteLine ("Before Move => " + Edges);
			foreach (Edge selectedEdge in SelectedEdges) {
				Edges.Replace (selectedEdge, new Edge[] {
					new Edge (direction),
					selectedEdge,
					new Edge (-direction)
				}
				);
			}
            //Console.WriteLine ("After Move => " + Edges);
			Compact ();
            //Console.WriteLine ("Compact => " + Edges);
			LinesChanged ();
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

		#endregion
	}
}

