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
	public class PipeRenderer : GameClass, IKnotRenderer
	{
		public GameObjectInfo Info { get; private set; }

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

		public void Update (GameTime gameTime)
		{
			for (int i = 0; i < pipes.Count; ++i) {
				pipes [i].Update (gameTime);
			}
			for (int i = 0; i < knots.Count; ++i) {
				knots [i].Update (gameTime);
			}
		}

		public void OnEdgesChanged (EdgeList edges)
		{
			pipes.Clear ();
			for (int n = 0; n < edges.Count; n++) {
				PipeModelInfo info = new PipeModelInfo(edges, edges [n], Info.Position);
				PipeModel pipe = pipeFactory [state, info] as PipeModel;
				// pipe.OnDataChange = () => UpdatePipes (edges);
				pipes.Add (pipe);
			}

			knots.Clear ();
			for (int n = 0; n < edges.Count; n++) {
				NodeModelInfo info = new NodeModelInfo(edges, edges [n], edges [n + 1], Info.Position);
				NodeModel knot = knotFactory [state, info] as NodeModel;
				knots.Add (knot);
			}
		}

		#region Draw
		
		public void Draw (GameTime gameTime)
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

		public GameObjectDistance Intersects (Ray ray)
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

		public Vector3 Center ()
		{
			return Info.Position;
		}

		#endregion

		#region Selection

		public virtual void OnSelected (GameTime gameTime)
		{
		}

		public virtual void OnUnselected (GameTime gameTime)
		{
		}

		#endregion
	}
}

