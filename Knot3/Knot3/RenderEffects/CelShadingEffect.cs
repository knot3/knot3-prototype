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

using Knot3.Core;
using Knot3.Utilities;
using Knot3.GameObjects;

namespace Knot3.RenderEffects
{
	/// <summary>
	/// Ein Cel-Shading-Effekt aus Basis der abstrakten RenderEffect -Klasse, der sowohl Modelle mit einem
	/// Toon-Shader zeichnet als auch einen Outline-Shader als Post-Processing-Effekt anwendet.
	/// </summary>
	public class CelShadingEffect : RenderEffect
	{
		Effect celShader;       // Toon shader effect
		Texture2D celMap;       // Texture map for cell shading
		Vector4 lightDirection; // Light source for toon shader

		Effect outlineShader;   // Outline shader effect
		float outlineThickness = 1.0f;  // current outline thickness
		float outlineThreshold = 0.2f;  // current edge detection threshold

		public CelShadingEffect (GameScreen screen)
		: base(screen)
		{
			/* Set our light direction for the cel-shader
			 */
			lightDirection = new Vector4 (0.0f, 0.0f, 1.0f, 1.0f);

			/* Load and initialize the cel-shader effect
			 */
			celShader = screen.LoadEffect ("CelShader");
			celShader.Parameters ["LightDirection"].SetValue (lightDirection);
			celMap = screen.content.Load<Texture2D> ("CelMap");
			celShader.Parameters ["Color"].SetValue (Color.Green.ToVector4 ());
			celShader.Parameters ["CelMap"].SetValue (celMap);

			/* Load and initialize the outline shader effect
			 */
			outlineShader = screen.LoadEffect ("OutlineShader");
			outlineShader.Parameters ["Thickness"].SetValue (outlineThickness);
			outlineShader.Parameters ["Threshold"].SetValue (outlineThreshold);
			outlineShader.Parameters ["ScreenSize"].SetValue (
			    new Vector2 (screen.viewport.Bounds.Width, screen.viewport.Bounds.Height));
		}

		public Color Color
		{
			get {
				return new Color (celShader.Parameters ["Color"].GetValueVector4 ());
			}
			set {
				celShader.Parameters ["Color"].SetValue (value.ToVector4 ());
			}
		}

		protected override void DrawRenderTarget (SpriteBatch spriteBatch, GameTime time)
		{
			spriteBatch.End ();
			spriteBatch.Begin (SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, outlineShader);
			spriteBatch.Draw (RenderTarget, Vector2.Zero, Color.White);
		}

		public override void RemapModel (Model model)
		{
			foreach (ModelMesh mesh in model.Meshes) {
				foreach (ModelMeshPart part in mesh.MeshParts) {
					part.Effect = celShader;
				}
			}
		}

		public override void DrawModel (GameModel model, GameTime time)
		{
			Camera camera = model.World.Camera;
			lightDirection = new Vector4 (-Vector3.Cross (Vector3.Normalize (camera.TargetDirection), camera.UpVector), 1);
			celShader.Parameters ["LightDirection"].SetValue (lightDirection);
			celShader.Parameters ["World"].SetValue (model.WorldMatrix * camera.WorldMatrix);
			celShader.Parameters ["InverseWorld"].SetValue (Matrix.Invert (model.WorldMatrix * camera.WorldMatrix));
			celShader.Parameters ["View"].SetValue (camera.ViewMatrix);
			celShader.Parameters ["Projection"].SetValue (camera.ProjectionMatrix);
			celShader.CurrentTechnique = celShader.Techniques ["ToonShader"];

			if (model.BaseColor != Color.Transparent) {
				if (model.HighlightIntensity != 0f) {
					Color = model.BaseColor.Mix (model.HighlightColor, model.HighlightIntensity);
				}
				else {
					Color = model.BaseColor;
				}
			}

			foreach (ModelMesh mesh in model.Model.Meshes) {
				mesh.Draw ();
			}
		}
	}
}
