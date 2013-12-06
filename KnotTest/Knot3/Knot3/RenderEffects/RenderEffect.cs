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
		/// Der zugewiesene GameState. Dieser Effekt kann nur innerhalb dieses GameState's verwendet werden.
		/// </summary>
		protected GameState state;
		private RenderTargetCache renderTarget;
		private Color background;
		private SpriteBatch spriteBatch;

		/// <summary>
		/// Erstellt einen neuen RenderEffect.
		/// </summary>
		/// <param name='state'>
		/// Game State.
		/// </param>
		public RenderEffect (GameState state)
		{
			this.state = state;
			renderTarget = new RenderTargetCache (state.device);
			spriteBatch = new SpriteBatch (state.device);
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
			state.device.PopRenderTarget ();
			spriteBatch.Begin (SpriteSortMode.Immediate, BlendState.NonPremultiplied);

			Draw (spriteBatch, gameTime);

			spriteBatch.End ();
			state.RenderEffects.Pop ();
		}

		public abstract void Draw (SpriteBatch spriteBatch, GameTime gameTime);

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
						BasicEffect effect = part.Effect as BasicEffect;

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

			foreach (ModelMesh mesh in model.Model.Meshes) {
				mesh.Draw ();
			}
		}
	}
}

