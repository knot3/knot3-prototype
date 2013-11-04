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
	public class World
	{
		// custom classes
		private Game game;
		private Camera camera;

		// game objects
		private List<GameObject> objects;

		// world data
		private Vector3 position;
		private Vector3 size;

		// enabled?
		public static bool Enabled { get; set; }

		/// <summary>
		/// Initializes a new Overlay-
		/// </summary>
		public World (Game game)
		{
			this.game = game;
			this.camera = game.Camera;

			size = new Vector3 (4000, 1000, 4000);
			position = new Vector3 (-2000, -100, -2000);

			objects = new List<GameObject> ();

			// some game objects
			objects.Add(new TexturedRectangle (game, new Vector3 (200, 200, 200), Vector3.Left, 400, Vector3.Up, 50));
			objects.Add(new TestModel (game));
			// the floor
			objects.Add(new TexturedRectangle (game, new Vector3 (0, -200, 0), Vector3.Left, 1000, Vector3.Forward, 1000));

			// create floor
			//floor = new Floor3D (graphics.GraphicsDevice, position, size, game);
		}

		public Vector3 Clamp (Vector3 v)
		{
			return v.Clamp (position, position + size);
		}
		
		public void Draw (GameTime gameTime)
		{
			if (Enabled) {
				foreach (GameObject obj in objects) {
					obj.Draw (gameTime);
				}
			}
		}
	}

	public class TestModel : GameObject
	{
		private Model model;

		public TestModel (Game game)
			: base(game)
		{
			// load test model
			model = LoadModel ("Test3D");
		}

		public override void DrawObject ()
		{
			// test:
			foreach (ModelMesh mesh in model.Meshes) {
				foreach (BasicEffect effect in mesh.Effects) {
					if (game.Input.KeyboardState.IsKeyDown (Keys.L)) {
						effect.EnableDefaultLighting ();  // Beleuchtung aktivieren
					} else {
						effect.LightingEnabled = false;
					}
					effect.World = Matrix.CreateScale (0.01f) * Matrix.CreateTranslation (camera.Target);  //camera.WorldMatrix*0.001f;
					effect.View = camera.ViewMatrix;
					effect.Projection = camera.ProjectionMatrix;
				}
				mesh.Draw ();
			}
		}
	}

	public class TexturedQuad
	{
		//Attributes
		private Vector3[] edges;
		private GraphicsDevice device;
		private Game game;
		//...
		private VertexPositionNormalTexture[] vertices;
		private VertexBuffer vertexBuffer;
		private IndexBuffer indexBuffer;
		private Texture2D texture;

		//Constructor
		public TexturedQuad (GraphicsDevice device, Vector3[] edges, Game game)
		{
			this.device = device;
			this.edges = edges;
			this.game = game;
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

		public GameObject (Game game)
		{
			this.game = game;
		}

		public void Draw (GameTime gameTime)
		{
			basicEffect = new BasicEffect (device);
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
	}
}

