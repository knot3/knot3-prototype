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
		private GameObject _selectedObject;

		public GameObject SelectedObject {
			get { return _selectedObject; }
			set {
				if (_selectedObject != value) {
					if (_selectedObject != null)
						_selectedObject.OnUnselected ();
					_selectedObject = value; 
					if (_selectedObject != null)
						_selectedObject.OnSelected ();
				}
			}
		}

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
			objects.Add (new TexturedRectangle (state, "floor", position + new Vector3 (size.X, 0, size.Z) / 2,
				Vector3.Left, size.X, Vector3.Forward, size.Z)
			);
		}

		public float SelectedObjectDistance {
			get {
				if (SelectedObject != null) {
					Vector3 toTarget = SelectedObject.Center () - camera.Position;
					return toTarget.Length ();
				} else {
					return 0;
				}
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
		}

		public void UpdateMouseRay (GameTime gameTime)
		{
			double millis = gameTime.TotalGameTime.TotalMilliseconds;
			if (millis > lastRayCheck + 100 && (input.CurrentInputAction == InputAction.TargetMove
				|| input.CurrentInputAction == InputAction.FreeMouse)) {
				lastRayCheck = millis;

				Ray ray = camera.GetMouseRay (input.MouseState.ToVector2 ());

				GameObjectDistance nearest = null;
				foreach (GameObject obj in objects) {
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
	
	public class GameModel : GameObject
	{
		protected Model Model { get; set; }

		protected float Scale { get; set; }

		protected Angles3 Rotation { get; set; }

		protected override Vector3 Position { get; set; }

		protected ModelMesh[] ModelMeshes;

		public GameModel (GameState state, string modelname, Vector3 position, float scale)
			: base(state)
		{
			// load test model
			Model = LoadModel (modelname);
			Scale = scale;
			Rotation = Angles3.Zero;
			Position = position;
			ModelMeshes = Model.Meshes.ToArray ();
		}

		public GameModel (GameState state, Model model, Vector3 position, float scale)
			: base(state)
		{
			// load test model
			Model = model;
			Scale = scale;
			Rotation = Angles3.Zero;
			Position = position;
			ModelMeshes = Model.Meshes.ToArray ();
		}

		public virtual void UpdateEffect (BasicEffect effect)
		{
		}

		public override void DrawObject (GameTime gameTime)
		{
			// test:
			foreach (ModelMesh mesh in ModelMeshes) {
				foreach (BasicEffect effect in mesh.Effects) {
					if (Keys.L.IsHeldDown ()) {
						effect.LightingEnabled = false;
					} else {
						effect.EnableDefaultLighting ();  // Beleuchtung aktivieren
					}
					UpdateEffect (effect);
					effect.World = Matrix.CreateScale (Scale)
						* Matrix.CreateFromYawPitchRoll (Rotation.Y, Rotation.X, Rotation.Z)
						* Matrix.CreateTranslation (Position);
					effect.View = camera.ViewMatrix;
					effect.Projection = camera.ProjectionMatrix;
				}
				mesh.Draw ();
			}
		}

		public override GameObjectDistance Intersects (Ray ray)
		{
			foreach (BoundingSphere sphere in Model.Bounds()) {
				Nullable<float> distance = ray.Intersects (sphere.Scale (Scale).Translate (Position));
				if (distance != null) {
					GameObjectDistance intersection = new GameObjectDistance () {
						Object=this, Distance=distance.Value
					};
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

	public abstract class GameObject : GameClass
	{
		protected BasicEffect basicEffect;

		protected abstract Vector3 Position { get; set; }

		public bool IsMovable { get; set; }

		public GameObject (GameState state)
			: base(state)
		{
			basicEffect = new BasicEffect (device);
		}

		protected Plane CurrentGroundPlane ()
		{
			Plane groundPlane = new Plane (Position, Position + Vector3.Up,
							Position + Vector3.Normalize (Vector3.Cross (Vector3.Up, Position - camera.Position)));
			Console.WriteLine ("groundPlane=" + groundPlane);
			return groundPlane;
		}

		protected Ray CurrentMouseRay ()
		{
			Ray ray = camera.GetMouseRay (input.MouseState.ToVector2 ());
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
			if (IsMovable && world.SelectedObject == this) {
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

		public void Draw (GameTime gameTime)
		{
			basicEffect.World = camera.WorldMatrix;
			basicEffect.View = camera.ViewMatrix;
			basicEffect.Projection = camera.ProjectionMatrix;
			DrawObject (gameTime);
		}

		public abstract void DrawObject (GameTime gameTime);

		protected Texture2D LoadTexture (string name)
		{
			try {
				return content.Load<Texture2D> (name);
			} catch (ContentLoadException ex) {
				Console.WriteLine (ex.ToString ());
				return null;
			}
		}

		private static Dictionary<string, Model> modelCache = new Dictionary<string, Model> ();

		protected Model LoadModel (string name)
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

		protected Texture2D DummyTexture ()
		{
			Texture2D dummyTexture = new Texture2D (device, 1, 1);
			dummyTexture.SetData (new Color[] { Color.Red });
			return dummyTexture;
		}

		public abstract GameObjectDistance Intersects (Ray ray);

		public abstract Vector3 Center ();

		public bool IsSelected ()
		{
			return world.SelectedObject == this;
		}

		public virtual void OnSelected ()
		{
		}

		public virtual void OnUnselected ()
		{
		}
	}
}

