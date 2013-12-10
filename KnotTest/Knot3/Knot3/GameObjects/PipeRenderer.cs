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
	public class PipeRenderer : KnotRenderer, IEnumerable<IGameObject>
	{
		public override dynamic Info { get; protected set; }

		public override World World { get; set; }

		// pipes and knots
		private List<PipeModel> pipes;
		private List<NodeModel> nodes;
		private ModelFactory pipeFactory;
		private ModelFactory nodeFactory;

		public PipeRenderer (GameState state, GameObjectInfo info)
			: base(state)
		{
			Info = info;
			pipes = new List<PipeModel> ();
			nodes = new List<NodeModel> ();
			pipeFactory = new ModelFactory ((s, i) => new PipeModel (s, i as PipeModelInfo));
			nodeFactory = new ModelFactory ((s, i) => new NodeModel (s, i as NodeModelInfo));
		}

		public override void Update (GameTime gameTime)
		{
			for (int i = 0; i < pipes.Count; ++i) {
				pipes [i].Update (gameTime);
			}
			for (int i = 0; i < nodes.Count; ++i) {
				nodes [i].Update (gameTime);
			}
		}

		public override void OnEdgesChanged (EdgeList edges)
		{
			pipes.Clear ();
			for (int n = 0; n < edges.Count; n++) {
				PipeModelInfo info = new PipeModelInfo (edges, edges [n], Info.Position);
				PipeModel pipe = pipeFactory [state, info] as PipeModel;
				// pipe.OnDataChange = () => UpdatePipes (edges);
				pipe.World = World;
				pipes.Add (pipe);
			}

			nodes.Clear ();
			for (int n = 0; n < edges.Count; n++) {
				if (edges [n].Direction != edges [n + 1].Direction) {
					NodeModelInfo info = new NodeModelInfo (edges, edges [n], edges [n + 1], Info.Position);
					NodeModel node = nodeFactory [state, info] as NodeModel;
					node.World = World;
					nodes.Add (node);
				}
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
		}

		// Explicit interface implementation for nongeneric interface
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator (); // Just return the generic version
		}

		#region Draw
		
		public override void Draw (GameTime gameTime)
		{
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

