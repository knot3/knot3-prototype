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

using Knot3.Core;
using Knot3.Utilities;

namespace Knot3.RenderEffects
{
	public class BlurEffect : RenderEffect
	{
		private static Effect testEffect;

		public BlurEffect (GameScreen screen)
		: base(screen)
		{
			testEffect = screen.LoadEffect ("blur");
		}

		public override void Begin (Color background, GameTime time)
		{
			base.Begin (background, time);
		}

		public override void End (GameTime time)
		{
			base.End (time);
		}

		protected override void DrawRenderTarget (SpriteBatch spriteBatch, GameTime time)
		{
			testEffect.CurrentTechnique = testEffect.Techniques ["BlurTest1"];
			//testEffect.Parameters["World"].SetValue(camera.WorldMatrix);
			//testEffect.Parameters["View"].SetValue(camera.ViewMatrix);
			//testEffect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
			foreach (EffectPass pass in testEffect.CurrentTechnique.Passes) {
				pass.Apply ();
			}

			spriteBatch.Draw (RenderTarget, Vector2.Zero, Color.White);
		}
	}
}
