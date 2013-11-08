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
	public abstract class GameState : GameClass
	{
		public GameState (Game game)
			: base(game)
		{
		}

		protected GameState state {
			get { return this; }
		}
		
		public abstract void Initialize ();

		public abstract GameState Update (GameTime gameTime);

		public abstract void Draw (GameTime gameTime);

		public abstract void Unload ();
	}
	
	static class GameStates
	{
		public static ConstructionMode ConstructionMode;

		public static void Initialize(Game game) {
			ConstructionMode = new ConstructionMode(game);
			ConstructionMode.Initialize();
		}
	}
}

