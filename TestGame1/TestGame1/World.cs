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
	public class World : GameClass
	{
		// game objects
		private List<GameObject> objects;

		public GameObject SelectedObject { get; private set; }

		private TexturedRectangle floor;

		// world data
		private Vector3 position;
		private Vector3 size;
		private double lastRayCheck = 0;

		/// <summary>
		/// Initializes a new Overlay
		/// </summary>
		public World (GameState state)
			: base(state)
		{
			size = new Vector3 (2000, 1000, 2000);
			position = new Vector3 (-1000, -100, -1000);

			objects = new List<GameObject> ();

			// some game objects

			// TexturedRectangle rect = new TexturedRectangle (game, "image1", new Vector3 (400, 400, -400), Vector3.Right + Vector3.Backward, 400, Vector3.Up, 50);
			// rect.IsMovable = true;
			// objects.Add (rect);

			// the floor
			floor = new TexturedRectangle (state, "floor", position + new Vector3 (size.X, 0, size.Z) / 2,
				Vector3.Left, size.X, Vector3.Forward, size.Z);
			objects.Add (floor);
		}

		public void SelectObject (GameObject obj, GameTime gameTime)
		{
			if (SelectedObject != obj) {
				if (SelectedObject != null) {
					SelectedObject.OnUnselected (gameTime);
				}
				SelectedObject = obj; 
				if (SelectedObject != null) {
					SelectedObject.OnSelected (gameTime);
				}
			}
		}

		public float SelectedObjectDistance ()
		{
			if (SelectedObject != null) {
				Vector3 toTarget = SelectedObject.Center () - camera.Position;
				return toTarget.Length ();
			} else {
				return 0;
			}
		}

		public Vector3 Clamp (Vector3 v)
		{
			return v.Clamp (position, position + size);
		}
		
		public void Add (GameObject obj)
		{
			objects.Add (obj);
		}
		
		public void Draw (GameTime gameTime)
		{
			foreach (GameObject obj in objects) {
				obj.Draw (gameTime);
			}
		}

		public void Update (GameTime gameTime)
		{
			// run the update method on all game objects
			foreach (GameObject obj in objects) {
				obj.Update (gameTime);
			}

			// mouse ray selection
			UpdateMouseRay (gameTime);

			// spawn a game object
			if (Keys.Z.IsDown ()) {
				//objects.Add (new GameModel (state, "Test3D", new Vector3 (-200, 200, 200), 0.1f));
				var obj = new TestModel (state, "Test3D", new Vector3 (200, 200, 200), 0.1f);
				obj.IsMovable = true;
				objects.Add (obj);
			}
			if (Keys.P.IsDown ()) {
				//objects.Add (new GameModel (state, "Test3D", new Vector3 (-200, 200, 200), 0.1f));
				var obj = new TestModel (state, "pipe1", new Vector3 (-200, 200, -200), 100f);
				obj.IsMovable = true;
				objects.Add (obj);
			}

			// debug mode?
			floor.IsVisible = Game.Debug;
		}

		public void UpdateMouseRay (GameTime gameTime)
		{
			double millis = gameTime.TotalGameTime.TotalMilliseconds;
			if (millis > lastRayCheck + 100 && (input.CurrentInputAction == InputAction.TargetMove
				|| input.CurrentInputAction == InputAction.FreeMouse)) {
				lastRayCheck = millis;

				Ray ray = camera.GetMouseRay (Input.MouseState.ToVector2 ());

				GameObjectDistance nearest = null;
				foreach (GameObject obj in objects) {
					if (obj.IsVisible) {
						GameObjectDistance intersection = obj.Intersects (ray);
						if (intersection != null) {
							//Console.WriteLine ("time=" + (int)gameTime.TotalGameTime.TotalMilliseconds +
							//	", obj = " + obj + ", distance = " + MathHelper.Clamp ((float)distance, 0, 100000)
							//);
							if (intersection.Distance > 0 && (nearest == null || intersection.Distance < nearest.Distance)) {
								nearest = intersection;
							}
						}
					}
				}
				if (nearest != null) {
					SelectObject (nearest.Object, gameTime);
				}
			}
		}
	}

	public sealed class GameObjectDistance
	{
		public GameObject Object;
		public float Distance;
	}
	
	public class TestModel : GameModel
	{
		public TestModel (GameState state, string modelname, Vector3 position, float scale)
			: base(state, modelname, position, scale)
		{
		}

		public override void Update (GameTime gameTime)
		{
			if (Keys.U.IsHeldDown ()) {
				Position = Position.RotateY (MathHelper.PiOver4 / 100f);
			}
			base.Update (gameTime);
		}
	}
}

