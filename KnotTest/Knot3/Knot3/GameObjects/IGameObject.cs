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

namespace Knot3.GameObjects
{
	public interface IGameObject
	{
		GameObjectInfo Info { get; }

		void OnSelected (GameTime gameTime);

		void OnUnselected (GameTime gameTime);

		void Draw (GameTime gameTime);

		void Update (GameTime gameTime);

		GameObjectDistance Intersects (Ray ray);

		Vector3 Center ();
	}

	public sealed class GameObjectDistance
	{
		public IGameObject Object;
		public float Distance;
	}
}

