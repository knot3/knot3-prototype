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

namespace TestGame1
{
	public static class Test
	{

		//In a 2D grid, returns the angle to a specified point from the +X axis 
		private static float ArcTanAngle (float X, float Y)
		{ 
			if (X == 0) { 
				if (Y == 1) 
					return (float)Microsoft.Xna.Framework.MathHelper.PiOver2;
				else 
					return (float)-Microsoft.Xna.Framework.MathHelper.PiOver2; 
			} else if (X > 0) 
				return (float)Math.Atan (Y / X);
			else if (X < 0) { 
				if (Y > 0) 
					return (float)Math.Atan (Y / X) + Microsoft.Xna.Framework.MathHelper.Pi;
				else 
					return (float)Math.Atan (Y / X) - Microsoft.Xna.Framework.MathHelper.Pi; 
			} else 
				return 0; 
		}
		
		public static Matrix GetRotationMatrix (Vector3 source, Vector3 target)
		{
			float dot = Vector3.Dot (source, target);
			if (!float.IsNaN (dot)) {
				float angle = (float)Math.Acos (dot);
				if (!float.IsNaN (angle)) {
					Vector3 cross = Vector3.Cross (source, target);
					cross.Normalize ();
					Matrix rotation = Matrix.CreateFromAxisAngle (cross, angle);
					return rotation;
				}
			}
			return Matrix.Identity;
		}

		public static Vector3 NaNToNull (Vector3 v)
		{
			if (float.IsNaN (v.X))
				v.X = 0;
			if (float.IsNaN (v.Y))
				v.Y = 0;
			if (float.IsNaN (v.Z))
				v.Z = 0;
			return v;
		}

		public static Ray GetMouseRay (Vector2 mousePosition, GraphicsDeviceManager graphics, Matrix ProjectionMatrix, Matrix ViewMatrix)
		{
			Viewport viewport = graphics.GraphicsDevice.Viewport;

			Vector3 nearPoint = new Vector3 (mousePosition, 0);
			Vector3 farPoint = new Vector3 (mousePosition, 1);

			nearPoint = viewport.Unproject (nearPoint, ProjectionMatrix, ViewMatrix, Matrix.Identity);
			farPoint = viewport.Unproject (farPoint, ProjectionMatrix, ViewMatrix, Matrix.Identity);

			Vector3 direction = farPoint - nearPoint;
			direction.Normalize ();

			return new Ray (nearPoint, direction);
		}

		private static void DrawCircle (GraphicsDeviceManager graphics)
		{
			var vertices = new VertexPositionColor[100];
			for (int i = 0; i < 99; i++) {
				float angle = (float)(i / 100.0 * Math.PI * 2);
				vertices [i].Position = new Vector3 (200 + (float)Math.Cos (angle) * 100, 200 + (float)Math.Sin (angle) * 100, 0);
				vertices [i].Color = Color.Black;
			}
			vertices [99] = vertices [0];
			graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor> (PrimitiveType.LineStrip, vertices, 0, 99);
		}

		public static void Lightning (BasicEffect basicEffect)
		{
			// primitive color
			basicEffect.AmbientLightColor = new Vector3 (0.1f, 0.1f, 0.1f);
			basicEffect.DiffuseColor = new Vector3 (1.0f, 1.0f, 1.0f);
			basicEffect.SpecularColor = new Vector3 (0.25f, 0.25f, 0.25f);
			basicEffect.SpecularPower = 5.0f;
			basicEffect.Alpha = 1.0f;

			basicEffect.LightingEnabled = true;
			if (basicEffect.LightingEnabled) {
				basicEffect.DirectionalLight0.Enabled = true; // enable each light individually
				if (basicEffect.DirectionalLight0.Enabled) {
					// x direction
					basicEffect.DirectionalLight0.DiffuseColor = new Vector3 (1, 0, 0); // range is 0 to 1
					basicEffect.DirectionalLight0.Direction = Vector3.Normalize (new Vector3 (-1, 0, 0));
					// points from the light to the origin of the scene
					basicEffect.DirectionalLight0.SpecularColor = Vector3.One;
				}

				basicEffect.DirectionalLight1.Enabled = true;
				if (basicEffect.DirectionalLight1.Enabled) {
					// y direction
					basicEffect.DirectionalLight1.DiffuseColor = new Vector3 (0, 0.75f, 0);
					basicEffect.DirectionalLight1.Direction = Vector3.Normalize (new Vector3 (0, -1, 0));
					basicEffect.DirectionalLight1.SpecularColor = Vector3.One;
				}

				basicEffect.DirectionalLight2.Enabled = true;
				if (basicEffect.DirectionalLight2.Enabled) {
					// z direction
					basicEffect.DirectionalLight2.DiffuseColor = new Vector3 (0, 0, 0.5f);
					basicEffect.DirectionalLight2.Direction = Vector3.Normalize (new Vector3 (0, 0, -1));
					basicEffect.DirectionalLight2.SpecularColor = Vector3.One;
				}
			}

		}
	}

	public class Floor3D
	{
		//Attributes
		private Vector3 position;
		private Vector3 size;
		private GraphicsDevice device;
		private Game game;
		//...
		private VertexPositionNormalTexture[] vertices;
		private VertexBuffer vertexBuffer;
		private IndexBuffer indexBuffer;
		private Texture2D texture;

		//Constructor
		public Floor3D (GraphicsDevice device, Vector3 position, Vector3 size, Game game)
		{
			this.device = device;
			this.position = position;
			this.size = size;
			this.game = game;
			BuildFloorBuffer ();
		}

		//Build our vertex buffer
		private void BuildFloorBuffer ()
		{
			try {
				texture = game.Content.Load<Texture2D> ("background");
			} catch (ContentLoadException ex) {
				Console.WriteLine (ex.ToString ());
				texture = null;
				return;
			}
			// Vertices erstellen und zuweisen.
			vertices = new VertexPositionNormalTexture[4];

			vertices [0].Position = new Vector3 (position.X, position.Y, position.Z);
			vertices [0].TextureCoordinate = new Vector2 (0.0f, 1.0f);
			vertices [0].Normal = Vector3.Forward;

			vertices [1].Position = new Vector3 (position.X, position.Y, position.Z + size.Z);
			vertices [1].TextureCoordinate = new Vector2 (0.0f, 0.0f);
			vertices [1].Normal = Vector3.Forward;

			vertices [2].Position = new Vector3 (position.X + size.X, position.Y, position.Z);
			vertices [2].TextureCoordinate = new Vector2 (1.0f, 1.0f);
			vertices [2].Normal = Vector3.Forward;

			vertices [3].Position = new Vector3 (position.X + size.X, position.Y, position.Z + size.Z);
			vertices [3].TextureCoordinate = new Vector2 (1.0f, 0.0f);
			vertices [3].Normal = Vector3.Forward;

			vertexBuffer = new VertexBuffer (
		        device,
		        typeof(VertexPositionNormalTexture),
		        vertices.Length,
		        BufferUsage.WriteOnly
			);

			vertexBuffer.SetData (vertices);

			// Indices erstellen und zuweisen.
			var indices = new int[12];

			// 1. Dreieck Vordersiete
			indices [0] = 2;
			indices [1] = 1;
			indices [2] = 0;

			// 2. Dreieck Vordersiete
			indices [3] = 2;
			indices [4] = 3;
			indices [5] = 1;

			// 1. Dreieck Rueckseite
			indices [6] = 0;
			indices [7] = 1;
			indices [8] = 2;

			// 2. Dreieck Rueckseite
			indices [9] = 1;
			indices [10] = 3;
			indices [11] = 2;

			indexBuffer = new IndexBuffer (
		        device,
		        typeof(int),
		        indices.Length,
		        BufferUsage.WriteOnly
			);
			indexBuffer.SetData (indices);
		}

		//Draw method
		public void Draw (Camera camera, BasicEffect effect)
		{
			if (texture != null) {
				effect.World = camera.WorldMatrix;
				effect.View = camera.ViewMatrix;
				effect.Projection = camera.ProjectionMatrix;

			
				if (!Keys.L.IsHeldDown()) {
					effect.EnableDefaultLighting ();  // Beleuchtung aktivieren
				}
    
				//effect.TextureEnabled = true; 
				effect.Texture = texture;
				effect.TextureEnabled = true;
				effect.VertexColorEnabled = false;
				//effect.VertexColorEnabled = true;

				effect.CurrentTechnique.Passes [0].Apply ();

				foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
					pass.Apply ();

					device.SetVertexBuffer (vertexBuffer);
					device.Indices = indexBuffer;
					//effect.DirectionalLight0.Enabled = true;
					//effect.DirectionalLight0.DiffuseColor = Color.Pink.ToVector3();
					//effect.DirectionalLight0.Direction = Vector3.Backward;

					device.DrawIndexedPrimitives (PrimitiveType.TriangleList, 0, 0, vertices.Length, 0, 4);

					pass.Apply ();
				}
			}
		}
	}
}

