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

namespace Knot3.RenderEffects
{
	public class FadeEffect : RenderEffect
	{
		private RenderTarget2D lastFrame;
		private float alpha;

		public FadeEffect (GameState state, GameState oldState)
			: base(state)
		{
			if (oldState != null) {
				lastFrame = oldState.PostProcessing.RenderTarget;
				alpha = 1.0f;
			}
		}

		public override void Draw (SpriteBatch spriteBatch, GameTime gameTime)
		{
			if (lastFrame != null) {
				alpha -= 0.05f;
				// Console.WriteLine ("alpha=" + alpha);
				spriteBatch.Draw (lastFrame, Vector2.Zero, new Rectangle (0, 0, viewport.Width, viewport.Height), Color.White);
			}
			if (alpha <= 0) {
				lastFrame = null;
				alpha = 0.0f;
			}
			
			spriteBatch.Draw (RenderTarget, Vector2.Zero, Color.White * (1 - alpha));
		}

		public bool IsFinished { get { return alpha <= 0; } }
	}
}
