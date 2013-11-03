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

			model = game.Content.Load<Model> ("Test3D");
			dummyTexture = new Texture2D (graphics.GraphicsDevice, 1, 1);
			dummyTexture.SetData (new Color[] { Color.Red });
		}

		public Vector3 Clamp (Vector3 v)
		{
			return v.Clamp(position, position+size);
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
			var floorColors = new Color[1] {
			// Color.White, Color.LightGray
				Color.CornflowerBlue
			};

			BasicEffect eff = new BasicEffect (graphics.GraphicsDevice);
			Floor3D floor = new Floor3D (graphics.GraphicsDevice, position, size, floorColors, game);
			floor.Draw (camera, eff);
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
		private VertexPositionColorTexture[] vertices;
		private VertexBuffer vertexBuffer;
		private IndexBuffer indexBuffer;
		private Texture2D texture;

		//Constructor
		public Floor3D (GraphicsDevice device, Vector3 position, Vector3 size, Color[] floorColors, Game game)
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
			texture = game.Content.Load<Texture2D> ("background");

			// Vertices erstellen und zuweisen.
			vertices = new VertexPositionColorTexture[4];

			vertices [0].Position = new Vector3 (position.X, position.Y, position.Z);
			vertices [0].Color = Color.Yellow;
			vertices [0].TextureCoordinate = new Vector2 (0.0f, 1.0f);

			vertices [1].Position = new Vector3 (position.X, position.Y, position.Z + size.Z);
			vertices [1].Color = Color.Red;
			vertices [1].TextureCoordinate = new Vector2 (0.0f, 0.0f);

			vertices [2].Position = new Vector3 (position.X + size.X, position.Y, position.Z);
			vertices [2].Color = Color.Blue;
			vertices [2].TextureCoordinate = new Vector2 (1.0f, 1.0f);

			vertices [3].Position = new Vector3 (position.X + size.X, position.Y, position.Z + size.Z);
			vertices [3].Color = Color.Green;
			vertices [3].TextureCoordinate = new Vector2 (1.0f, 0.0f);

			vertexBuffer = new VertexBuffer (
		        device,
		        typeof(VertexPositionColorTexture),
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

	public class Floor
	{
		//Attributes
		private Vector3 position;
		private Vector3 size;
		private VertexBuffer floorBuffer;
		private GraphicsDevice device;
		private int tilesize = 100;
		private Color[] floorColors;

		//Constructor
		public Floor (GraphicsDevice device, Vector3 position, Vector3 size, Color[] floorColors)
		{
			this.device = device;
			this.position = position;
			this.size = size;
			this.floorColors = floorColors;
			BuildFloorBuffer ();
		}

		//Build our vertex buffer
		private void BuildFloorBuffer ()
		{
			List<VertexPositionColor> vertexList = new List<VertexPositionColor> ();
			int counter = 0;

			Vector2 tiles = new Vector2 (size.X, size.Z) / tilesize;

			//Loop through to create floor
			for (int x = 0; x < tiles.X; x++) {
				counter++;
				for (int z = 0; z < tiles.Y; z++) {
					counter++;

					//loop through and add vertices
					foreach (VertexPositionColor vertex in 
					         FloorTile(x, z, floorColors[counter % floorColors.Count()])) {
						vertexList.Add (vertex);
					}

				}
			}

			//Create our buffer
			floorBuffer = new VertexBuffer (device, VertexPositionColor.VertexDeclaration, vertexList.Count, BufferUsage.None);
			floorBuffer.SetData<VertexPositionColor> (vertexList.ToArray ());

		}

		//Defines a single tile in our floor
		private List<VertexPositionColor> FloorTile (float x, float z, Color tileColor)
		{
			List<VertexPositionColor> vList = new List<VertexPositionColor> ();
			vList.Add (new VertexPositionColor (position + tilesize * new Vector3 (0 + x, 0, 0 + z), tileColor));
			vList.Add (new VertexPositionColor (position + tilesize * new Vector3 (1 + x, 0, 0 + z), tileColor));
			vList.Add (new VertexPositionColor (position + tilesize * new Vector3 (0 + x, 0, 1 + z), tileColor));
			vList.Add (new VertexPositionColor (position + tilesize * new Vector3 (1 + x, 0, 0 + z), tileColor));
			vList.Add (new VertexPositionColor (position + tilesize * new Vector3 (1 + x, 0, 1 + z), tileColor));
			vList.Add (new VertexPositionColor (position + tilesize * new Vector3 (0 + x, 0, 1 + z), tileColor));
			return vList;
		}

		//Draw method
		public void Draw (Camera camera, BasicEffect effect)
		{
			//Test.Lightning(effect);
			effect.EnableDefaultLighting ();
			effect.TextureEnabled = true; 
			effect.VertexColorEnabled = true;
			effect.View = camera.ViewMatrix;
			effect.Projection = camera.ProjectionMatrix;
			effect.World = camera.WorldMatrix;

			//Loop through and draw each vertex
			foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
				pass.Apply ();
				device.SetVertexBuffer (floorBuffer);
				device.DrawPrimitives (PrimitiveType.TriangleList, 0, floorBuffer.VertexCount / 3);
			}
		}
	}
}

