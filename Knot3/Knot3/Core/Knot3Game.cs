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
	/// Die Haupt-Klasse des Spiels. Verwaltet den jeweils aktuellen GameScreen.
	/// </summary>
	public class Knot3Game : Microsoft.Xna.Framework.Game
	{
		// graphics-related classes
		public GraphicsDeviceManager Graphics { get; private set; }

		// custom classes
		public GameScreen State { get; private set; }

		// colors, sizes, ...
		public static Vector2 DefaultSize = new Vector2 (1280, 720);

		// debug
		public static bool Debug { get { return Options.Default ["game", "debug", false]; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="TestGame1.Game"/> class.
		/// </summary>
		public Knot3Game ()
		{
			Graphics = new GraphicsDeviceManager (this);

			Graphics.PreferredBackBufferWidth = (int)DefaultSize.X;
			Graphics.PreferredBackBufferHeight = (int)DefaultSize.Y;

			Graphics.IsFullScreen = false;
			isFullscreen = false;
			Graphics.ApplyChanges ();

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
			Graphics.GraphicsDevice.PresentationParameters.MultiSampleCount = 4;
			Graphics.PreferMultiSampling = true;

			// base method
			base.Initialize ();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{
			GameScreens.Initialize (this);
			State = GameScreens.StartScreen;
			State.Entered (null);
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
		/// <param name="time">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime time)
		{
			// change game screen?
			if (State != State.NextState) {
				State.NextState.PostProcessingEffect = new FadeEffect (State.NextState, State);
				State.BeforeExit (time);
				State = State.NextState.NextState = State.NextState;
				State.Entered (time);
			}

			// global keyboard ans mouse input
			UpdateInput (time);

			// set the next game screen
			State.Update (time);

			// base method
			base.Update (time);
		}

		private void UpdateInput (GameTime time)
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
		/// <param name="time">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime time)
		{
			// current game screen
			State.Draw (time);

			// base class
			base.Draw (time);
		}

		public bool VSync
		{
			get {
				return Graphics.SynchronizeWithVerticalRetrace;
			}
			set {
				Graphics.SynchronizeWithVerticalRetrace = value;
				this.IsFixedTimeStep = value;
				Graphics.ApplyChanges ();
			}
		}

		private bool isFullscreen;

		public bool IsFullscreen
		{
			get {
				return isFullscreen;
			}
			set {
				if (value != isFullscreen) {
					Console.WriteLine ("Fullscreen Toggle");
					if (value) {
						Graphics.PreferredBackBufferWidth = Graphics.GraphicsDevice.DisplayMode.Width;
						Graphics.PreferredBackBufferHeight = Graphics.GraphicsDevice.DisplayMode.Height;
					}
					else {
						Graphics.PreferredBackBufferWidth = (int)Knot3Game.DefaultSize.X;
						Graphics.PreferredBackBufferHeight = (int)Knot3Game.DefaultSize.Y;
					}
					Graphics.ApplyChanges ();
					Graphics.ToggleFullScreen ();
					Graphics.ApplyChanges ();
					isFullscreen = value;
				}
			}
		}

		public static TimeSpan Time (Action action)
		{
			Stopwatch stopwatch = Stopwatch.StartNew ();
			action ();
			stopwatch.Stop ();
			return stopwatch.Elapsed;
		}
	}
}
