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
	public abstract class GameState
	{
		public Game game;

		public GameState NextState { get; set; }

		public GameState (Game game)
		{
			this.game = game;
			this.NextState = this;
			this.PostProcessing = new NoRenderEffect (this);
		}

		public Input input { get; protected set; }

		public Camera camera { get; protected set; }

		public World world { get; protected set; }

		public GraphicsDeviceManager graphics { get { return game.Graphics; } }

		public GraphicsDevice device { get { return game.GraphicsDevice; } }

		public Viewport viewport { get { return device.Viewport; } }

		public ContentManager content { get { return game.Content; } }

		public RenderEffect PostProcessing;

		public abstract void Initialize ();

		public abstract void Update (GameTime gameTime);

		public abstract void Draw (GameTime gameTime);

		public abstract void Unload ();

		public abstract void Activate(GameTime gameTime);

		public abstract void Deactivate(GameTime gameTime);
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

