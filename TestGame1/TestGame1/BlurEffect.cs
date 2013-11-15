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
		private Dictionary<Point, RenderTarget2D> renderTargets;
		private RenderTarget2D renderTarget;
		private static Effect testEffect;

		public BlurEffect (GameState state)
			: base(state)
		{
		}

		public override void LoadContent ()
		{
			renderTargets = new Dictionary<Point, RenderTarget2D>();
			testEffect = state.LoadEffect ("blur");
		}

		public override void Begin (GameTime gameTime)
		{
			PresentationParameters pp = device.PresentationParameters;
			Point resolution = new Point (pp.BackBufferWidth, pp.BackBufferHeight);
			if (!renderTargets.ContainsKey (resolution)) {
				renderTargets [resolution] = new RenderTarget2D (device, resolution.X, resolution.Y,
                    false, SurfaceFormat.Color, DepthFormat.Depth24, 1, RenderTargetUsage.DiscardContents);
			}
			renderTarget = renderTargets [resolution];
			device.SetRenderTarget (renderTarget);
		}

		public override void End (GameTime gameTime)
		{
			try {
				device.SetRenderTarget (null);
				device.Clear (Color.Black);
				//device.Textures[1] = renderTarget;
				SpriteBatch spriteBatch = new SpriteBatch (device);
				spriteBatch.Begin (SpriteSortMode.Immediate, null);
				testEffect.CurrentTechnique = testEffect.Techniques ["BlurTest1"];
				//testEffect.Parameters["World"].SetValue(camera.WorldMatrix);
				//testEffect.Parameters["View"].SetValue(camera.ViewMatrix);
				//testEffect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
				foreach (EffectPass pass in testEffect.CurrentTechnique.Passes) {
					pass.Apply ();
				}
				spriteBatch.Draw (renderTarget, Vector2.Zero, Color.White); 
				spriteBatch.End ();
			} catch (NullReferenceException ex) {
				Console.WriteLine (ex.ToString ());
			}
		}
	}
}

