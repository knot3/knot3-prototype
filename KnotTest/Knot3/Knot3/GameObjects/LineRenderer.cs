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
using Knot3.KnotData;

namespace Knot3.GameObjects
{
	/// <summary>
	/// Renders a Knot using primitive lines.
	/// </summary>
	public class LineRenderer : GameStateClass, IEdgeChangeReceiver, IGameObject
	{
		public dynamic Info { get; private set; }

		// graphic stuff
		private BasicEffect basicEffect;

		// edges
		private EdgeList edges;

		public LineRenderer (GameState state, GameObjectInfo info)
			: base(state)
		{
			Info = info;
			basicEffect = new BasicEffect (device);
		}

		public void Update (GameTime gameTime)
		{
		}

		public void OnEdgesChanged (EdgeList edges)
		{
			this.edges = edges;
		}

		#region Draw

		public void Draw (GameTime gameTime)
		{
			basicEffect.World = camera.WorldMatrix;
			basicEffect.View = camera.ViewMatrix;
			basicEffect.Projection = camera.ProjectionMatrix;

			if (edges.Count > 0) {
				DrawRoundedLines ();
			}
		}

		private void DrawRoundedLines ()
		{
			Vector3 offset = Vector3.Zero; //new Vector3 (10, 10, 10);

			var vertices = new VertexPositionColor[edges.Count * 4];

			Vector3 last = new Vector3 (0, 0, 0);
			for (int n = 0; n < edges.Count; n++) {
				Vector3 p1 = edges.FromNode (n).Vector () + offset;
				Vector3 p2 = edges.ToNode (n).Vector () + offset;

				var diff = p1 - p2;
				diff.Normalize ();
				p1 = p1 - 10 * diff;
				p2 = p2 + 10 * diff;

				vertices [4 * n + 0].Position = n == 0 ? p1 : last;
				vertices [4 * n + 1].Position = p1;
				vertices [4 * n + 2].Position = p1;
				vertices [4 * n + 3].Position = p2;

				//Console.WriteLine (vertices [4 * n + 2]);
				last = p2;
			}
			for (int n = 0; n < edges.Count; n++) {
				vertices [4 * n + 0].Color = Color.Black;
				vertices [4 * n + 1].Color = Color.Black;
				vertices [4 * n + 2].Color = Color.Black;
				vertices [4 * n + 3].Color = Color.Black;
			}
			basicEffect.CurrentTechnique.Passes [0].Apply ();
			device.DrawUserPrimitives (PrimitiveType.LineList, vertices, 0, edges.Count * 2); 
		}

		#endregion

		#region Intersection

		public GameObjectDistance Intersects (Ray ray)
		{
			return null;
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

