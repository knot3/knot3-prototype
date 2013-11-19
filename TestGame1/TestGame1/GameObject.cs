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

namespace TestGame1
{
	public abstract class GameObject : GameClass
	{
		protected BasicEffect basicEffect;

		protected abstract Vector3 Position { get; set; }

		public bool IsMovable { get; set; }

		public bool IsVisible { get; set; }

		public GameObject (GameState state)
			: base(state)
		{
			basicEffect = new BasicEffect (device);
			IsMovable = false;
			IsVisible = true;
		}

		#region Move

		protected Plane CurrentGroundPlane ()
		{
			Plane groundPlane = new Plane (Position, Position + Vector3.Up,
							Position + Vector3.Normalize (Vector3.Cross (Vector3.Up, Position - camera.Position)));
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
			float previousLength = (Position - camera.Position).Length ();
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
			if (IsVisible && IsMovable && world.SelectedObject == this) {
				// is SelectedObjectMove the current input action?
				if (input.CurrentInputAction == InputAction.SelectedObjectMove) {
					Plane groundPlane = CurrentGroundPlane ();
					Ray ray = CurrentMouseRay ();
					Vector3? newPosition = CurrentMousePosition (ray, groundPlane);
					if (newPosition.HasValue) {
						Position = newPosition.Value;
					}
				}
			}
		}

		#endregion

		#region Draw

		public void Draw (GameTime gameTime)
		{
			if (IsVisible) {
				basicEffect.World = camera.WorldMatrix;
				basicEffect.View = camera.ViewMatrix;
				basicEffect.Projection = camera.ProjectionMatrix;
				DrawObject (gameTime);
			}
		}

		public abstract void DrawObject (GameTime gameTime);

		#endregion

		#region Textures and Models

		public static Texture2D LoadTexture (ContentManager content, string name)
		{
			try {
				return content.Load<Texture2D> (name);
			} catch (ContentLoadException ex) {
				Console.WriteLine (ex.ToString ());
				return null;
			}
		}

		private static Dictionary<string, Model> modelCache = new Dictionary<string, Model> ();

		public static Model LoadModel (ContentManager content, string name)
		{
			if (modelCache.ContainsKey (name)) {
				return modelCache [name];
			} else {
				try {
					Model model = content.Load<Model> (name);
					modelCache [name] = model;
					return model;
				} catch (ContentLoadException ex) {
					Console.WriteLine (ex.ToString ());
					return null;
				}
			}
		}

		#endregion

		#region Intersection

		public abstract GameObjectDistance Intersects (Ray ray);

		public abstract Vector3 Center ();

		#endregion

		#region Selection

		public bool IsSelected ()
		{
			return world.SelectedObject == this;
		}

		public virtual void OnSelected (GameTime gameTime)
		{
		}

		public virtual void OnUnselected (GameTime gameTime)
		{
		}

		#endregion
	}
}

