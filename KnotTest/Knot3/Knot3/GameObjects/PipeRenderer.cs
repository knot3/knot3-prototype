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
	public class PipeRenderer : KnotRenderer, IEnumerable<GameModel>
	{
		public override dynamic Info { get; protected set; }

		// pipes and knots
		private List<PipeModel> pipes;
		private List<NodeModel> knots;
		private PipeModelFactory pipeFactory;
		private NodeModelFactory knotFactory;

		public PipeRenderer (GameState state, GameObjectInfo info)
			: base(state)
		{
			Info = info;
			pipes = new List<PipeModel> ();
			knots = new List<NodeModel> ();
			pipeFactory = new PipeModelFactory ();
			knotFactory = new NodeModelFactory ();
		}

		public override void Update (GameTime gameTime)
		{
			for (int i = 0; i < pipes.Count; ++i) {
				pipes [i].Update (gameTime);
			}
			for (int i = 0; i < knots.Count; ++i) {
				knots [i].Update (gameTime);
			}
		}

		public override void OnEdgesChanged (EdgeList edges)
		{
			pipes.Clear ();
			for (int n = 0; n < edges.Count; n++) {
				PipeModelInfo info = new PipeModelInfo (edges, edges [n], Info.Position);
				PipeModel pipe = pipeFactory [state, info] as PipeModel;
				// pipe.OnDataChange = () => UpdatePipes (edges);
				pipes.Add (pipe);
			}

			knots.Clear ();
			for (int n = 0; n < edges.Count; n++) {
				NodeModelInfo info = new NodeModelInfo (edges, edges [n], edges [n + 1], Info.Position);
				NodeModel knot = knotFactory [state, info] as NodeModel;
				knots.Add (knot);
			}
		}
		
		public IEnumerator<GameModel> GetEnumerator ()
		{
			foreach (PipeModel pipe in pipes) {
				yield return pipe;
			}
			foreach (NodeModel knot in knots) {
				yield return knot;
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
			foreach (PipeModel pipe in pipes) {
				pipe.Draw (gameTime);
			}
			foreach (NodeModel knot in knots) {
				knot.Draw (gameTime);
			}
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

