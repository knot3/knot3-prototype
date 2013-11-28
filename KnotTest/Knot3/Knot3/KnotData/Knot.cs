using System;
using Knot3.Utilities;

namespace Knot3.KnotData
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

	/// <summary>
	/// Ein Knoten, der aus einer Liste von Kanten (EdgeList) besteht und ein Knotenformat referenziert (IKnotFormat).
	/// </summary>
	public class Knot
	{
		#region Properties

		public KnotInfo Info { get; private set; }

		public EdgeList Edges;
		private IKnotFormat Format;

		public Action<EdgeList> EdgesChanged {
			set { Edges.EdgesChanged = value; }
			get { return Edges.EdgesChanged; }
		}

		#endregion

		#region Constructors

		public Knot (KnotInfo info, IKnotFormat format, EdgeList edges = null)
		{
			info.EdgeCount = () => this.Edges.Count;
			Info = info;
			Edges = edges != null ? edges : new EdgeList ();
			Format = format;
		}

		#endregion

		#region Public Methods

		public void Rename (string name)
		{
			if (name.Length > 0 && name != Info.Name) {
				KnotInfo info = Info;
				info.Name = name;
				info.Filename = Format.FindFilename(name);
				Info = info;
			}
		}

		public void Save() {
			Format.SaveKnot(this);
		}

		#endregion

		#region Default Knots

		public static Knot RandomKnot (int count, IKnotFormat format)
		{
			EdgeList edges = new EdgeList ();
			for (int i = 0; i < 30; ++i) {
				edges.Add (Edge.RandomEdge ());
			}
			edges.Compact ();
			for (int i = 0; i < edges.Count; ++i) {
				edges [i].Color = Edge.RandomColor ();
			}

			return UntitledKnot (edges, format);
		}

		public static Knot DefaultKnot (IKnotFormat format)
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

			return UntitledKnot (edges, format);
		}

		private static Knot UntitledKnot (EdgeList edges, IKnotFormat format)
		{
			int num = new Random ().Next () % 1000;
			string name = "Untitled #" + num;
			Knot knot = new Knot (new KnotInfo {
				Filename = format.FindFilename(name),
				Name = name,
				IsValid = true
			}, format, edges);
			return knot;
		}

		#endregion
	}
}

