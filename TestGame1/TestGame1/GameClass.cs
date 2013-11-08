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
	public abstract class GameClass
	{
		protected Game game;

		protected GraphicsDevice device {
			get { return game.GraphicsDevice; }
		}

		protected GraphicsDeviceManager graphics {
			get { return game.Graphics; }
		}
		
		private GameState _state;

		protected GameState state {
			get { return _state != null ? _state : game.State; }
		}

		protected virtual Camera camera {
			get { return state.camera; }
			set { }
		}

		protected virtual Input input {
			get { return state.input; }
			set { }
		}

		protected virtual World world {
			get { return state.world; }
			set { }
		}

		public GameClass (GameState state)
		{
 			this.game = state.game;
			this._state = state;
		}

		public GameClass (Game game)
		{
			this.game = game;
			this._state = null;
		}
	}
}

