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
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		BasicEffect basicEffect;
		Color backColor = Color.CornflowerBlue;
		NodeList nodes;
		LineList lines;
		Camera camera;
		KeyboardState previousKeyboardState;
		int previousScrollValue;
        SpriteFont font;

		public Game1 ()
		{
			graphics = new GraphicsDeviceManager (this);
			graphics.PreferredBackBufferWidth = 1280;
			graphics.PreferredBackBufferHeight = 720;

			graphics.IsFullScreen = false;
			graphics.ApplyChanges ();

			Content.RootDirectory = "Content";
			Window.Title = "Test Game 1";

			nodes = new NodeList ();
			lines = new LineList (nodes);
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{
			// TODO: Add your initialization logic here
			basicEffect = new BasicEffect (GraphicsDevice);
			basicEffect.VertexColorEnabled = true;
			basicEffect.View = Matrix.CreateLookAt (new Vector3 (0, 0, -1000), new Vector3 (0, 0, 1), new Vector3 (0, 1, 0));
			basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView (MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1.0f, 2000.0f);

			camera = new Camera (graphics, basicEffect);

			if (0 == 1) {
				basicEffect.Projection = Matrix.CreateOrthographicOffCenter (
				0, GraphicsDevice.Viewport.Width,     // left, right
				GraphicsDevice.Viewport.Height, 0,    // bottom, top
				0, 1
				);
			}

			base.Initialize ();
            previousKeyboardState = Keyboard.GetState();

            try
            {
                font = Content.Load<SpriteFont>("Font");
            }
            catch (ContentLoadException ex)
            {
                font = null;
                Console.WriteLine(ex.Message);
            }
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{
			camera.LoadContent ();

			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch (GraphicsDevice);

			// TODO: use this.Content to load your game content here
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
			camera.Update (gameTime);
			base.Update (gameTime);
		}

		private bool IsKeyDown (Keys key)
		{
			KeyboardState keyboardState = Keyboard.GetState ();
			// Is the key down?
			if (keyboardState.IsKeyDown (key)) {
				// If not down last update, key has just been pressed.
				if (!previousKeyboardState.IsKeyDown (key)) {
					return true;
				}
			}
			return false;
		}

		private void UpdateInput ()
		{
			KeyboardState keyboardState = Keyboard.GetState ();
			MouseState mouseState = Mouse.GetState (); 

			// change background color
			if (IsKeyDown (Keys.Space)) {
				backColor = new Color (backColor.R, backColor.G, (byte)~backColor.B);
			}

			// allows the game to exit
			if (IsKeyDown (Keys.Escape)) {
				this.Exit ();
			}

			// fullscreen
			if (IsKeyDown (Keys.F) || IsKeyDown (Keys.F11)) {
				graphics.ToggleFullScreen ();
                graphics.PreferredBackBufferWidth = 1440;
                graphics.PreferredBackBufferHeight = 900;
                graphics.ApplyChanges();
			}

			// scroll wheel zoom
			if (mouseState.ScrollWheelValue < previousScrollValue || keyboardState.IsKeyDown (Keys.OemPlus)) {
				camera.zoom (-10);
			} else if (mouseState.ScrollWheelValue > previousScrollValue || keyboardState.IsKeyDown (Keys.OemMinus)) {
				camera.zoom (+10);
			}

			if (IsKeyDown (Keys.Y)) {
				lines.SelectedLine -= 1;
			} else if (IsKeyDown (Keys.X)) {
				lines.SelectedLine += 1;
			}

			// test
			if (mouseState.LeftButton == ButtonState.Pressed) {
				int x = Convert.ToInt32 (mouseState.X) / 16;
				int y = Convert.ToInt32 (mouseState.Y) / 16 + 1;

				//tiles [x, y] = null;//Left button Clicked, Change Texture here!

			}

			// Update saved state.
			previousScrollValue = mouseState.ScrollWheelValue;
			previousKeyboardState = keyboardState;
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

			camera.Draw (gameTime);
			DrawCoordinates ();

			base.Draw (gameTime);
		}

		private void DrawLines ()
		{
			Node.Scale = 100;
			Vector3 offset = new Vector3 (10, 10, 10);

			var vertices = new VertexPositionColor[lines.Count * 4];

			Vector3 last = new Vector3(0,0,0);
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

				Console.WriteLine (vertices [4 * n + 2]);
				last = p2;
			}
			Console.WriteLine (lines.SelectedLine);
			for (int n = 0; n < lines.Count*4; n++) {
				if (n % 4 >= 2) {
					vertices [n].Color = Color.White;
				} else {
					vertices [n].Color = Color.Gray;
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
            if (font != null)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(font, "X: " + (int)MathHelper.ToDegrees(camera.AngleX), new Vector2(20, 20), Color.Green);
                spriteBatch.DrawString(font, "Y: " + (int)MathHelper.ToDegrees(camera.AngleY), new Vector2(20, 50), Color.Red);
                spriteBatch.DrawString(font, "Z: " + (int)MathHelper.ToDegrees(camera.AngleZ), new Vector2(20, 80), Color.Yellow);
                spriteBatch.End();
            }
		}

		private void DrawCircle ()
		{
			var vertices = new VertexPositionColor[100];
			for (int i = 0; i < 99; i++) {
				float angle = (float)(i / 100.0 * Math.PI * 2);
				vertices [i].Position = new Vector3 (200 + (float)Math.Cos (angle) * 100, 200 + (float)Math.Sin (angle) * 100, 0);
				vertices [i].Color = Color.Black;
			}
			vertices [99] = vertices [0];
			graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor> (PrimitiveType.LineStrip, vertices, 0, 99);
		}
	}
}
