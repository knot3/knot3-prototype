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

using Knot3.Utilities;
using Knot3.KnotData;
using Knot3.RenderEffects;

namespace Knot3.GameObjects
{
	/// <summary>
	/// Renders a knot using 3D models (pipes and nodes).
	/// </summary>
	public class PipeRenderer : KnotRenderer
	{
		// pipes and knots
		private List<PipeModel> pipes;
		private List<NodeModel> knots;
		private PipeModelCache pipeCache;
		private NodeModelCache knotCache;

		protected override Vector3 Position { get; set; }

		public PipeRenderer (GameState state)
			: base(state)
		{
			pipes = new List<PipeModel> ();
			knots = new List<NodeModel> ();
			pipeCache = new PipeModelCache (state);
			knotCache = new NodeModelCache (state);
			Position = Vector3.Zero; //new Vector3 (10, 10, 10);
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
				PipeModel pipe = pipeCache [edges, edges [n], Position];
				// pipe.OnDataChange = () => UpdatePipes (edges);
				pipes.Add (pipe);
			}

			knots.Clear ();
			for (int n = 0; n < edges.Count; n++) {
				NodeModel knot = knotCache [edges, edges [n], edges [n + 1], Position];
				knots.Add (knot);
			}
		}
		
		public override void DrawObject (GameTime gameTime)
		{
			foreach (PipeModel pipe in pipes) {
				pipe.Draw (gameTime);
			}
			foreach (NodeModel knot in knots) {
				knot.Draw (gameTime);
			}
		}

		public override GameObjectDistance Intersects (Ray ray)
		{
			GameObjectDistance nearest = null;
			if (!input.GrabMouseMovement) {
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
			return Position;
		}
	}
}

