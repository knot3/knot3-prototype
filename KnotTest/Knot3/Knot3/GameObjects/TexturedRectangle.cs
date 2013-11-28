using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

using Knot3.Core;
using Knot3.Utilities;

namespace Knot3.GameObjects
{
	public class TexturedRectangleInfo : GameObjectInfo
	{
		public string Texturename;
		public Vector3 Up;
		public Vector3 Left;
		public float Width;
		public float Height;

		public TexturedRectangleInfo (string texturename, Vector3 origin, Vector3 left, float width, Vector3 up, float height)
		{
			Texturename = texturename;
			Left = left;
			Width = width;
			Up = up;
			Height = height;
			Position = origin;
		}

		public override bool Equals (GameObjectInfo other)
		{
			if (other == null) 
				return false;

			if (other is GameModelInfo) {
				if (this.Texturename == (other as GameModelInfo).Modelname && base.Equals (other))
					return true;
				else
					return false;
			} else {
				return base.Equals (other);
			}
		}
	}
	
	/// <summary>
	/// Eine frei in der Spielwelt liegende Textur, die auf ein Rechteck gezeichnet wird.
	/// </summary>
	public class TexturedRectangle : GameStateClass, IGameObject
	{
		#region Attributes and Properties

		public dynamic Info { get; private set; }

		private Vector3 UpperLeft;
		private Vector3 LowerLeft;
		private Vector3 UpperRight;
		private Vector3 LowerRight;
		private Vector3 Normal;
		private VertexPositionNormalTexture[] Vertices;
		private short[] Indexes;
		private BasicEffect basicEffect;
		private Texture2D texture;

		#endregion

		#region Constructors

		public TexturedRectangle (GameState state, TexturedRectangleInfo info)
			: base(state)
		{
			Info = info;
			SetPosition (Info.Position);

			basicEffect = new BasicEffect (device);
			texture = Textures.LoadTexture (content, info.Texturename);
			if (texture != null) {
				FillVertices ();
			}
		}

		#endregion

		#region Update
		
		public void Update (GameTime gameTime)
		{
		}

		#endregion

		#region Draw

		public void Draw (GameTime gameTime)
		{
			if (Info.IsVisible) {
				basicEffect.World = camera.WorldMatrix;
				basicEffect.View = camera.ViewMatrix;
				basicEffect.Projection = camera.ProjectionMatrix;

				basicEffect.AmbientLightColor = new Vector3 (0.8f, 0.8f, 0.8f);
				//effect.LightingEnabled = true;
				basicEffect.TextureEnabled = true;
				basicEffect.VertexColorEnabled = false;
				basicEffect.Texture = texture;

				if (Keys.L.IsHeldDown ()) {
					basicEffect.EnableDefaultLighting ();  // Beleuchtung aktivieren
				}

				foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes) {
					pass.Apply ();

					device.DrawUserIndexedPrimitives<VertexPositionNormalTexture> (
                    PrimitiveType.TriangleList, Vertices, 0, Vertices.Length, Indexes, 0, Indexes.Length / 3
					);
				}
			}
		}

		#endregion

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

		protected void SetPosition (Vector3 position)
		{
			Info.Position = position;
			// Calculate the quad corners
			Normal = Vector3.Cross (Info.Left, Info.Up);
			Vector3 uppercenter = (Info.Up * Info.Height / 2) + position;
			UpperLeft = uppercenter + (Info.Left * Info.Width / 2);
			UpperRight = uppercenter - (Info.Left * Info.Width / 2);
			LowerLeft = UpperLeft - (Info.Up * Info.Height);
			LowerRight = UpperRight - (Info.Up * Info.Height);
			FillVertices ();
		}

		private Vector3 Length ()
		{
			return Info.Left * Info.Width + Info.Up * Info.Height;
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

		public GameObjectDistance Intersects (Ray ray)
		{
			foreach (BoundingBox bounds in Bounds()) {
				Nullable<float> distance = ray.Intersects (bounds);
				if (distance != null) {
					GameObjectDistance intersection = new GameObjectDistance () {
						Object=this, Distance=distance.Value
					};
					return intersection;
				}
			}
			return null;
		}

		public Vector3 Center ()
		{
			return LowerLeft + (UpperRight - LowerLeft) / 2;
		}

		#region Selection

		public virtual void OnSelected (GameTime gameTime)
		{
		}

		public virtual void OnUnselected (GameTime gameTime)
		{
		}

		#endregion
	}
}

