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
using Knot3.GameObjects;
using Knot3.Utilities;

namespace Knot3.RenderEffects
{
	public class RenderEffectStack
	{
		private RenderEffect defaultEffect;

		public RenderEffectStack (RenderEffect defaultEffect)
		{
			this.defaultEffect = defaultEffect;
		}

		#region RenderEffect Stack

		private Stack<RenderEffect> activeEffects = new Stack<RenderEffect> ();

		public void Push (RenderEffect current)
		{
			activeEffects.Push (current);
		}

		public RenderEffect Pop ()
		{
			RenderEffect removed = activeEffects.Pop ();
			return removed;
		}

		public RenderEffect Current {
			get {
				if (activeEffects.Count > 0)
					return activeEffects.Peek ();
				else
					return defaultEffect;
			}
		}

		#endregion
	}

	/// <summary>
	/// Rendereffekte erben von der abstrakte Klasse RenderEffect und halten ein RenderTarget2D-Objekt,
	/// in das gezeichnet wird, während der RenderEffect aktiv ist.
	/// </summary>
	public abstract class RenderEffect
	{
		protected GameState state;
		private RenderTargetCache renderTarget;
		private Color background;
		private SpriteBatch spriteBatch;

		public RenderEffect (GameState state)
		{
			this.state = state;
			renderTarget = new RenderTargetCache (state.device);
			spriteBatch = new SpriteBatch (state.device);
			background = Color.Transparent;
		}

		public RenderTarget2D RenderTarget { get { return renderTarget.CurrentRenderTarget; } }

		public void Begin (GameTime gameTime)
		{
			Begin (Color.Transparent, gameTime);
		}

		public virtual void Begin (Color background, GameTime gameTime)
		{
			state.RenderEffects.Push (this);
			RenderTarget2D current = RenderTarget;
			state.device.PushRenderTarget (current);
			state.device.Clear (background);
			this.background = background;

			// set the stencil state
			state.device.DepthStencilState = DepthStencilState.Default;
			// Setting the other states isn't really necessary but good form
			state.device.BlendState = BlendState.Opaque;
			state.device.RasterizerState = RasterizerState.CullCounterClockwise;
			state.device.SamplerStates [0] = SamplerState.LinearWrap;
		}

		public virtual void End (GameTime gameTime)
		{
			state.device.PopRenderTarget ();
			spriteBatch.Begin (SpriteSortMode.Immediate, BlendState.NonPremultiplied);

			Draw (spriteBatch, gameTime);

			spriteBatch.End ();
			state.RenderEffects.Pop ();
		}

		public abstract void Draw (SpriteBatch spriteBatch, GameTime gameTime);

		public virtual void RemapModel (Model model)
		{
		}

		public virtual void DrawModel (GameModel model, GameTime gameTime)
		{
			foreach (ModelMesh mesh in model.Model.Meshes) {
				foreach (ModelMeshPart part in mesh.MeshParts) {
					if (part.Effect is BasicEffect) {
						BasicEffect effect = part.Effect as BasicEffect;

						// lighting
						if (Keys.L.IsHeldDown ()) {
							effect.LightingEnabled = false;
						} else {
							effect.EnableDefaultLighting ();  // Beleuchtung aktivieren
						}

						// matrices
						effect.World = model.WorldMatrix * state.camera.WorldMatrix;
						effect.View = state.camera.ViewMatrix;
						effect.Projection = state.camera.ProjectionMatrix;

						// colors
						if (model.BaseColor != Color.Transparent) {
							if (model.HighlightIntensity != 0f) {
								effect.DiffuseColor = model.BaseColor.Mix (model.HighlightColor, model.HighlightIntensity).ToVector3 ();
							} else {
								effect.DiffuseColor = model.BaseColor.ToVector3 ();
							}
						}
						if (background == Color.Transparent) {
							effect.Alpha = model.Alpha;
						} else {
							effect.DiffuseColor = new Color (effect.DiffuseColor).Mix (background, 1f - model.Alpha).ToVector3 ();
						}
						effect.FogEnabled = false;
					}
				}
			}

			foreach (ModelMesh mesh in model.Model.Meshes) {
				mesh.Draw ();
			}
		}
	}
}

