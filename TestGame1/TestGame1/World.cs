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

		public GameObject SelectedObject { get; set; }

		// world data
		private Vector3 position;
		private Vector3 size;
		private double lastRayCheck = 0;

		/// <summary>
		/// Initializes a new Overlay
		/// </summary>
		public World (Game game)
			: base(game)
		{
			size = new Vector3 (2000, 1000, 2000);
			position = new Vector3 (-1000, -100, -1000);

			objects = new List<GameObject> ();

			// some game objects
			TexturedRectangle rect = new TexturedRectangle (game, "image1", new Vector3 (400, 400, -400), Vector3.Right+Vector3.Backward, 400, Vector3.Up, 50);
			rect.IsMovable = true;
			objects.Add (rect);
			// the floor
			objects.Add (new TexturedRectangle (game, "floor", position + new Vector3 (size.X, 0, size.Z) / 2,
				Vector3.Left, size.X, Vector3.Forward, size.Z)
			);
		}

		public Vector3 Clamp (Vector3 v)
		{
			return v.Clamp (position, position + size);
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
			if (Keys.Z.IsDown()) {
				//objects.Add (new GameModel (game, "Test3D", new Vector3 (-200, 200, 200), 0.1f));
				var obj = new TestModel (game, "Test3D", new Vector3 (200, 200, 200), 0.1f);
				obj.IsMovable = true;
				objects.Add (obj);
			}
		}

		public void UpdateMouseRay (GameTime gameTime)
		{
			double millis = gameTime.TotalGameTime.TotalMilliseconds;
            if (millis > lastRayCheck + 250 && (input.CurrentInputAction == InputAction.TargetMove
                 || input.CurrentInputAction == InputAction.FreeMouse))
            {
				lastRayCheck = millis;

				Ray ray = camera.GetMouseRay (game.Input.MouseState.ToVector2 ());

				GameObjectDistance nearest = null;
				foreach (GameObject obj in objects) {
					Nullable<float> distance = obj.Intersects (ray);
					if (distance != null) {
						//Console.WriteLine ("time=" + (int)gameTime.TotalGameTime.TotalMilliseconds +
						//	", obj = " + obj + ", distance = " + MathHelper.Clamp ((float)distance, 0, 100000)
						//);
						if (distance > 0 && (nearest == null || distance < nearest.Distance)) {
							nearest = new GameObjectDistance () { Object=obj, Distance=distance.Value };
						}
					}
				}
				if (nearest != null) {
					SelectedObject = nearest.Object;
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
		public TestModel (Game game, string modelname, Vector3 position, float scale)
			: base(game, modelname, position, scale)
		{
		}

		public override void Update (GameTime gameTime)
		{
			if (game.Input.KeyboardState.IsKeyDown (Keys.U)) {
				Position = Position.RotateY (MathHelper.PiOver4 / 100f);
			}
			base.Update (gameTime);
		}
	}
	
	public class GameModel : GameObject
	{
		protected Model Model { get; set; }

		protected override Vector3 Position { get; set; }

		protected float Scale { get; set; }

		protected ModelMesh[] ModelMeshes;

		public GameModel (Game game, string modelname, Vector3 position, float scale)
			: base(game)
		{
			// load test model
			Model = LoadModel (modelname);
			Position = position;
			Scale = scale;
			ModelMeshes = Model.Meshes.ToArray ();
		}

		public override void DrawObject ()
		{
			// test:
			foreach (ModelMesh mesh in ModelMeshes) {
				foreach (BasicEffect effect in mesh.Effects) {
					if (game.Input.KeyboardState.IsKeyDown (Keys.L)) {
						effect.EnableDefaultLighting ();  // Beleuchtung aktivieren
					} else {
						effect.LightingEnabled = false;
					}
					effect.World = Matrix.CreateScale (Scale) * Matrix.CreateTranslation (Position);
					effect.View = camera.ViewMatrix;
					effect.Projection = camera.ProjectionMatrix;
				}
				mesh.Draw ();
			}
		}

		public override Nullable<float> Intersects (Ray ray)
		{
			foreach (BoundingSphere sphere in Model.Bounds()) {
				Nullable<float> intersection = ray.Intersects (sphere.Scale (Scale).Translate (Position));
				if (intersection != null) {
					return intersection;
				}
			}
			return null;
		}

		public override Vector3 Center ()
		{
			Vector3 center = Vector3.Zero;
			int count = Model.Meshes.Count;
			foreach (ModelMesh mesh in Model.Meshes) {
				center += mesh.BoundingSphere.Center / count;
			}
			return center / Scale + Position;
		}
	}

	public abstract class GameObject
	{
		protected Game game;
		protected BasicEffect basicEffect;

		protected GraphicsDevice device {
			get { return game.GraphicsDevice; }
		}

		protected Camera camera {
			get { return game.Camera; }
		}

		protected abstract Vector3 Position { get; set; }

		public bool IsMovable { get; set; }

		public GameObject (Game game)
		{
			this.game = game;
			basicEffect = new BasicEffect (device);
		}

		public virtual void Update (GameTime gameTime)
		{
			// check whether is object is movable and whether it is selected
			if (IsMovable && game.World.SelectedObject == this) {
				// is SelectedObjectMove the current input action?
				if (game.Input.CurrentInputAction == InputAction.SelectedObjectMove) {
					Plane GroundPlane = new Plane (camera.Target, Vector3.Up,
							camera.Target + Vector3.Cross (Vector3.Up, camera.TargetVector));
					Ray ray = camera.GetMouseRay (game.Input.MouseState.ToVector2 ());
					float? position = ray.Intersects (GroundPlane);
					float previousLength = (Position - camera.Position).Length ();
					if (position.HasValue)
						Position = ray.Position + ray.Direction * position.Value;
					float currentLength = (Position - camera.Position).Length ();
					Position = camera.Position + (Position - camera.Position) * previousLength / currentLength;
				}
			}
		}

		public void Draw (GameTime gameTime)
		{
			basicEffect.World = camera.WorldMatrix;
			basicEffect.View = camera.ViewMatrix;
			basicEffect.Projection = camera.ProjectionMatrix;
			DrawObject ();
		}

		public abstract void DrawObject ();

		protected Texture2D LoadTexture (string name)
		{
			try {
				return game.Content.Load<Texture2D> (name);
			} catch (ContentLoadException ex) {
				Console.WriteLine (ex.ToString ());
				return null;
			}
		}

		protected Model LoadModel (string name)
		{
			try {
				return game.Content.Load<Model> (name);
			} catch (ContentLoadException ex) {
				Console.WriteLine (ex.ToString ());
				return null;
			}
		}

		protected Texture2D DummyTexture ()
		{
			Texture2D dummyTexture = new Texture2D (device, 1, 1);
			dummyTexture.SetData (new Color[] { Color.Red });
			return dummyTexture;
		}

		public abstract Nullable<float> Intersects (Ray ray);

		public abstract Vector3 Center ();
	}
}

