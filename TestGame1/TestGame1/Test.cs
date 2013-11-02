using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TestGame1
{
	public class Test
	{
		public Test ()
		{
		}

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

	}
}

