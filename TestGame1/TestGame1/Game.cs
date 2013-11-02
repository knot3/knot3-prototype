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

namespace TestGame1
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game : Microsoft.Xna.Framework.Game
	{
		// graphics-related classes
		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private BasicEffect basicEffect;

		// nodes
		private NodeList nodes;
		private LineList lines;

		// fonts and colors
		private SpriteFont font;
		private Color backColor = Color.CornflowerBlue;
		public static Size defaultSize = new Size (1280, 720);

		// custom classes
		private Input input;
		private Camera camera;

		// debug
		public static bool Debug = true;

		/// <summary>
		/// Initializes a new instance of the <see cref="TestGame1.Game"/> class.
		/// </summary>
		public Game ()
		{
			graphics = new GraphicsDeviceManager (this);
			
			graphics.PreferredBackBufferWidth = defaultSize.Width;
			graphics.PreferredBackBufferHeight = defaultSize.Height;

			graphics.IsFullScreen = false;
			graphics.ApplyChanges ();

			Content.RootDirectory = "Content";
			Window.Title = "Test Game 1";
		}

		/// <summary>
		/// Initialize the Game.
		/// </summary>
		protected override void Initialize ()
		{
			// basic effect
			basicEffect = new BasicEffect (GraphicsDevice);
			basicEffect.VertexColorEnabled = true;
			basicEffect.View = Matrix.CreateLookAt (new Vector3 (0, 0, -1000), new Vector3 (0, 0, 1), new Vector3 (0, 1, 0));
			basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView (MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1.0f, 2000.0f);

			// camera
			camera = new Camera (graphics, basicEffect, this);

			// input
			input = new Input (Camera, graphics, this);
			input.SaveStates ();

			// base method
			base.Initialize ();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{
			// load fonts
			try {
				font = Content.Load<SpriteFont> ("Font");
			} catch (ContentLoadException ex) {
				font = null;
				Console.WriteLine (ex.Message);
			}

			// load nodes
			Node.Scale = 100;
			nodes = new NodeList ();
			lines = new LineList (nodes);

			// load camera
			Camera.LoadContent ();

			// create a new SpriteBatch, which can be used to draw textures
			spriteBatch = new SpriteBatch (GraphicsDevice);

			// add some default nodes
			nodes.Add (new Node (0, 0, 0));
			nodes.Add (new Node (0, 1, 0));
			nodes.Add (new Node (1, 1, 0));
			nodes.Add (new Node (1, 0, 0));

			nodes.Add (new Node (1, 0, 1));
			nodes.Add (new Node (1, 1, 1));
			nodes.Add (new Node (0, 1, 1));
			nodes.Add (new Node (0, 0, 1));
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent ()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{
			UpdateInput ();
			// camera
			Camera.Update (gameTime);
			// input
			Input.Update (gameTime);
			Input.SaveStates ();
			// base method
			base.Update (gameTime);
		}

		private void UpdateInput ()
		{
			// change background color
			if (Keys.Space.IsDown ()) {
				backColor = new Color (backColor.R, backColor.G, (byte)~backColor.B);
			}

			if (Keys.Y.IsDown ()) {
				lines.SelectedLine -= 1;
			} else if (Keys.X.IsDown ()) {
				lines.SelectedLine += 1;
			}
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear (backColor);
			basicEffect.CurrentTechnique.Passes [0].Apply ();

			if (nodes.Count > 0) {
				DrawLines ();
			}

			Camera.Draw (gameTime);
			DrawCoordinates ();
			base.Draw (gameTime);
		}

		private void DrawLines ()
		{
			Vector3 offset = new Vector3 (10, 10, 10);

			var vertices = new VertexPositionColor[lines.Count * 4];

			Vector3 last = new Vector3 (0, 0, 0);
			for (int n = 0; n < lines.Count; n++) {
				Vector3 p1 = lines [n].From.Vector () + offset;
				Vector3 p2 = lines [n].To.Vector () + offset;

				var diff = p1 - p2;
				diff.Normalize ();
				p1 = p1 - 10 * diff;
				p2 = p2 + 10 * diff;

				vertices [4 * n + 0].Position = n == 0 ? p1 : last;
				vertices [4 * n + 1].Position = p1;
				vertices [4 * n + 2].Position = p1;
				vertices [4 * n + 3].Position = p2;

				//Console.WriteLine (vertices [4 * n + 2]);
				last = p2;
			}
			for (int n = 0; n < lines.Count*4; n++) {
				if (n % 4 >= 2) {
					vertices [n].Color = Color.White;
				} else {
					vertices [n].Color = Color.Wheat;
				}
			}
			for (int n = 0; n < lines.Count; n++) {
				vertices [4 * n + 2].Color = lines.Color (n);
				vertices [4 * n + 3].Color = lines.Color (n);
			}
			graphics.GraphicsDevice.DrawUserPrimitives (PrimitiveType.LineList, vertices, 0, lines.Count * 2); 
		}

		private void DrawCoordinates ()
		{
			int length = 1000;
			var vertices = new VertexPositionColor[6];
			vertices [0].Position = new Vector3 (-length, 0, 0);
			vertices [0].Color = Color.Green;
			vertices [1].Position = new Vector3 (+length, 0, 0);
			vertices [1].Color = Color.Green;
			vertices [2].Position = new Vector3 (0, -length, 0);
			vertices [2].Color = Color.Red;
			vertices [3].Position = new Vector3 (0, +length, 0);
			vertices [3].Color = Color.Red;
			vertices [4].Position = new Vector3 (0, 0, -length);
			vertices [4].Color = Color.Yellow;
			vertices [5].Position = new Vector3 (0, 0, +length);
			vertices [5].Color = Color.Yellow;
			graphics.GraphicsDevice.DrawUserPrimitives (PrimitiveType.LineList, vertices, 0, 3);
			if (font != null) {
				spriteBatch.Begin ();
				try {
					int height = 20;
					int width1 = 20, width2 = 140, width3 = 200, width4 = 260;
					spriteBatch.DrawString (font, "Rotation: ", new Vector2 (width1, height), Color.White);
					spriteBatch.DrawString (font, "" + Camera.RotationAngle.Degrees.X, new Vector2 (width2, height), Color.Green);
					spriteBatch.DrawString (font, "" + Camera.RotationAngle.Degrees.Y, new Vector2 (width3, height), Color.Red);
					spriteBatch.DrawString (font, "" + Camera.RotationAngle.Degrees.Z, new Vector2 (width4, height), Color.Yellow);
					height += 20;
					spriteBatch.DrawString (font, "Cam Pos: ", new Vector2 (width1, height), Color.White);
					spriteBatch.DrawString (font, "" + Camera.Position.X, new Vector2 (width2, height), Color.Green);
					spriteBatch.DrawString (font, "" + Camera.Position.Y, new Vector2 (width3, height), Color.Red);
					spriteBatch.DrawString (font, "" + Camera.Position.Z, new Vector2 (width4, height), Color.Yellow);
					height += 20;
					spriteBatch.DrawString (font, "Cam Target: ", new Vector2 (width1, height), Color.White);
					spriteBatch.DrawString (font, "" + Camera.Target.X, new Vector2 (width2, height), Color.Green);
					spriteBatch.DrawString (font, "" + Camera.Target.Y, new Vector2 (width3, height), Color.Red);
					spriteBatch.DrawString (font, "" + Camera.Target.Z, new Vector2 (width4, height), Color.Yellow);
					height += 20;
					spriteBatch.DrawString (font, "FoV: ", new Vector2 (width1, height), Color.White);
					spriteBatch.DrawString (font, "" + Camera.FoV, new Vector2 (width2, height), Color.White);
					height += 20;
					spriteBatch.DrawString (font, "Distance: ", new Vector2 (width1, height), Color.White);
					spriteBatch.DrawString (font, "" + Camera.TargetDistance, new Vector2 (width2, height), Color.White);

				} catch (ArgumentException exp) {
					Console.WriteLine (exp.ToString ());
				} catch (InvalidOperationException exp) {
					Console.WriteLine (exp.ToString ());
				}
				spriteBatch.End ();
			}
		}

		public Camera Camera { get { return camera; } }

		public Input Input { get { return input; } }
	}
}
