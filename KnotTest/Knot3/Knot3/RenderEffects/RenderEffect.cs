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
	/// <summary>
	/// Implementiert einen Stack für Rendereffekte (Instanzen von IRenderEffect), damit eine verschachtelte
	/// Anwendung von RenderEffect's erleichtert wird.
	/// </summary>
	public class RenderEffectStack
	{
		private IRenderEffect defaultEffect;

		public RenderEffectStack (IRenderEffect defaultEffect)
		{
			this.defaultEffect = defaultEffect;
		}

		#region RenderEffect Stack

		private Stack<IRenderEffect> activeEffects = new Stack<IRenderEffect> ();

		public void Push (IRenderEffect current)
		{
			activeEffects.Push (current);
		}

		public IRenderEffect Pop ()
		{
			IRenderEffect removed = activeEffects.Pop ();
			return removed;
		}

		public IRenderEffect Current {
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
	/// in das gezeichnet wird, während der RenderEffect aktiv ist. Sie implementieren außerdem das Interface
	/// IRenderEffect.
	/// </summary>
	public abstract class RenderEffect : IRenderEffect
	{
		/// <summary>
		/// Der zugewiesene GameScreen. Dieser Effekt kann nur innerhalb dieses GameScreen's verwendet werden.
		/// </summary>
		protected GameScreen screen;
		private RenderTargetCache renderTarget;
		private Color background;
		private SpriteBatch spriteBatch;

		/// <summary>
		/// Erstellt einen neuen RenderEffect.
		/// </summary>
		/// <param name='screen'>
		/// Game State.
		/// </param>
		public RenderEffect (GameScreen screen)
		{
			this.screen = screen;
			renderTarget = new RenderTargetCache (screen.device);
			spriteBatch = new SpriteBatch (screen.device);
			background = Color.Transparent;
		}

		/// <summary>
		/// Die Textur, in die dieses RenderTarget rendert.
		/// </summary>
		/// <value>
		/// The render target.
		/// </value>
		public RenderTarget2D RenderTarget { get { return renderTarget.CurrentRenderTarget; } }

		/// <summary>
		/// Startet den Rendereffekt. Das bisher von XNA benutzte RenderTarget wird auf einem Stack gespeichert
		/// (RenderTargets), und unser RenderTarget wird als aktuelles RenderTarget gesetzt.
		/// </summary>
		/// <param name='gameTime'>
		/// Game time.
		/// </param>
		public void Begin (GameTime gameTime)
		{
			Begin (Color.Transparent, gameTime);
		}

		public virtual void Begin (Color background, GameTime gameTime)
		{
			screen.RenderEffects.Push (this);
			RenderTarget2D current = RenderTarget;
			screen.device.PushRenderTarget (current);
			screen.device.Clear (background);
			this.background = background;

			// set the stencil screen
			screen.device.DepthStencilState = DepthStencilState.Default;
			// Setting the other screens isn't really necessary but good form
			screen.device.BlendState = BlendState.Opaque;
			screen.device.RasterizerState = RasterizerState.CullCounterClockwise;
			screen.device.SamplerStates [0] = SamplerState.LinearWrap;
		}

		/// <summary>
		/// Beendet den Rendereffekt. Dabei wird das von XNA benutzte RenderTarget wieder auf das in der Begin()-Methode
		/// auf einem Stack gesicherte übergeordnete RenderTarget festgelegt. Dann wird unser RendetTarget auf
		/// das übergeordneten Rendertarget gezeichnet.
		/// </summary>
		/// <param name='gameTime'>
		/// Game time.
		/// </param>
		public virtual void End (GameTime gameTime)
		{
			if (!Overlay.Profiler.ContainsKey ("RenderEffect"))
				Overlay.Profiler ["RenderEffect"] = 0;
			Overlay.Profiler ["RenderEffect"] += Knot3.Core.Game.Time (() => {

				screen.device.PopRenderTarget ();
				spriteBatch.Begin (SpriteSortMode.Immediate, BlendState.NonPremultiplied);

				DrawRenderTarget (spriteBatch, gameTime);

				spriteBatch.End ();
				screen.RenderEffects.Pop ();

			}
			).TotalMilliseconds;
		}
		
		public void DrawLastFrame (GameTime gameTime)
		{
			spriteBatch.Begin (SpriteSortMode.Immediate, BlendState.NonPremultiplied);
			DrawRenderTarget (spriteBatch, gameTime);
			spriteBatch.End ();
		}

		protected abstract void DrawRenderTarget (SpriteBatch spriteBatch, GameTime gameTime);

		/// <summary>
		/// Die XNA-3D-Modelle haben standardmäßig einen BasicEffect-Shader als zu verwendenden Shader zugewiesen.
		/// Hier wird dieser Shader durch den Shader des Rendereffekts überschrieben.
		/// </summary>
		/// <param name='model'>
		/// Model.
		/// </param>
		public virtual void RemapModel (Model model)
		{
		}

		/// <summary>
		/// Zeichnet ein 3D-Modell mit diesem Rendereffekt.
		/// </summary>
		/// <param name='model'>
		/// Model.
		/// </param>
		/// <param name='gameTime'>
		/// Game time.
		/// </param>
		public virtual void DrawModel (GameModel model, GameTime gameTime)
		{
			foreach (ModelMesh mesh in model.Model.Meshes) {
				foreach (ModelMeshPart part in mesh.MeshParts) {
					if (part.Effect is BasicEffect) {
						ModifyBasicEffect (part.Effect as BasicEffect, model);
					}
				}
			}

			foreach (ModelMesh mesh in model.Model.Meshes) {
				mesh.Draw ();
			}
		}

		protected void ModifyBasicEffect (BasicEffect effect, GameModel model)
		{
			// lighting
			if (Keys.L.IsHeldDown ()) {
				effect.LightingEnabled = false;
			} else {
				effect.EnableDefaultLighting ();  // Beleuchtung aktivieren
			}

			// matrices
			effect.World = model.WorldMatrix * model.World.Camera.WorldMatrix;
			effect.View = model.World.Camera.ViewMatrix;
			effect.Projection = model.World.Camera.ProjectionMatrix;

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

