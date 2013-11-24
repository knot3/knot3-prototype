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
	public class RenderTargetCache : GameClass
	{
		private Dictionary<Point, RenderTarget2D> renderTargets;

		public RenderTargetCache (GameState state)
			: base(state)
		{
			renderTargets = new Dictionary<Point, RenderTarget2D> ();
		}

		public RenderTarget2D CurrentRenderTarget {
			get {
				PresentationParameters pp = device.PresentationParameters;
				Point resolution = new Point (pp.BackBufferWidth, pp.BackBufferHeight);
				if (!renderTargets.ContainsKey (resolution)) {
					renderTargets [resolution] = new RenderTarget2D (device, resolution.X, resolution.Y,
                    false, SurfaceFormat.Color, DepthFormat.Depth24, 1, RenderTargetUsage.PreserveContents);
				}
				return renderTargets [resolution];
			}
		}
	}

	public static class RenderTargets
	{
		#region RenderTarget Stack

		private static Stack<RenderTarget2D> RenderTargetStack = new Stack<RenderTarget2D> ();

		public static void PushRenderTarget (this GraphicsDevice device, RenderTarget2D current)
		{
			RenderTargetStack.Push (current);
			device.SetRenderTarget (current);
		}

		public static RenderTarget2D PopRenderTarget (this GraphicsDevice device)
		{
			RenderTarget2D removed = RenderTargetStack.Pop ();
			if (RenderTargetStack.Count () > 0)
				device.SetRenderTarget (RenderTargetStack.Peek ());
			else
				device.SetRenderTarget (null);
			return removed;
		}

		#endregion
	}

	public abstract class RenderEffect : GameClass
	{
		private static Stack<RenderEffect> ActiveRenderEffects = new Stack<RenderEffect> ();
		private RenderTargetCache renderTarget;

		public RenderEffect (GameState state)
			: base(state)
		{
			renderTarget = new RenderTargetCache (state);
		}

		public RenderTarget2D RenderTarget { get { return renderTarget.CurrentRenderTarget; } }

		public void Begin (GameTime gameTime)
		{
			Begin (Color.Transparent, gameTime);
		}

		public virtual void Begin (Color background, GameTime gameTime)
		{
			ActiveRenderEffects.Push (this);
			RenderTarget2D current = RenderTarget;
			device.PushRenderTarget (current);
			device.Clear (background);

			device.DepthStencilState = DepthStencilState.Default;

			/* Setting the other states isn't really necessary but good form
             */
			device.BlendState = BlendState.Opaque;
			device.RasterizerState = RasterizerState.CullCounterClockwise;
			device.SamplerStates [0] = SamplerState.LinearWrap;
		}

		public virtual void End (GameTime gameTime)
		{
			try {
				device.PopRenderTarget ();
				//device.Textures[1] = renderTarget;
				SpriteBatch spriteBatch = new SpriteBatch (device);
				spriteBatch.Begin (SpriteSortMode.Immediate, BlendState.NonPremultiplied);

				Draw (spriteBatch, gameTime);

				spriteBatch.End ();
				ActiveRenderEffects.Pop ();
			} catch (NullReferenceException ex) {
				Console.WriteLine (ex.ToString ());
			}
		}

		public abstract void Draw (SpriteBatch spriteBatch, GameTime gameTime);

		public virtual void RemapModel (Model model)
		{
		}

		public virtual void RenderModel (Model model, Matrix view, Matrix proj, Matrix world)
		{
			foreach (ModelMesh mesh in model.Meshes) {
				foreach (ModelMeshPart part in mesh.MeshParts) {
					if (part.Effect is BasicEffect) {
						BasicEffect effect = part.Effect as BasicEffect;
						if (Keys.L.IsHeldDown ()) {
							effect.LightingEnabled = false;
						} else {
							effect.EnableDefaultLighting ();  // Beleuchtung aktivieren
						}
						effect.World = world;
						effect.View = camera.ViewMatrix;
						effect.Projection = camera.ProjectionMatrix;
					}
				}
			}
		}
	}
	
	public class NoRenderEffect : RenderEffect
	{
		public NoRenderEffect (GameState state)
			: base(state)
		{
		}

		public override void Draw (SpriteBatch spriteBatch, GameTime gameTime)
		{
			spriteBatch.Draw (RenderTarget, Vector2.Zero, Color.White);
		}
	}
}

