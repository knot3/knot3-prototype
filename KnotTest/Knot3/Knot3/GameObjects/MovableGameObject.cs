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

namespace Knot3.GameObjects
{
	/// <summary>
	/// Ein Decorator für ein IGameObject, das ein freies Bewegen dieses Objekts im dreidimensionalen Raum ermöglicht.
	/// </summary>
	public class MovableGameObject : IGameObject
	{
		private GameScreen screen;
		private IGameObject Obj;

		public World World {
			get { return Obj.World; }
			set {}
		}

		public MovableGameObject (GameScreen screen, IGameObject obj)
		{
 			this.screen = screen;
			Obj = obj;
			Obj.Info.IsMovable = true;
		}

		public GameObjectInfo Info {
			get {
				return Obj.Info;
			}
		}

		#region Move

		protected Plane CurrentGroundPlane ()
		{
			Plane groundPlane = new Plane (
				Info.Position, Info.Position + Vector3.Up,
				Info.Position + Vector3.Normalize (Vector3.Cross (Vector3.Up, Info.Position - World.Camera.Position))
			);
			//Console.WriteLine ("groundPlane=" + groundPlane);
			return groundPlane;
		}

		protected Ray CurrentMouseRay ()
		{
			Ray ray = World.Camera.GetMouseRay (InputManager.MouseState.ToVector2 ());
			return ray;
		}

		protected Vector3? CurrentMousePosition (Ray ray, Plane groundPlane)
		{
			float? planeDistance = ray.Intersects (groundPlane);
			float previousLength = (Info.Position - World.Camera.Position).Length ();
			if (planeDistance.HasValue) {
				Vector3 planePosition = ray.Position + ray.Direction * planeDistance.Value;
				float currentLength = (planePosition - World.Camera.Position).Length ();
				return World.Camera.Position + (planePosition - World.Camera.Position) * previousLength / currentLength;
			} else {
				return null;
			}
		}

		public virtual void Update (GameTime time)
		{
			// check whether is object is movable and whether it is selected
			bool isSelected = World.SelectedObject == this || World.SelectedObject == Obj;
			if (Info.IsVisible && Info.IsMovable && isSelected) {
				// is SelectedObjectMove the current input action?
				if (screen.input.CurrentInputAction == InputAction.SelectedObjectMove) {
					Plane groundPlane = CurrentGroundPlane ();
					Ray ray = CurrentMouseRay ();
					Vector3? newPosition = CurrentMousePosition (ray, groundPlane);
					if (newPosition.HasValue) {
						Info.Position = newPosition.Value;
					}
				}
			}

			Obj.Update (time);
		}

		#endregion

		#region Draw

		public void Draw (GameTime time)
		{
			Obj.Draw (time);
		}

		#endregion

		#region Intersection

		public GameObjectDistance Intersects (Ray ray)
		{
			return Obj.Intersects (ray);
		}

		public Vector3 Center ()
		{
			return Obj.Center ();
		}

		#endregion
	}
}

