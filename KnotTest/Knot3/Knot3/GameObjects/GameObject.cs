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

namespace Knot3.GameObjects
{
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

	public abstract class GameObject : GameClass, IGameObject
	{
		public GameObjectInfo Info { get; private set; }

		public GameObject (GameState state, GameObjectInfo info)
			: base(state)
		{
			Info = info;
		}

		#region Move

		protected Plane CurrentGroundPlane ()
		{
			Plane groundPlane = new Plane (
				Info.Position, Info.Position + Vector3.Up,
				Info.Position + Vector3.Normalize (Vector3.Cross (Vector3.Up, Info.Position - camera.Position))
			);
			Console.WriteLine ("groundPlane=" + groundPlane);
			return groundPlane;
		}

		protected Ray CurrentMouseRay ()
		{
			Ray ray = camera.GetMouseRay (Input.MouseState.ToVector2 ());
			return ray;
		}

		protected Vector3? CurrentMousePosition (Ray ray, Plane groundPlane)
		{
			float? planeDistance = ray.Intersects (groundPlane);
			float previousLength = (Info.Position - camera.Position).Length ();
			if (planeDistance.HasValue) {
				Vector3 planePosition = ray.Position + ray.Direction * planeDistance.Value;
				float currentLength = (planePosition - camera.Position).Length ();
				return camera.Position + (planePosition - camera.Position) * previousLength / currentLength;
			} else {
				return null;
			}
		}

		public virtual void Update (GameTime gameTime)
		{
			// check whether is object is movable and whether it is selected
			if (Info.IsVisible && Info.IsMovable && world.SelectedObject == this) {
				// is SelectedObjectMove the current input action?
				if (input.CurrentInputAction == InputAction.SelectedObjectMove) {
					Plane groundPlane = CurrentGroundPlane ();
					Ray ray = CurrentMouseRay ();
					Vector3? newPosition = CurrentMousePosition (ray, groundPlane);
					if (newPosition.HasValue) {
						Info.Position = newPosition.Value;
					}
				}
			}
		}

		#endregion

		#region Draw

		public abstract void Draw (GameTime gameTime);

		#endregion

		#region Intersection

		public abstract GameObjectDistance Intersects (Ray ray);

		public abstract Vector3 Center ();

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

