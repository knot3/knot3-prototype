using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace TestGame1
{
	public class TexturedRectangle
	{
		private GraphicsDevice device;
		private Game game;
		public Vector3 Origin;
		public Vector3 UpperLeft;
		public Vector3 LowerLeft;
		public Vector3 UpperRight;
		public Vector3 LowerRight;
		public Vector3 Normal;
		public Vector3 Up;
		public Vector3 Left;
		public VertexPositionNormalTexture[] Vertices;
		public short[] Indexes;
		private Texture2D texture;

		public TexturedRectangle (GraphicsDevice device, Game game, Vector3 origin, Vector3 normal, Vector3 up, 
				float width, float height)
		{
			this.device = device;
			this.game = game;

			Vertices = new VertexPositionNormalTexture[4];
			Indexes = new short[6];
			Origin = origin;
			Normal = normal;
			Up = up;

			// Calculate the quad corners
			Left = Vector3.Cross (normal, Up);
			Vector3 uppercenter = (Up * height / 2) + origin;
			UpperLeft = uppercenter + (Left * width / 2);
			UpperRight = uppercenter - (Left * width / 2);
			LowerLeft = UpperLeft - (Up * height);
			LowerRight = UpperRight - (Up * height);

			FillVertices ();
		}
        
		private void FillVertices ()
		{
			try {
				texture = game.Content.Load<Texture2D> ("background");
			} catch (ContentLoadException ex) {
				Console.WriteLine (ex.ToString ());
				texture = null;
				return;
			}

			// Fill in texture coordinates to display full texture
			// on quad
			Vector2 textureUpperLeft = new Vector2 (0.0f, 0.0f);
			Vector2 textureUpperRight = new Vector2 (1.0f, 0.0f);
			Vector2 textureLowerLeft = new Vector2 (0.0f, 1.0f);
			Vector2 textureLowerRight = new Vector2 (1.0f, 1.0f);

			// Provide a normal for each vertex
			for (int i = 0; i < Vertices.Length; i++) {
				Vertices [i].Normal = Normal;
			}

			// Set the position and texture coordinate for each
			// vertex
			Vertices [0].Position = LowerLeft;
			Vertices [0].TextureCoordinate = textureLowerLeft;
			Vertices [1].Position = UpperLeft;
			Vertices [1].TextureCoordinate = textureUpperLeft;
			Vertices [2].Position = LowerRight;
			Vertices [2].TextureCoordinate = textureLowerRight;
			Vertices [3].Position = UpperRight;
			Vertices [3].TextureCoordinate = textureUpperRight;


			// Set the index buffer for each vertex, using
			// clockwise winding
			Indexes [0] = 0;
			Indexes [1] = 1;
			Indexes [2] = 2;
			Indexes [3] = 2;
			Indexes [4] = 1;
			Indexes [5] = 3;
		}

		public void Draw (Camera camera, BasicEffect effect)
		{
			effect = new BasicEffect (device);
			effect.AmbientLightColor = new Vector3 (0.8f, 0.8f, 0.8f);
			//effect.LightingEnabled = true;
			effect.World = camera.WorldMatrix;
			effect.View = camera.ViewMatrix;
			effect.Projection = camera.ProjectionMatrix;
			effect.TextureEnabled = true;
			effect.VertexColorEnabled = false;
			effect.Texture = texture;

			

			foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
				pass.Apply ();

				
				device.DrawUserIndexedPrimitives
                    <VertexPositionNormalTexture> (
                    PrimitiveType.TriangleList,
                    Vertices, 0, 4,
                    Indexes, 0, 2);

			}
		}
	}
}

