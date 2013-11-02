using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
	}
}

