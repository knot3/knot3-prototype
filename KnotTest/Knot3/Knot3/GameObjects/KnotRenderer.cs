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
	public abstract class KnotRenderer: IEdgeChangeListener, IGameObject
	{
		protected GameScreen state;

		public abstract World World { get; set; }

		public abstract GameObjectInfo Info { get; protected set; }

		protected NodeMap nodeMap = new NodeMap ();
		
		public KnotRenderer (GameScreen state)
		{
			this.state = state;
		}

		public virtual void OnEdgesChanged (EdgeList edges)
		{
			nodeMap.OnEdgesChanged(edges);
		}

		public abstract void Update (GameTime gameTime);

		public abstract void Draw (GameTime gameTime);

		public abstract GameObjectDistance Intersects (Ray ray);

		public abstract Vector3 Center ();
	}
}

