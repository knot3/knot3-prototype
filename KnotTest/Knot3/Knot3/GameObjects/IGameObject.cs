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

namespace Knot3.GameObjects
{
	public interface IGameObject : IGameStateClass
	{
		dynamic Info { get; }

		void OnSelected (GameTime gameTime);

		void OnUnselected (GameTime gameTime);

		void Draw (GameTime gameTime);

		void Update (GameTime gameTime);

		GameObjectDistance Intersects (Ray ray);

		Vector3 Center ();
	}

	public class GameObjectInfo : IEquatable<GameObjectInfo>
	{
		public Vector3 Position;
		public bool IsVisible;
		public bool IsMovable;

		public GameObjectInfo ()
		{
			Position = Vector3.Zero;
			IsVisible = true;
			IsMovable = false;
		}

		public virtual bool Equals (GameObjectInfo other)
		{
			if (other == null) 
				return false;

			if (this.Position == other.Position)
				return true;
			else
				return false;
		}

		public override bool Equals (Object obj)
		{
			if (obj == null) 
				return false;

			GameObjectInfo personObj = obj as GameObjectInfo;
			if (personObj == null)
				return false;
			else   
				return Equals (personObj);   
		}

		public override int GetHashCode ()
		{
			return this.Position.GetHashCode ();
		}

		public static bool operator == (GameObjectInfo o1, GameObjectInfo o2)
		{
			if ((object)o1 == null || ((object)o2) == null)
				return Object.Equals (o1, o2);

			return o2.Equals (o2);
		}

		public static bool operator != (GameObjectInfo o1, GameObjectInfo o2)
		{
			return ! (o1 == o2);
		}
	}

	public sealed class GameObjectDistance
	{
		public IGameObject Object;
		public float Distance;
	}
}

