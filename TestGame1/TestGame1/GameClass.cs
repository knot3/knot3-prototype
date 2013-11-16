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
		public GameClass (GameState state)
		{
			this.state = state;
		}

		protected GameState state { get; private set; }

		protected GraphicsDeviceManager graphics { get { return state.graphics; } }

		protected GraphicsDevice device { get { return state.device; } }

		protected Viewport viewport { get { return state.device.Viewport; } }

		protected ContentManager content { get { return state.content; } }

		protected virtual Camera camera {
			get { return state.camera; }
			set {}
		}

		protected virtual Input input {
			get { return state.input; }
			set {}
		}

		protected virtual World world {
			get { return state.world; }
			set {}
		}
	}
}

