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

using Knot3.Utilities;
using Knot3.Settings;
using Knot3.RenderEffects;

namespace Knot3.Core
{
	/// <summary>
	/// Die Haupt-Klasse des Spiels. Verwaltet den jeweils aktuellen GameState.
	/// </summary>
	public class Game : Microsoft.Xna.Framework.Game
	{
		// graphics-related classes
		private GraphicsDeviceManager graphics;

		// custom classes
		public GameState State { get; private set; }

		// colors, sizes, ...
		public static Size DefaultSize = new Size (1280, 720);

		// debug
		public static bool Debug { get { return Options.Default ["game", "debug", false]; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="TestGame1.Game"/> class.
		/// </summary>
		public Game ()
		{
			graphics = new GraphicsDeviceManager (this);
			
			graphics.PreferredBackBufferWidth = DefaultSize.Width;
			graphics.PreferredBackBufferHeight = DefaultSize.Height;

			graphics.IsFullScreen = false;
			isFullscreen = false;
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
			VSync = true;

			// anti aliasing
			graphics.GraphicsDevice.PresentationParameters.MultiSampleCount = 4;
			graphics.PreferMultiSampling = true;

			// base method
			base.Initialize ();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{
			GameStates.Initialize (this);
			State = GameStates.StartScreen;
			State.Activate (null);
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
			// change game state?
			if (State != State.NextState) {
				State.NextState.PostProcessing = new FadeEffect (State.NextState, State);
				State.Deactivate (gameTime);
				State = State.NextState.NextState = State.NextState;
				State.Activate (gameTime);
			}

			// global keyboard ans mouse input 
			UpdateInput (gameTime);

			// set the next game state
			State.Update (gameTime);

			// base method
			base.Update (gameTime);
		}

		private void UpdateInput (GameTime gameTime)
		{
			// allows the game to exit
			if (Keys.F8.IsDown ()) {
				this.Exit ();
				return;
			}
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			// current game state
			State.Draw (gameTime);

			// base class
			base.Draw (gameTime);
		}

		public bool VSync {
			get {
				return graphics.SynchronizeWithVerticalRetrace;
			}
			set {
				graphics.SynchronizeWithVerticalRetrace = value;
				this.IsFixedTimeStep = value;
				graphics.ApplyChanges ();
			}
		}

		private bool isFullscreen;

		public bool IsFullscreen {
			get {
				return isFullscreen;
			}
			set {
				if (value != isFullscreen) {
					Console.WriteLine ("Fullscreen Toggle");
					if (value) {
						graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.DisplayMode.Width;
						graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.DisplayMode.Height;
					} else {
						graphics.PreferredBackBufferWidth = Game.DefaultSize.Width;
						graphics.PreferredBackBufferHeight = Game.DefaultSize.Height;
					}
					graphics.ApplyChanges ();
					graphics.ToggleFullScreen ();
					graphics.ApplyChanges ();
					isFullscreen = value;
				}
			}
		}
		
		public GraphicsDeviceManager Graphics { get { return graphics; } }

		public static TimeSpan Time (Action action)
		{
			Stopwatch stopwatch = Stopwatch.StartNew ();
			action ();
			stopwatch.Stop ();
			return stopwatch.Elapsed;
		}
	}
}