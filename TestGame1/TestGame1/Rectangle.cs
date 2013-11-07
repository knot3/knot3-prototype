using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace TestGame1
{
	public class TexturedRectangle : GameObject
	{
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
		private float Width;
		private float Height;

		protected override Vector3 Position {
			get {
				return Origin;
			}
			set {
				Origin = value;
				// Calculate the quad corners
				Normal = Vector3.Cross (Left, Up);
				Vector3 uppercenter = (Up * Height / 2) + value;
				UpperLeft = uppercenter + (Left * Width / 2);
				UpperRight = uppercenter - (Left * Width / 2);
				LowerLeft = UpperLeft - (Up * Height);
				LowerRight = UpperRight - (Up * Height);
				FillVertices ();
			}
		}

		public TexturedRectangle (Game game, string texturename, Vector3 origin, Vector3 left, float width, Vector3 up, float height)
			: base(game)
		{
			Left = left;
			Width = width;
			Up = up;
			Height = height;
			Position = origin;

			texture = LoadTexture (texturename);
			if (texture != null) {
				FillVertices ();
			}
		}
        
		private void FillVertices ()
		{
			// Fill in texture coordinates to display full texture
			// on quad
			Vector2 textureUpperLeft = new Vector2 (0.0f, 0.0f);
			Vector2 textureUpperRight = new Vector2 (1.0f, 0.0f);
			Vector2 textureLowerLeft = new Vector2 (0.0f, 1.0f);
			Vector2 textureLowerRight = new Vector2 (1.0f, 1.0f);

			Vertices = new VertexPositionNormalTexture[4];
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
			Indexes = new short[12];
			Indexes [0] = 0;
			Indexes [1] = 1;
			Indexes [2] = 2;
			Indexes [3] = 2;
			Indexes [4] = 1;
			Indexes [5] = 3;
			
			Indexes [6] = 2;
			Indexes [7] = 1;
			Indexes [8] = 0;
			Indexes [9] = 3;
			Indexes [10] = 1;
			Indexes [11] = 2;
		}

		public override void DrawObject (GameTime gameTime)
		{
			basicEffect.AmbientLightColor = new Vector3 (0.8f, 0.8f, 0.8f);
			//effect.LightingEnabled = true;
			basicEffect.TextureEnabled = true;
			basicEffect.VertexColorEnabled = false;
			basicEffect.Texture = texture;

			if (game.Input.KeyboardState.IsKeyDown (Keys.L)) {
				basicEffect.EnableDefaultLighting ();  // Beleuchtung aktivieren
			}

			foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes) {
				pass.Apply ();

				device.DrawUserIndexedPrimitives<VertexPositionNormalTexture> (
                    PrimitiveType.TriangleList, Vertices, 0, Vertices.Length, Indexes, 0, Indexes.Length / 3
				);
			}
		}

		private Vector3 Length ()
		{
			return Left * Width + Up * Height;
		}
		
		public BoundingBox[] Bounds ()
		{
			//Console.WriteLine ("LowerLeft=" + LowerLeft + ", UpperRight=" + UpperRight + ", BoundingBox=" + LowerLeft.Bounds (UpperRight - LowerLeft));
			//return LowerLeft.Bounds (UpperRight - LowerLeft + new Vector3 (1, 1, 1));
			return new BoundingBox[]{
				LowerLeft.Bounds (UpperRight - LowerLeft), LowerRight.Bounds (UpperLeft - LowerRight),
				UpperRight.Bounds (LowerLeft - UpperRight), UpperLeft.Bounds (LowerRight - UpperLeft)
			};
		}

		public override Nullable<float> Intersects (Ray ray)
		{
			foreach (BoundingBox bounds in Bounds()) {
				Nullable<float> distance = ray.Intersects (bounds);
				if (distance != null)
					return distance;
			}
			return null;
		}

		public override Vector3 Center ()
		{
			return LowerLeft + (UpperRight - LowerLeft) / 2;
		}
	}
}

