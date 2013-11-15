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
	public class BlurEffect : PostProcessing
	{
		private RenderTarget2D renderTarget;
		private static Effect testEffect;

		public BlurEffect (GameState state)
			: base(state)
		{
		}

		public override void LoadContent ()
		{
			testEffect = state.LoadEffect ("blur");
		}

		public override void Begin (GameTime gameTime)
		{
			if (renderTarget == null) {
				PresentationParameters pp = device.PresentationParameters;
				renderTarget = new RenderTarget2D (device, pp.BackBufferWidth, pp.BackBufferHeight,
			    	false, SurfaceFormat.Color, DepthFormat.Depth24);
			}
			device.SetRenderTarget (renderTarget);
		}

		public override void End (GameTime gameTime)
		{
			try {
				device.SetRenderTarget (null);
				device.Textures [1] = renderTarget;
				SpriteBatch spriteBatch = new SpriteBatch (device);
				spriteBatch.Begin (0, null, null, null, null, testEffect);
				testEffect.CurrentTechnique = testEffect.Techniques ["BlurTest1"];
                testEffect.Parameters["MatrixTransform"].SetValue(camera.ProjectionMatrix);
				spriteBatch.Draw (renderTarget, Vector2.Zero, Color.White); 
				foreach (EffectPass pass in testEffect.CurrentTechnique.Passes) {
					pass.Apply ();
				}
				spriteBatch.End ();
			} catch (NullReferenceException ex) {
				Console.WriteLine (ex.ToString ());
			}
		}
	}
}

