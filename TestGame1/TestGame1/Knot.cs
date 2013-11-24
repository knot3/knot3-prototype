using System;

namespace TestGame1
{
	public struct KnotInfo
	{
		public string Filename;
		public string Name;
		public Func<int> EdgeCount;
		public bool IsValid;

		public override string ToString ()
		{
			return "{Filename=" + Filename + ",Name=" + Name + ",EdgeCount=" + EdgeCount () + ",IsValid=" + IsValid + "}";
		}
	}

	public class Knot
	{
		#region Properties

		public KnotInfo Info { get; private set; }

		public EdgeList Edges;
		public Action<Knot> Save;

		public Action<EdgeList> EdgesChanged {
			set { Edges.EdgesChanged = value; }
			get { return Edges.EdgesChanged; }
		}

		#endregion

		#region Constructors

		public Knot (KnotInfo info, Action<Knot> save, EdgeList edges = null)
		{
			info.EdgeCount = () => this.Edges.Count;
			Info = info;
			Edges = edges != null ? edges : new EdgeList ();
			Save = save;
		}

		#endregion

		public static Knot RandomKnot (int count, Action<Knot> save)
		{
			EdgeList edges = new EdgeList ();
			for (int i = 0; i < 30; ++i) {
				edges.Add (Edge.RandomEdge ());
			}
			edges.Compact ();
			for (int i = 0; i < edges.Count; ++i) {
				edges [i].Color = Edge.RandomColor ();
			}

			return UntitledKnot (edges, save);
		}

		public static Knot DefaultKnot (Action<Knot> save)
		{
			// add some default nodes
			EdgeList edges = new EdgeList ();

			edges.AddRange (new[] {
				Edge.Up,
				Edge.Right,
				Edge.Down,
				Edge.Backward,
				Edge.Up,
				Edge.Left,
				Edge.Down,
				Edge.Forward
			}
			);
			edges.Compact ();
			for (int i = 0; i < edges.Count; ++i) {
				edges [i].Color = Edge.RandomColor ();
			}

			return UntitledKnot (edges, save);
		}

		private static Knot UntitledKnot (EdgeList edges, Action<Knot> save)
		{
			int num = new Random ().Next () % 1000;
			Knot knot = new Knot (new KnotInfo {
				Filename = Files.SavegameDirectory+Files.Separator+"untitled-"+num+".knot",
				Name = "Untitled #" + num,
				IsValid = true
			}, save, edges);
			return knot;
		}
	}
}

