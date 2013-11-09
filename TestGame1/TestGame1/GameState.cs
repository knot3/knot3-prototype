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
		protected Game game;

		public GameState (Game game)
		{
			this.game = game;
		}

		public Input input { get; protected set; }

		public Camera camera { get; protected set; }

		public World world { get; protected set; }

		public GraphicsDevice device { get { return game.GraphicsDevice; } }

		public GraphicsDeviceManager graphics { get { return game.Graphics; } }

		public ContentManager content { get { return game.Content; } }
		
		public abstract void Initialize ();

		public abstract GameState Update (GameTime gameTime);

		public abstract void Draw (GameTime gameTime);

		public abstract void Unload ();
	}
	
	static class GameStates
	{
		public static KnotMode KnotMode;
		public static StartScreen StartScreen;

		public static void Initialize (Game game)
		{
			KnotMode = new KnotMode (game);
			StartScreen = new StartScreen (game);
			KnotMode.Initialize ();
			StartScreen.Initialize ();
		}
	}
}

