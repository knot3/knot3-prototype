using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Knot3.Utilities;

namespace Knot3.KnotData
{
	public class KnotMetaData
	{
		private string name;
		public Func<int> CountEdges;
		public IKnotIO Format;
		public string Filename;

		public KnotMetaData (string name, Func<int> countEdges, IKnotIO format, string filename)
		{
			this.name = name;
			CountEdges = countEdges;
			Format = format;
			Filename = filename;
		}

		public string Name {
			get { return name; }
			set {
				name = value;
				Filename = Files.SavegameDirectory + Files.Separator + FileUtility.ConvertToFilename (name) + ".knot";
			}
		}

		public override string ToString ()
		{
			return "{Name=" + Name + ",CountEdges=" + CountEdges + ",Format=" + Format + ",Filename=" + Filename + "}";
		}
	}

	public class Knot : IEnumerable<Edge>, ICloneable
	{
		public string Name {
			get { return MetaData.Name; }
			set { MetaData.Name = value; }
		}

		public KnotMetaData MetaData { get; private set; }

		public IEnumerable<Edge> SelectedEdges { get { return selectedEdges; } }

		private Circle<Edge> edges;
		private List<Edge> selectedEdges;

		// events
		public Action EdgesChanged;
		public Action SelectionChanged;

		public Knot ()
		{
			MetaData = new KnotMetaData ("", () => edges.Count, null, null);
			edges = new Circle<Edge> (new Edge[]{
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
			selectedEdges = new List<Edge> ();
		}
		
		public Knot (KnotMetaData metaData, IEnumerable<Edge> edges)
		{
			MetaData = metaData;
			MetaData.CountEdges = () => this.edges.Count;
			this.edges = new Circle<Edge> (edges);
			selectedEdges = new List<Edge> ();
		}
		
		public object Clone ()
		{
			Circle<Edge> newCircle = new Circle<Edge> (edges as IEnumerable<Edge>);
			return new Knot {
				MetaData = new KnotMetaData(
					MetaData.Name,
					() => newCircle.Count,
					MetaData.Format,
					MetaData.Filename
				),
				edges = newCircle,
				selectedEdges = new List<Edge>(selectedEdges),
				EdgesChanged = EdgesChanged,
				SelectionChanged = SelectionChanged,
			};
		}

		public void Move (Direction direction, int distance = 1)
		{
			HashSet<Edge> selected = new HashSet<Edge> (selectedEdges);

			Circle<Edge> current = edges;
			do {
				if (!selected.Contains (current.Previous.Content) && selected.Contains (current.Content)) {
					for (int i = 0; i < distance; ++i)
						current.InsertBefore (new Edge (direction));
				}
				if (selected.Contains (current.Content) && !selected.Contains (current.Next.Content)) {
					for (int i = 0; i < distance; ++i)
						current.InsertAfter (new Edge (direction.ReverseDirection ()));
				}

				current = current.Next;
			} while (current != edges);

			current = edges;
			do {
				if (current != current.Previous.Previous && current != current.Previous.Previous
					&& current.Previous.Content.Direction == current.Previous.Previous.Content.Direction.ReverseDirection ()) {
					current.Previous.Previous.Remove ();
					current.Previous.Remove ();
				} else {
					current = current.Next;
				}
			} while (current != edges);

			EdgesChanged ();
		}

		public IEnumerator<Edge> GetEnumerator ()
		{
			return edges.GetEnumerator ();
		}

		// explicit interface implementation for nongeneric interface
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator (); // just return the generic version
		}

		public void Save (IKnotIO format, string filename)
		{
			MetaData.Filename = filename;
			format.Save (this);
		}

		public void Save ()
		{
			if (MetaData.Format == null)
				throw new IOException ("Error: Knot: MetaData.Format is null!");
			else if (MetaData.Filename == null)
				throw new IOException ("Error: Knot: MetaData.Filename is null!");
			else
				MetaData.Format.Save (this);
		}

		private Circle<Edge> lastSelected;

		public void AddRangeToSelection (Edge selectedEdge)
		{
			Circle<Edge> selectedCircle = null;
			if (edges.Contains (selectedEdge, out selectedCircle)) {
				List<Edge> forward = new List<Edge> (lastSelected.RangeTo (selectedCircle));
				List<Edge> backward = new List<Edge> (selectedCircle.RangeTo (lastSelected));

				if (forward.Count < backward.Count) {
					foreach (Edge e in forward) {
						if (!selectedEdges.Contains (e))
							selectedEdges.Add (e);
					}
				} else {
					foreach (Edge e in backward) {
						if (!selectedEdges.Contains (e))
							selectedEdges.Add (e);
					}
				}
				lastSelected = selectedCircle;
			}
			SelectionChanged ();
		}

		public void AddToSelection (Edge edge)
		{
			if (!selectedEdges.Contains (edge))
				selectedEdges.Add (edge);
			lastSelected = edges.Find (edge);
			SelectionChanged ();
		}

		public void ClearSelection ()
		{
			selectedEdges.Clear ();
			lastSelected = null;
			SelectionChanged ();
		}

		public bool IsSelected (Edge edge)
		{
			return selectedEdges.Contains (edge);
		}

		public override string ToString ()
		{
			return "Knot(name=" + Name + ",#edgecount=" + edges.Count
				+ ",format=" + (MetaData.Format != null ? MetaData.ToString () : "null")
				+ ")";
		}
	}
}

