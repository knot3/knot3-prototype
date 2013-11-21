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

namespace TestGame1
{
	public class CelShadingEffect : RenderTargetPostProcessing
	{
		public Effect celShader;       // Toon shader effect
		Texture2D celMap;       // Texture map for cell shading
		Texture2D colorMap;
		Vector4 lightDirection; // Light source for toon shader

		Effect outlineShader;   // Outline shader effect
		float outlineThickness = 0.8f;  // current outline thickness
		float outlineThreshold = 0.2f;  // current edge detection threshold


		public CelShadingEffect (GameState state)
			: base(state)
		{
			/* Set our light direction for the cel-shader
             */
			lightDirection = new Vector4 (0.0f, 0.0f, 1.0f, 1.0f);

			/* Load and initialize the cel-shader effect
             */
			colorMap = content.Load<Texture2D> ("ColorMap");
			celShader = state.LoadEffect ("CelShader");
			celShader.Parameters ["LightDirection"].SetValue (lightDirection);
			celMap = content.Load<Texture2D> ("CelMap");
			celShader.Parameters ["Color"].SetValue (Color.Green.ToVector4());
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
			celShader.Parameters ["Color"].SetValue (color.ToVector4());
		}

		public Matrix[] LoadBones (Model model)
		{
			Matrix[] bones = new Matrix[model.Bones.Count];
			model.CopyAbsoluteBoneTransformsTo (bones);
			return bones;
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
					part.Effect = (state.PostProcessing as CelShadingEffect).celShader;
				}
			}
		}

		public override void RenderModel (Model model, Matrix view, Matrix proj, Matrix world)
        {
            lightDirection = new Vector4(-Vector3.Cross(Vector3.Normalize(camera.TargetVector), camera.UpVector), 1);
            celShader.Parameters["LightDirection"].SetValue(lightDirection);
			celShader.Parameters ["Projection"].SetValue (camera.ProjectionMatrix);
			celShader.Parameters ["View"].SetValue (camera.ViewMatrix);

			celShader.Parameters ["World"].SetValue (world);
			celShader.Parameters ["InverseWorld"].SetValue (Matrix.Invert (world));
			celShader.CurrentTechnique = celShader.Techniques ["ToonShader"];
		}
	}
}

