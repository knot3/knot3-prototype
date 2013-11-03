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
		// graphics-related classes
		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private BasicEffect basicEffect;
		
		// custom classes
		private Game game;
		private Camera camera;
		private Floor3D floor;
		private TexturedRectangle quad;

		// fonts
		private Texture2D dummyTexture;
		private Model model;
		private Rectangle dummyRectangle;

		// world data
		private Vector3 position;
		private Vector3 size;

		// enabled?
		public static bool Enabled { get; set; }

		/// <summary>
		/// Initializes a new Overlay-
		/// </summary>
		public World (Camera camera, GraphicsDeviceManager graphics, BasicEffect basicEffect, Game game)
		{
			this.camera = camera;
			this.graphics = graphics;
			this.basicEffect = basicEffect;
			this.game = game;

			size = new Vector3 (4000, 1000, 4000);
			position = new Vector3 (-2000, -100, -2000);

			// create a new SpriteBatch, which can be used to draw textures
			spriteBatch = new SpriteBatch (graphics.GraphicsDevice);

			// load test texture
			try {
				model = game.Content.Load<Model> ("Test3D");
			} catch (ContentLoadException ex) {
				model = null;
				Console.WriteLine (ex.ToString ());
			}

			// create floor
			floor = new Floor3D (graphics.GraphicsDevice, position, size, game);
			quad = new TexturedRectangle (graphics.GraphicsDevice, game,
				new Vector3 (200, 200, 200), Vector3.Backward, Vector3.Up, 400, 50
			);

			// ...
			dummyTexture = new Texture2D (graphics.GraphicsDevice, 1, 1);
			dummyTexture.SetData (new Color[] { Color.Red });
		}

		public Vector3 Clamp (Vector3 v)
		{
			return v.Clamp (position, position + size);
		}
		
		public void Draw (GameTime gameTime)
		{
			if (Enabled) {
				DrawBorders (gameTime);
				//DrawTest (gameTime);
			}
		}
		
		public void DrawTest (GameTime gameTime)
		{
			// test:
			foreach (ModelMesh mesh in model.Meshes) {
				foreach (BasicEffect effect in mesh.Effects) {
					if (game.Input.KeyboardState.IsKeyDown (Keys.L)) {
						effect.EnableDefaultLighting ();  // Beleuchtung aktivieren
					}
					effect.World = camera.WorldMatrix;
					effect.View = camera.ViewMatrix;
					effect.Projection = camera.ProjectionMatrix;
				}
				mesh.Draw ();
			}

		}

		private void DrawBorders (GameTime gameTime)
		{
			// var floorColors = new Color[1] {
			//     Color.CornflowerBlue
			// };

			BasicEffect eff = new BasicEffect (graphics.GraphicsDevice);
			//floor.Draw (camera, eff);
			quad.Draw (camera, eff);
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

	public class TexturedRectangle2
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
		public TexturedRectangle2 (GraphicsDevice device, Game game, Vector3[] edges)
		{
			this.device = device;
			this.edges = edges;
			this.game = game;
			BuildBuffer ();
		}

		//Build our vertex buffer
		private void BuildBuffer ()
		{
			try {
				texture = game.Content.Load<Texture2D> ("background");
			} catch (ContentLoadException ex) {
				Console.WriteLine (ex.ToString ());
				texture = null;
				return;
			}
			// Vertices erstellen und zuweisen.
			vertices = new VertexPositionNormalTexture[4];

			vertices [0].Position = edges [0];
			vertices [0].TextureCoordinate = new Vector2 (0.0f, 1.0f);
			vertices [0].Normal = Vector3.Forward;

			vertices [1].Position = edges [1];
			vertices [1].TextureCoordinate = new Vector2 (0.0f, 0.0f);
			vertices [1].Normal = Vector3.Forward;

			vertices [2].Position = edges [2];
			vertices [2].TextureCoordinate = new Vector2 (1.0f, 1.0f);
			vertices [2].Normal = Vector3.Forward;

			vertices [3].Position = edges [3];
			vertices [3].TextureCoordinate = new Vector2 (1.0f, 0.0f);
			vertices [3].Normal = Vector3.Forward;

			vertexBuffer = new VertexBuffer (
		        device,
		        typeof(VertexPositionNormalTexture),
		        vertices.Length,
		        BufferUsage.WriteOnly
			);

			vertexBuffer.SetData (vertices);

			// Indices erstellen und zuweisen.
			var indices = new int[12];

			// 1. Dreieck Vordersiete
			indices [0] = 2;
			indices [1] = 1;
			indices [2] = 0;

			// 2. Dreieck Vordersiete
			indices [3] = 2;
			indices [4] = 3;
			indices [5] = 1;

			// 1. Dreieck Rueckseite
			indices [6] = 0;
			indices [7] = 1;
			indices [8] = 2;

			// 2. Dreieck Rueckseite
			indices [9] = 1;
			indices [10] = 3;
			indices [11] = 2;

			indexBuffer = new IndexBuffer (
		        device,
		        typeof(int),
		        indices.Length,
		        BufferUsage.WriteOnly
			);
			indexBuffer.SetData (indices);
		}

		//Draw method
		public void Draw (Camera camera, BasicEffect effect)
		{
			if (texture != null) {
				effect.World = camera.WorldMatrix;
				effect.View = camera.ViewMatrix;
				effect.Projection = camera.ProjectionMatrix;

			
				if (game.Input.KeyboardState.IsKeyDown (Keys.L)) {
					effect.EnableDefaultLighting ();  // Beleuchtung aktivieren
				}
    
				//effect.TextureEnabled = true; 
				effect.Texture = texture;
				effect.TextureEnabled = true;
				effect.VertexColorEnabled = false;
				//effect.VertexColorEnabled = true;

				effect.CurrentTechnique.Passes [0].Apply ();

				foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
					pass.Apply ();

					device.SetVertexBuffer (vertexBuffer);
					device.Indices = indexBuffer;
					//effect.DirectionalLight0.Enabled = true;
					//effect.DirectionalLight0.DiffuseColor = Color.Pink.ToVector3();
					//effect.DirectionalLight0.Direction = Vector3.Backward;

					device.DrawIndexedPrimitives (PrimitiveType.TriangleList, 0, 0, vertices.Length, 0, 4);

					pass.Apply ();
				}
			}
		}
	}

	public class Floor3D
	{
		//Attributes
		private Vector3 position;
		private Vector3 size;
		private GraphicsDevice device;
		private Game game;
		//...
		private VertexPositionNormalTexture[] vertices;
		private VertexBuffer vertexBuffer;
		private IndexBuffer indexBuffer;
		private Texture2D texture;

		//Constructor
		public Floor3D (GraphicsDevice device, Vector3 position, Vector3 size, Game game)
		{
			this.device = device;
			this.position = position;
			this.size = size;
			this.game = game;
			BuildFloorBuffer ();
		}

		//Build our vertex buffer
		private void BuildFloorBuffer ()
		{
			try {
				texture = game.Content.Load<Texture2D> ("background");
			} catch (ContentLoadException ex) {
				Console.WriteLine (ex.ToString ());
				texture = null;
				return;
			}
			// Vertices erstellen und zuweisen.
			vertices = new VertexPositionNormalTexture[4];

			vertices [0].Position = new Vector3 (position.X, position.Y, position.Z);
			vertices [0].TextureCoordinate = new Vector2 (0.0f, 1.0f);
			vertices [0].Normal = Vector3.Forward;

			vertices [1].Position = new Vector3 (position.X, position.Y, position.Z + size.Z);
			vertices [1].TextureCoordinate = new Vector2 (0.0f, 0.0f);
			vertices [1].Normal = Vector3.Forward;

			vertices [2].Position = new Vector3 (position.X + size.X, position.Y, position.Z);
			vertices [2].TextureCoordinate = new Vector2 (1.0f, 1.0f);
			vertices [2].Normal = Vector3.Forward;

			vertices [3].Position = new Vector3 (position.X + size.X, position.Y, position.Z + size.Z);
			vertices [3].TextureCoordinate = new Vector2 (1.0f, 0.0f);
			vertices [3].Normal = Vector3.Forward;

			vertexBuffer = new VertexBuffer (
		        device,
		        typeof(VertexPositionNormalTexture),
		        vertices.Length,
		        BufferUsage.WriteOnly
			);

			vertexBuffer.SetData (vertices);

			// Indices erstellen und zuweisen.
			var indices = new int[12];

			// 1. Dreieck Vordersiete
			indices [0] = 2;
			indices [1] = 1;
			indices [2] = 0;

			// 2. Dreieck Vordersiete
			indices [3] = 2;
			indices [4] = 3;
			indices [5] = 1;

			// 1. Dreieck Rueckseite
			indices [6] = 0;
			indices [7] = 1;
			indices [8] = 2;

			// 2. Dreieck Rueckseite
			indices [9] = 1;
			indices [10] = 3;
			indices [11] = 2;

			indexBuffer = new IndexBuffer (
		        device,
		        typeof(int),
		        indices.Length,
		        BufferUsage.WriteOnly
			);
			indexBuffer.SetData (indices);
		}

		//Draw method
		public void Draw (Camera camera, BasicEffect effect)
		{
			if (texture != null) {
				effect.World = camera.WorldMatrix;
				effect.View = camera.ViewMatrix;
				effect.Projection = camera.ProjectionMatrix;

			
				if (game.Input.KeyboardState.IsKeyDown (Keys.L)) {
					effect.EnableDefaultLighting ();  // Beleuchtung aktivieren
				}
    
				//effect.TextureEnabled = true; 
				effect.Texture = texture;
				effect.TextureEnabled = true;
				effect.VertexColorEnabled = false;
				//effect.VertexColorEnabled = true;

				effect.CurrentTechnique.Passes [0].Apply ();

				foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
					pass.Apply ();

					device.SetVertexBuffer (vertexBuffer);
					device.Indices = indexBuffer;
					//effect.DirectionalLight0.Enabled = true;
					//effect.DirectionalLight0.DiffuseColor = Color.Pink.ToVector3();
					//effect.DirectionalLight0.Direction = Vector3.Backward;

					device.DrawIndexedPrimitives (PrimitiveType.TriangleList, 0, 0, vertices.Length, 0, 4);

					pass.Apply ();
				}
			}
		}
	}
}

