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
		private BasicEffect basicEffect;

		// nodes
		private NodeList nodes;
		private LineList lines;

		// colors, ...
		private Color backColor = Color.CornflowerBlue;
		public static Size defaultSize = new Size (1280, 720);

		// custom classes
		private Input input;
		private Camera camera;
		private Overlay overlay;
		private DrawLines drawLines;

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
			input = new Input (camera, graphics, this);
			input.SaveStates ();

			// overlay
			overlay = new Overlay (camera, graphics, this);

			// line drawing
			drawLines = new DrawLines (camera, graphics, this);

			// base method
			base.Initialize ();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{
			// load nodes
			Node.Scale = 100;
			nodes = new NodeList ();
			lines = new LineList (nodes);

			// load camera
			camera.LoadContent ();

			// load overlay
			overlay.LoadContent ();

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
			Updateinput ();
			// camera
			camera.Update (gameTime);
			// input
			input.Update (gameTime);
			input.SaveStates ();
			// base method
			base.Update (gameTime);
		}

		private void Updateinput ()
		{
			// change background color
			if (Keys.Space.IsDown ()) {
				backColor = new Color (backColor.R, backColor.G, (byte)~backColor.B);
			}

			// select lines
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

			drawLines.Draw (lines, gameTime);
			camera.Draw (gameTime);
			overlay.Draw (gameTime);
			base.Draw (gameTime);
		}

		public Camera Camera { get { return camera; } }

		public Input Input { get { return input; } }
	}
}
