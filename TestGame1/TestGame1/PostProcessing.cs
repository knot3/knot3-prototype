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

	public class NoPostProcessing : PostProcessing
	{
		public NoPostProcessing (GameState state)
			: base(state)
		{
		}

		public override void LoadContent ()
		{
		}

		public override void Begin (GameTime gameTime)
		{
		}

		public override void End (GameTime gameTime)
		{
		}
	}

	public abstract class RenderTargetPostProcessing : PostProcessing
	{
		private Dictionary<Point, RenderTarget2D> renderTargets;
		private RenderTarget2D renderTarget;

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

				Draw (gameTime);

				spriteBatch.Draw (renderTarget, Vector2.Zero, Color.White); 
				spriteBatch.End ();
			} catch (NullReferenceException ex) {
				Console.WriteLine (ex.ToString ());
			}
		}

		public abstract void Draw (GameTime gameTime);
	}
}

