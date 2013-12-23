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
using Knot3.Core;
using System.Collections;

namespace Knot3.GameObjects
{
	public class ShadowGameObject : IGameObject
	{
		// game screen
		protected GameScreen screen;

		// the decorated object
		private IGameObject Obj;

		// game world
		public World World {
			get { return Obj.World; }
			set {}
		}

		// info
		public GameObjectInfo Info { get; private set; }

		public ShadowGameObject (GameScreen screen, IGameObject obj)
		{
 this.screen = screen;
			Info = new GameObjectInfo ();
			Obj = obj;
			Info.IsVisible = true;
			Info.IsSelectable = false;
			Info.IsMovable = false;
		}

		public Vector3 ShadowPosition {
			get {
				return Info.Position;
			}
			set {
				Info.Position = value;
			}
		}

		public Vector3 OriginalPosition {
			get {
				return Obj.Info.Position;
			}
		}

		#region Update

		public virtual void Update (GameTime time)
		{
			Info.IsVisible = Math.Abs ((ShadowPosition - Obj.Info.Position).Length ()) > 50;
		}

		#endregion

		#region Draw

		public virtual void Draw (GameTime time)
		{
			Vector3 originalPositon = Obj.Info.Position;
			Obj.Info.Position = ShadowPosition;
			Obj.Draw (time);
			Obj.Info.Position = originalPositon;
		}

		#endregion

		#region Intersection

		public GameObjectDistance Intersects (Ray ray)
		{
			return null;
		}

		public Vector3 Center ()
		{
			return Obj.Center ();
		}

		#endregion
	}
	
}
