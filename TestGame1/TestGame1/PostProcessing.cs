using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace TestGame1
{
	public abstract class PostProcessing : GameClass
	{
		public PostProcessing (GameState state)
			: base(state)
		{
		}

		public virtual void LoadContent ()
		{
		}

		public virtual void Begin (GameTime gameTime)
		{
		}

		public virtual void End (GameTime gameTime)
		{
		}
	}

	public class NoPostProcessing : PostProcessing
	{
		public NoPostProcessing (GameState state)
			: base(state)
		{
		}
	}
}

