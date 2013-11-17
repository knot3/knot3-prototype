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

		public abstract void LoadContent ();

		public abstract void Begin (GameTime gameTime);

		public abstract void End (GameTime gameTime);
	}

	public class NoPostProcessing : RenderTargetPostProcessing
	{
		public NoPostProcessing (GameState state)
			: base(state)
		{
		}

		public override void Draw (SpriteBatch spriteBatch, GameTime gameTime)
		{
			spriteBatch.Draw (RenderTarget, Vector2.Zero, Color.White);
		}
	}

	public abstract class RenderTargetPostProcessing : PostProcessing
	{
		private Dictionary<Point, RenderTarget2D> renderTargets;

		public RenderTarget2D RenderTarget { get; private set; }

		public RenderTargetPostProcessing (GameState state)
			: base(state)
		{
		}

		public override void LoadContent ()
		{
			renderTargets = new Dictionary<Point, RenderTarget2D> ();
		}

		public override void Begin (GameTime gameTime)
		{
			PresentationParameters pp = device.PresentationParameters;
			Point resolution = new Point (pp.BackBufferWidth, pp.BackBufferHeight);
			if (!renderTargets.ContainsKey (resolution)) {
				renderTargets [resolution] = new RenderTarget2D (device, resolution.X, resolution.Y,
                    false, SurfaceFormat.Color, DepthFormat.Depth24, 1, RenderTargetUsage.DiscardContents);
			}
			RenderTarget = renderTargets [resolution];
			device.SetRenderTarget (RenderTarget);
		}

		public override void End (GameTime gameTime)
		{
			try {
				device.SetRenderTarget (null);
				device.Clear (Color.Black);
				//device.Textures[1] = renderTarget;
				SpriteBatch spriteBatch = new SpriteBatch (device);
				spriteBatch.Begin (SpriteSortMode.Immediate, BlendState.NonPremultiplied);

				Draw (spriteBatch, gameTime);

				spriteBatch.End ();
			} catch (NullReferenceException ex) {
				Console.WriteLine (ex.ToString ());
			}
		}

		public abstract void Draw (SpriteBatch spriteBatch, GameTime gameTime);
	}

	public class FadeEffect : RenderTargetPostProcessing
	{
		private RenderTarget2D lastFrame;
		private float alpha;

		public FadeEffect (GameState state, GameState oldState)
			: base(state)
		{
			if (oldState != null && oldState.PostProcessing is RenderTargetPostProcessing) {
				lastFrame = (oldState.PostProcessing as RenderTargetPostProcessing).RenderTarget;
				alpha = 1.0f;
			}
		}

		public override void Draw (SpriteBatch spriteBatch, GameTime gameTime)
		{
			spriteBatch.Draw (RenderTarget, Vector2.Zero, Color.White * (1-alpha));

			if (lastFrame != null) {
				alpha -= 0.05f;
				Console.WriteLine ("alpha=" + alpha);
				spriteBatch.Draw (lastFrame, Vector2.Zero, new Rectangle (0, 0, viewport.Width, viewport.Height), Color.White * alpha);
			}
			if (alpha <= 0) {
				lastFrame = null;
				alpha = 0.0f;
			}
		}
	}
}

