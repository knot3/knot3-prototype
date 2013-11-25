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
	public class NodeModel : CachedGameModel
	{
		private Edge EdgeA;
		private Edge EdgeB;

		public NodeModel (GameState state, EdgeList edges, Edge edgeA, Edge edgeB, Vector3 position, float scale)
			: base(state, "knot1", position, scale)
		{
			EdgeA = edgeA;
			EdgeB = edgeB;
			IsVisible = edgeA.Direction != edgeB.Direction;
		}

		public override void DrawObject (GameTime gameTime)
		{
			BaseColor = EdgeA.Color.Mix (EdgeB.Color);

			base.DrawObject (gameTime);
		}
	}
}

