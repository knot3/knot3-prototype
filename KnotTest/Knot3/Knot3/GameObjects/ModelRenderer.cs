using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using Knot3.Core;
using Knot3.Utilities;
using Knot3.KnotData;
using Knot3.RenderEffects;
using System.Collections;

namespace Knot3.GameObjects
{
	/// <summary>
	/// Verwaltet eine Liste von Spielobjekten der Klassen PipeModel (Röhren) und NodeModel (Knotenpunkt)
	/// für ein gegebenes KnotData.Knot-Objekt und zeichnet diese über die World-Klasse.
	/// </summary>
	public sealed class ModelRenderer : IGameObject, IEnumerable<IGameObject>
	{
		private GameScreen screen;

		public GameObjectInfo Info { get; private set; }

		public World World { get; set; }

		// pipes, nodes and arrows
		private Knot knot;
		private List<PipeModel> pipes;
		private List<NodeModel> nodes;
		private List<ArrowModel> arrows;
		private ModelFactory pipeFactory;
		private ModelFactory nodeFactory;
		private ModelFactory arrowFactory;
		private NodeMap nodeMap = new NodeMap ();

		public ModelRenderer (GameScreen screen, GameObjectInfo info)
		{
			this.screen = screen;
			Info = info;
			pipes = new List<PipeModel> ();
			nodes = new List<NodeModel> ();
			arrows = new List<ArrowModel> ();
			pipeFactory = new ModelFactory ((s, i) => new PipeModel (s, i as PipeModelInfo));
			nodeFactory = new ModelFactory ((s, i) => new NodeModel (s, i as NodeModelInfo));
			arrowFactory = new ModelFactory ((s, i) => new ArrowModel (s, i as ArrowModelInfo));
		}

		public Knot Knot {
			set {
				knot = value;
				knot.EdgesChanged += OnEdgesChanged;
				knot.SelectionChanged += () => CreateArrows (knot.SelectedEdges);
				OnEdgesChanged();
			}
		}

		public void Update (GameTime time)
		{
			foreach (PipeModel pipe in pipes) {
				pipe.Update (time);
			}
			foreach (NodeModel node in nodes) {
				node.Update (time);
			}
			foreach (ArrowModel arrow in arrows) {
				arrow.Update (time);
			}
		}

		public void OnEdgesChanged ()
		{
			nodeMap.OnEdgesChanged (knot);

			pipes.Clear ();
			foreach (Edge edge in knot) {
				PipeModelInfo info = new PipeModelInfo (knot, nodeMap, edge, Info.Position);
				PipeModel pipe = pipeFactory [screen, info] as PipeModel;
				// pipe.OnDataChange = () => UpdatePipes (edges);
				pipe.World = World;
				pipes.Add (pipe);
			}

			nodes.Clear ();
			WrapList<Edge> edgeList = new WrapList<Edge>(knot);
			for (int n = 0; n < edgeList.Count; n++) {
				if (edgeList [n].Direction != edgeList [n + 1].Direction) {
					NodeModelInfo info = new NodeModelInfo (nodeMap, edgeList [n], edgeList [n + 1], Info.Position);
					NodeModel node = nodeFactory [screen, info] as NodeModel;
					node.World = World;
					nodes.Add (node);
				}
			}

			CreateArrows (knot.SelectedEdges);

			World.Redraw = true;
			Console.WriteLine ("Redraw=true <- PipeRenderer");
		}

		public void CreateArrows (IEnumerable<Edge> selectedEdges)
		{
			CreateArrows(new List<Edge>(selectedEdges));
		}

		private void CreateArrows (IList<Edge> selectedEdges)
		{
			arrows.Clear ();
			if (selectedEdges.Count > 0) {
				CreateArrow (selectedEdges [(int)selectedEdges.Count / 2]);
			}
		}

		private void CreateArrow (Edge edge)
		{
			Vector3[] validDirections = new Vector3[]{
				Vector3.Up, Vector3.Down, Vector3.Left, Vector3.Right, Vector3.Backward, Vector3.Forward
			};

			try {
				Node node1 = nodeMap.FromNode (edge);
				Node node2 = nodeMap.ToNode (edge);
				foreach (Vector3 direction in validDirections) {
					ArrowModelInfo info = new ArrowModelInfo (node1.CenterBetween (node2)-50*World.Camera.TargetDirection.PrimaryDirection(), direction, Info.Position);
					ArrowModel arrow = arrowFactory [screen, info] as ArrowModel;
					arrow.World = World;
					arrows.Add (arrow);
				}
			} catch (NullReferenceException ex) {
				Console.WriteLine (ex.ToString ());
			}
		}
		
		public IEnumerator<IGameObject> GetEnumerator ()
		{
			foreach (PipeModel pipe in pipes) {
				yield return pipe;
			}
			foreach (NodeModel node in nodes) {
				yield return node;
			}
			foreach (ArrowModel arrow in arrows) {
				yield return arrow;
			}
		}

		// Explicit interface implementation for nongeneric interface
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator (); // Just return the generic version
		}

		#region Draw
		
		public void Draw (GameTime time)
		{
			Overlay.Profiler ["# InFrustum"] = 0;
			Overlay.Profiler ["RenderEffect"] = 0;
			Overlay.Profiler ["Pipes"] = Knot3.Core.Game.Time (() => {
				foreach (PipeModel pipe in pipes) {
					pipe.Draw (time);
				}
			}
			).TotalMilliseconds;
			Overlay.Profiler ["Nodes"] = Knot3.Core.Game.Time (() => {
				foreach (NodeModel node in nodes) {
					node.Draw (time);
				}
			}
			).TotalMilliseconds;
			Overlay.Profiler ["Arrows"] = Knot3.Core.Game.Time (() => {
				foreach (ArrowModel arrow in arrows) {
					arrow.Draw (time);
				}
			}
			).TotalMilliseconds;
			Overlay.Profiler ["# Pipes"] = pipes.Count ();
			Overlay.Profiler ["# Nodes"] = nodes.Count ();
		}

		#endregion

		#region Intersection

		public GameObjectDistance Intersects (Ray ray)
		{
			GameObjectDistance nearest = null;
			if (!screen.input.GrabMouseMovement) {
				foreach (PipeModel pipe in pipes) {
					GameObjectDistance intersection = pipe.Intersects (ray);
					if (intersection != null) {
						if (intersection.Distance > 0 && (nearest == null || intersection.Distance < nearest.Distance)) {
							nearest = intersection;
						}
					}
				}
				foreach (ArrowModel arrow in arrows) {
					GameObjectDistance intersection = arrow.Intersects (ray);
					if (intersection != null) {
						if (intersection.Distance > 0 && (nearest == null || intersection.Distance < nearest.Distance)) {
							nearest = intersection;
						}
					}
				}
			}
			return nearest;
		}

		public Vector3 Center ()
		{
			return Info.Position;
		}

		#endregion
	}
}

