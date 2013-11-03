using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TestGame1
{
	public static class VectorExtensions
	{
		public static Vector3 MoveLinear (this Vector3 vectorToMove, Vector2 mouse, Vector3 up, Vector3 forward)
		{
			Vector3 side = Vector3.Cross (up, forward);
			Vector3 movedVector = vectorToMove - side * mouse.X - up * mouse.Y;
			return movedVector;
		}
		
		public static Vector3 RotateX (this Vector3 vectorToRotate, float angleRadians)
		{
			return Vector3.Transform (vectorToRotate, Matrix.CreateRotationX (angleRadians));
		}
		
		public static Vector3 RotateY (this Vector3 vectorToRotate, float angleRadians)
		{
			return Vector3.Transform (vectorToRotate, Matrix.CreateRotationY (angleRadians));
		}

		public static Vector3 RotateZ (this Vector3 vectorToRotate, float angleRadians)
		{
			return Vector3.Transform (vectorToRotate, Matrix.CreateRotationZ (angleRadians));
		}

		public static Vector3 RotateAroundVector (this Vector3 vectorToRotate, Vector3 axis, float angleRadians)
		{
			return Vector3.Transform (vectorToRotate, Matrix.CreateFromAxisAngle (axis, angleRadians));
		}
	}

	public class Angles3
	{
		private Vector3 v;

		public Angles3 (float X, float Y, float Z)
		{
			v = new Vector3 (X, Y, Z);
		}

		public Angles3 (Vector3 v)
		{
			this.v = v;
		}

		public float X {
			get{ return v.X; }
			set{ v.X = value; }
		}

		public float Y {
			get{ return v.Y; }
			set{ v.Y = value; }
		}

		public float Z {
			get{ return v.Z; }
			set{ v.Z = value; }
		}

		public static Angles3 Zero {
			get { return new Angles3 (Vector3.Zero); }
		}

		public Angles3 Degrees {
			get {
				return new Angles3 (
					(int)MathHelper.ToDegrees (X) % 360,
					(int)MathHelper.ToDegrees (Y) % 360,
					(int)MathHelper.ToDegrees (Z) % 360
				);
			} 
		}

		public Vector3 Vector {
			get {
				return v;
			} 
		}

		public static Angles3 operator + (Angles3 a, Angles3 b)
		{
			return new Angles3 (a.v + b.v);
		}
	}

	public class Size
	{
		public int Width { get; set; }

		public int Height { get; set; }

		public Size (int width, int height)
		{
			Width = width;
			Height = height;
		}
	}
}

