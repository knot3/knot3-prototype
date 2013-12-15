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
	public class ModelRenderer : KnotRenderer, IEnumerable<IGameObject>
	{
		public override GameObjectInfo Info { get; protected set; }

		public override World World { get; set; }

		// pipes and knots
		private List<PipeModel> pipes;
		private List<NodeModel> nodes;
		private List<ArrowModel> arrows;
		private ModelFactory pipeFactory;
		private ModelFactory nodeFactory;
		private ModelFactory arrowFactory;

		public ModelRenderer (GameScreen state, GameObjectInfo info)
			: base(state)
		{
			Info = info;
			pipes = new List<PipeModel> ();
			nodes = new List<NodeModel> ();
			arrows = new List<ArrowModel> ();
			pipeFactory = new ModelFactory ((s, i) => new PipeModel (s, i as PipeModelInfo));
			nodeFactory = new ModelFactory ((s, i) => new NodeModel (s, i as NodeModelInfo));
			arrowFactory = new ModelFactory ((s, i) => new ArrowModel (s, i as ArrowModelInfo));
		}

		public override void Update (GameTime gameTime)
		{
			foreach (PipeModel pipe in pipes) {
				pipe.Update (gameTime);
			}
			foreach (NodeModel node in nodes) {
				node.Update (gameTime);
			}
			foreach (ArrowModel arrow in arrows) {
				arrow.Update (gameTime);
			}
		}

		public override void OnEdgesChanged (EdgeList edgeList)
		{
			base.OnEdgesChanged (edgeList);

			pipes.Clear ();
			foreach (Edge edge in edgeList) {
				PipeModelInfo info = new PipeModelInfo (edgeList, nodeMap, edge, Info.Position);
				PipeModel pipe = pipeFactory [state, info] as PipeModel;
				// pipe.OnDataChange = () => UpdatePipes (edges);
				pipe.World = World;
				pipes.Add (pipe);
			}

			nodes.Clear ();
			for (int n = 0; n < edgeList.Count; n++) {
				if (edgeList [n].Direction != edgeList [n + 1].Direction) {
					NodeModelInfo info = new NodeModelInfo (edgeList, nodeMap, edgeList [n], edgeList [n + 1], Info.Position);
					NodeModel node = nodeFactory [state, info] as NodeModel;
					node.World = World;
					nodes.Add (node);
				}
			}

			CreateArrows (edgeList.SelectedEdges);

			World.Redraw = true;
			Console.WriteLine ("Redraw=true <- PipeRenderer");
		}

		public void CreateArrows (WrapList<Edge> selectedEdges)
		{
			arrows.Clear ();
			//foreach (Edge edge in selectedEdges) {
			//	CreateArrow (edge);
			//}
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
					ArrowModelInfo info = new ArrowModelInfo (node1.CenterBetween (node2), direction, Info.Position);
					ArrowModel arrow = arrowFactory [state, info] as ArrowModel;
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
		
		public override void Draw (GameTime gameTime)
		{
			Overlay.Profiler ["# InFrustum"] = 0;
			Overlay.Profiler ["RenderEffect"] = 0;
			Overlay.Profiler ["Pipes"] = Knot3.Core.Game.Time (() => {
				foreach (PipeModel pipe in pipes) {
					pipe.Draw (gameTime);
				}
			}
			).TotalMilliseconds;
			Overlay.Profiler ["Nodes"] = Knot3.Core.Game.Time (() => {
				foreach (NodeModel node in nodes) {
					node.Draw (gameTime);
				}
			}
			).TotalMilliseconds;
			Overlay.Profiler ["Arrows"] = Knot3.Core.Game.Time (() => {
				foreach (ArrowModel arrow in arrows) {
					arrow.Draw (gameTime);
				}
			}
			).TotalMilliseconds;
			Overlay.Profiler ["# Pipes"] = pipes.Count ();
			Overlay.Profiler ["# Nodes"] = nodes.Count ();
		}

		#endregion

		#region Intersection

		public override GameObjectDistance Intersects (Ray ray)
		{
			GameObjectDistance nearest = null;
			if (!state.input.GrabMouseMovement) {
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

		public override Vector3 Center ()
		{
			return Info.Position;
		}

		#endregion
	}
}

