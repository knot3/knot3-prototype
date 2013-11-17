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
	public class BlurEffect : RenderTargetPostProcessing
	{
		private static Effect testEffect;

		public BlurEffect (GameState state)
			: base(state)
		{
		}

		public override void LoadContent ()
		{
			testEffect = state.LoadEffect ("blur");
			base.LoadContent ();
		}

		public override void Begin (GameTime gameTime)
		{
			base.Begin (gameTime);
		}

		public override void End (GameTime gameTime)
		{
			base.End (gameTime);
		}

		public override void Draw (GameTime gameTime)
		{
			testEffect.CurrentTechnique = testEffect.Techniques ["BlurTest1"];
			//testEffect.Parameters["World"].SetValue(camera.WorldMatrix);
			//testEffect.Parameters["View"].SetValue(camera.ViewMatrix);
			//testEffect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
			foreach (EffectPass pass in testEffect.CurrentTechnique.Passes) {
				pass.Apply ();
			}
		}
	}
}

