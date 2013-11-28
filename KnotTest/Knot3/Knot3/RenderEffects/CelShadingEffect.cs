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
	public class CelShadingEffect : RenderEffect
	{
		Effect celShader;       // Toon shader effect
		Texture2D celMap;       // Texture map for cell shading
		Vector4 lightDirection; // Light source for toon shader

		Effect outlineShader;   // Outline shader effect
		float outlineThickness = 1.0f;  // current outline thickness
		float outlineThreshold = 0.2f;  // current edge detection threshold


		public CelShadingEffect (GameState state)
			: base(state)
		{
			/* Set our light direction for the cel-shader
             */
			lightDirection = new Vector4 (0.0f, 0.0f, 1.0f, 1.0f);

			/* Load and initialize the cel-shader effect
             */
			celShader = state.LoadEffect ("CelShader");
			celShader.Parameters ["LightDirection"].SetValue (lightDirection);
			celMap = content.Load<Texture2D> ("CelMap");
			celShader.Parameters ["Color"].SetValue (Color.Green.ToVector4 ());
			celShader.Parameters ["CelMap"].SetValue (celMap);

			/* Load and initialize the outline shader effect
             */
			outlineShader = state.LoadEffect ("OutlineShader");
			outlineShader.Parameters ["Thickness"].SetValue (outlineThickness);
			outlineShader.Parameters ["Threshold"].SetValue (outlineThreshold);
			outlineShader.Parameters ["ScreenSize"].SetValue (
                new Vector2 (viewport.Bounds.Width, viewport.Bounds.Height));
		}

		public void SetColor (Color color)
		{
			celShader.Parameters ["Color"].SetValue (color.ToVector4 ());
		}

		public override void Draw (SpriteBatch spriteBatch, GameTime gameTime)
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

		public override void DrawModel (GameModel model, GameTime gameTime)
		{
			lightDirection = new Vector4 (-Vector3.Cross (Vector3.Normalize (camera.TargetDirection), camera.UpVector), 1);
			celShader.Parameters ["LightDirection"].SetValue (lightDirection);
			celShader.Parameters ["World"].SetValue (model.WorldMatrix * camera.WorldMatrix);
			celShader.Parameters ["InverseWorld"].SetValue (Matrix.Invert (model.WorldMatrix * camera.WorldMatrix));
			celShader.Parameters ["View"].SetValue (camera.ViewMatrix);
			celShader.Parameters ["Projection"].SetValue (camera.ProjectionMatrix);
			celShader.CurrentTechnique = celShader.Techniques ["ToonShader"];

			if (model.BaseColor != Color.Transparent) {
				if (model.HighlightIntensity != 0f) {
					SetColor (model.BaseColor.Mix (model.HighlightColor, model.HighlightIntensity));
				} else {
					SetColor (model.BaseColor);
				}
			}

			foreach (ModelMesh mesh in model.Model.Meshes) {
				mesh.Draw ();
			}
		}
	}
}

