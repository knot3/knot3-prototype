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

using Knot3.GameObjects;
using Knot3.RenderEffects;
using Knot3.Settings;
using Knot3.CreativeMode;

namespace Knot3
{
	/// <summary>
	/// Do I really need to explain this?
	/// For example the start screen, the creative and challenge mode, and the options menu are game states.
	/// </summary>
	public abstract class GameState
	{
		/// <summary>
		/// The game.
		/// </summary>
		public Game game;

		/// <summary>
		/// Gets or sets the next game state.
		/// </summary>
		/// <value>
		/// The next game state.
		/// </value>
		public GameState NextState { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Knot3.GameState"/> class.
		/// </summary>
		/// <param name='game'>
		/// The Game.
		/// </param>
		public GameState (Game game)
		{
			this.game = game;
			this.NextState = this;
			this.RenderEffects = new RenderEffectStack (this);
			this.PostProcessing = new NoEffect (this);
		}

		/// <summary>
		/// Gets or sets the input handler for this game state.
		/// </summary>
		/// <value>
		/// The input handler.
		/// </value>
		public Input input { get; protected set; }

		/// <summary>
		/// Gets or sets the camera for this game state.
		/// </summary>
		/// <value>
		/// The camera.
		/// </value>
		public Camera camera { get; protected set; }

		/// <summary>
		/// Gets or sets the game world for this game state.
		/// </summary>
		/// <value>
		/// The game world.
		/// </value>
		public World world { get; protected set; }

		/// <summary>
		/// Gets the graphics device manager for this game state.
		/// </summary>
		/// <value>
		/// The graphics device manager.
		/// </value>
		public GraphicsDeviceManager graphics { get { return game.Graphics; } }

		/// <summary>
		/// Gets the graphics device for this game state.
		/// </summary>
		/// <value>
		/// The graphics device.
		/// </value>
		public GraphicsDevice device { get { return game.GraphicsDevice; } }

		/// <summary>
		/// Gets the viewport for this game state.
		/// </summary>
		/// <value>
		/// The viewport.
		/// </value>
		public Viewport viewport { get { return device.Viewport; } }

		/// <summary>
		/// Gets the content manager for this game state.
		/// </summary>
		/// <value>
		/// The content manager.
		/// </value>
		public ContentManager content { get { return game.Content; } }

		/// <summary>
		/// Gets the currently active render effects for this game state.
		/// </summary>
		/// <value>
		/// The currently active render effects.
		/// </value>
		public RenderEffectStack RenderEffects { get; private set; }

		/// <summary>
		/// The post processing effect of this game state.
		/// </summary>
		public RenderEffect PostProcessing;

		/// <summary>
		/// Initialize the game state.
		/// </summary>
		public abstract void Initialize ();

		/// <summary>
		/// Update the game state.
		/// </summary>
		/// <param name='gameTime'>
		/// The Game time.
		/// </param>
		public abstract void Update (GameTime gameTime);

		/// <summary>
		/// Draw the game state.
		/// </summary>
		/// <param name='gameTime'>
		/// The Game time.
		/// </param>
		public abstract void Draw (GameTime gameTime);

		/// <summary>
		/// Unload the game state.
		/// </summary>
		public abstract void Unload ();

		/// <summary>
		/// This is run when this game state becomes the active game state.
		/// </summary>
		/// <param name='gameTime'>
		/// The Game time.
		/// </param>
		public abstract void Activate (GameTime gameTime);

		/// <summary>
		/// This is run when this game state is about to becomes inactive.
		/// </summary>
		/// <param name='gameTime'>
		/// The Game time.
		/// </param>
		public abstract void Deactivate (GameTime gameTime);
	}
	
	static class GameStates
	{
		public static CreativeModeScreen CreativeMode;
		public static StartScreen StartScreen;
		public static OptionScreen OptionScreen;
		public static VideoOptionScreen VideoOptionScreen;
		public static LoadSavegameScreen LoadSavegameScreen;

		public static void Initialize (Game game)
		{
			CreativeMode = new CreativeModeScreen (game);
			StartScreen = new StartScreen (game);
			OptionScreen = new OptionScreen (game);
			VideoOptionScreen = new VideoOptionScreen (game);
			LoadSavegameScreen = new LoadSavegameScreen (game);
			CreativeMode.Initialize ();
			StartScreen.Initialize ();
			OptionScreen.Initialize ();
			VideoOptionScreen.Initialize ();
			LoadSavegameScreen.Initialize ();
		}
	}
}

