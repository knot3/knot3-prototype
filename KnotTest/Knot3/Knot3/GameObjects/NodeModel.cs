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

namespace Knot3.GameObjects
{
	public class NodeModelInfo : GameModelInfo
	{
		public EdgeList Edges;
		public Edge EdgeA;
		public Edge EdgeB;

		public NodeModelInfo (EdgeList edges, Edge edgeA, Edge edgeB, Vector3 offset)
			: base("knot1")
		{
			Edges = edges;
			EdgeA = edgeA;
			EdgeB = edgeB;
			IsVisible = edgeA.Direction != edgeB.Direction;
			Position = edges.ToNode (edgeA).Vector ();
			Scale = 5f;
		}

		public override bool Equals (GameObjectInfo other)
		{
			if (other == null) 
				return false;

			if (other is NodeModelInfo) {
				if (this.EdgeA == (other as NodeModelInfo).EdgeA && this.EdgeB == (other as NodeModelInfo).EdgeB && base.Equals (other))
					return true;
				else
					return false;
			} else {
				return base.Equals (other);
			}
		}
	}

	public class NodeModel : GameModel
	{
		#region Attributes and Properties

		public new NodeModelInfo Info { get; private set; }

		#endregion

		public NodeModel (GameState state, NodeModelInfo info)
			: base(state, info)
		{
			Info = info;
		}

		public override void Draw (GameTime gameTime)
		{
			BaseColor = Info.EdgeA.Color.Mix (Info.EdgeB.Color);

			base.Draw (gameTime);
		}
	}

	public class NodeModelFactory : ModelFactory
	{
		protected override GameModel CreateModel (GameState state, GameModelInfo info)
		{
			return new NodeModel (state, info as NodeModelInfo);
		}
	}
}

