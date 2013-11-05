using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

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
		public BasicEffect basicEffect;

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
		private World world;
		private DrawLines drawLines;

		// debug
		public static bool Debug = false;

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
            // vsync
            graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;

            // anti aliasing
            graphics.GraphicsDevice.PresentationParameters.MultiSampleCount = 4;
            graphics.PreferMultiSampling = true;

			// basic effect
			basicEffect = new BasicEffect (GraphicsDevice);
			basicEffect.VertexColorEnabled = true;
			basicEffect.View = Matrix.CreateLookAt (new Vector3 (0, 0, -1000), new Vector3 (0, 0, 1), new Vector3 (0, 1, 0));
			basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView (MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1.0f, 2000.0f);

			// camera
			camera = new Camera (this);

			// input
			input = new Input (this);
			input.SaveStates ();

			// overlay
			overlay = new Overlay (this);

			// overlay
			world = new World (this);

			// line drawing
			drawLines = new DrawLines (this);

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
			// world
			world.Update (gameTime);
			// overlay
			overlay.Update (gameTime);
			// base method
			base.Update (gameTime);
		}

		private void Updateinput ()
		{
			// allows the game to exit
			if (Keys.Escape.IsDown ()) {
				this.Exit ();
				return;
			}

			// change background color
			if (Keys.Space.IsDown ()) {
				backColor = new Color (backColor.R, backColor.G, (byte)~backColor.B);
			}

			// select lines
			if (Keys.Y.IsDown () || Keys.NumPad1.IsDown ()) {
				lines.SelectedLine -= 1;
			} else if (Keys.X.IsDown () || Keys.NumPad3.IsDown ()) {
				lines.SelectedLine += 1;
			}

			// move lines
			if (Keys.NumPad8.IsDown ())
				lines.InsertAt (lines.SelectedLine, Vector3.Up);
			if (Keys.NumPad2.IsDown ())
				lines.InsertAt (lines.SelectedLine, Vector3.Down);
			if (Keys.NumPad4.IsDown ())
				lines.InsertAt (lines.SelectedLine, Vector3.Left);
			if (Keys.NumPad6.IsDown ())
				lines.InsertAt (lines.SelectedLine, Vector3.Right);
			if (Keys.NumPad7.IsDown ())
				lines.InsertAt (lines.SelectedLine, Vector3.Forward);
			if (Keys.NumPad9.IsDown ())
				lines.InsertAt (lines.SelectedLine, Vector3.Backward);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear (backColor);
			basicEffect.CurrentTechnique.Passes [0].Apply ();

			//Test.Lightning(basicEffect);
			GraphicsDevice.BlendState = BlendState.Opaque;
			GraphicsDevice.DepthStencilState = DepthStencilState.Default;

			world.Draw (gameTime);
			basicEffect.CurrentTechnique.Passes [0].Apply ();
			drawLines.Draw (lines, gameTime);
			basicEffect.CurrentTechnique.Passes [0].Apply ();
			overlay.Draw (gameTime);
			basicEffect.CurrentTechnique.Passes [0].Apply ();
			camera.Draw (gameTime);
			base.Draw (gameTime);
		}

		public Camera Camera { get { return camera; } }

		public Input Input { get { return input; } }

		public World World { get { return world; } }
		
		public GraphicsDeviceManager Graphics { get { return graphics; } }

		public static bool IsRunningOnMono ()
		{
			return Type.GetType ("Mono.Runtime") != null;
		}

		public static TimeSpan Time (Action action)
		{
			Stopwatch stopwatch = Stopwatch.StartNew ();
			action ();
			stopwatch.Stop ();
			return stopwatch.Elapsed;
		}
	}

	public abstract class GameClass
	{
		protected Game game;

		protected GraphicsDevice device {
			get { return game.GraphicsDevice; }
		}

		protected GraphicsDeviceManager graphics {
			get { return game.Graphics; }
		}

		protected Camera camera {
			get { return game.Camera; }
		}

		protected Input input {
			get { return game.Input; }
		}

		protected World world {
			get { return game.World; }
		}

		public GameClass (Game game)
		{
			this.game = game;
		}
	}
}
