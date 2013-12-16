using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Knot3.Utilities;

namespace Knot3.KnotData
{
	public struct KnotMetaData
	{
		public IKnotIO File;
		public string Name;
		public int CountEdges;

		public KnotMetaData (string name, int countEdges, IKnotIO file)
		{
			Name = name;
			CountEdges = countEdges;
			File = file;
		}

		public KnotMetaData (IKnotIO file)
		{
			Name = file.Name;
			CountEdges = file.CountEdges;
			File = file;
		}

		public override string ToString ()
		{
			return "{File=" + File + ",Name=" + Name + ",CountEdges=" + CountEdges + "}";
		}
	}

	public class Knot : IEnumerable<Edge>, ICloneable
	{
		public string Name { get; set; }

		public IEnumerable<Edge> SelectedEdges { get { return selectedEdges; } }

		private Circle<Edge> edges;
		private IKnotIO file;
		private List<Edge> selectedEdges;

		// events
		public Action EdgesChanged;
		public Action SelectionChanged;

		public Knot ()
		{
			Name = "Untitled";
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
		
		public Knot (IKnotIO _file)
		{
			Name = _file.Name;
			file = _file;
			edges = new Circle<Edge> (file.Edges);
			selectedEdges = new List<Edge> ();
		}
		
		public Knot (KnotMetaData metaData)
			: this (metaData.File)
		{
		}
		
		public object Clone ()
		{
			return new Knot {
				Name = Name,
				file = file,
				edges = new Circle<Edge> (edges as IEnumerable<Edge>),
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

		public void Save (IKnotIO file)
		{
			file.Save (this);
		}

		public void Save ()
		{
			if (file != null)
				file.Save (this);
			else
				throw new IOException ("Error: IKnotIO file is null!");
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
			return "name=" + Name + ",#edgecount=" + edges.Count + ",file=" + (file != null ? file.ToString () : "null");
		}
	}
}

