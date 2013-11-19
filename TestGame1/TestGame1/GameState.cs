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
	public abstract class GameState
	{
		public Game game;

		public PostProcessing PostProcessing { get; set; }

		public GameState (Game game)
		{
			this.game = game;
			this.PostProcessing = new NoPostProcessing (this);
			this.PostProcessing.LoadContent ();
		}

		public Input input { get; protected set; }

		public Camera camera { get; protected set; }

		public World world { get; protected set; }

		public GraphicsDeviceManager graphics { get { return game.Graphics; } }

		public GraphicsDevice device { get { return game.GraphicsDevice; } }

		public Viewport viewport { get { return device.Viewport; } }

		public ContentManager content { get { return game.Content; } }
		
		public abstract void Initialize ();

		public abstract GameState Update (GameTime gameTime);

		public abstract void Draw (GameTime gameTime);

		public abstract void Unload ();
	}
	
	static class GameStates
	{
		public static CreativeMode CreativeMode;
		public static StartScreen StartScreen;
		public static OptionScreen OptionScreen;
		public static VideoOptionScreen VideoOptionScreen;

		public static void Initialize (Game game)
		{
			CreativeMode = new CreativeMode (game);
			StartScreen = new StartScreen (game);
			OptionScreen = new OptionScreen (game);
			VideoOptionScreen = new VideoOptionScreen (game);
			CreativeMode.Initialize ();
			StartScreen.Initialize ();
			OptionScreen.Initialize ();
			VideoOptionScreen.Initialize ();
		}
	}
}

