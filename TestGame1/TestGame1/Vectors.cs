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
	public static class VectorExtensions
	{
		public static Vector3 ArcBallMove (this Vector3 vectorToMove, Vector2 mouse, Vector3 up, Vector3 forward)
		{
			Vector3 side = Vector3.Cross (up, forward);
			Vector3 movedVector = vectorToMove.RotateY (
						MathHelper.Pi / 300f * mouse.X
			);
			movedVector = movedVector.RotateAroundVector (
                       	-side,
						MathHelper.Pi / 200f * mouse.Y
			);
			return movedVector;
		}
		
		public static Vector3 MoveLinear (this Vector3 vectorToMove, Vector3 mouse, Vector3 up, Vector3 forward)
		{
			Vector3 side = Vector3.Cross (up, forward);
			Vector3 movedVector = vectorToMove - side * mouse.X - up * mouse.Y - forward * mouse.Z;
			return movedVector;
		}
		
		public static Vector3 MoveLinear (this Vector3 vectorToMove, Vector2 mouse, Vector3 up, Vector3 forward)
		{
			return vectorToMove.MoveLinear (new Vector3 (mouse.X, mouse.Y, 0), up, forward);
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

		public static Vector3 Clamp (this Vector3 v, Vector3 lower, Vector3 higher)
		{
			return new Vector3 (
				MathHelper.Clamp (v.X, lower.X, higher.X),
				MathHelper.Clamp (v.Y, lower.Y, higher.Y),
				MathHelper.Clamp (v.Z, lower.Z, higher.Z)
			);
		}

		public static Vector3 Clamp (this Vector3 v, int minLength, int maxLength)
		{
			if (v.Length () < minLength) {
				return v * minLength / v.Length ();
			} else if (v.Length () > maxLength) {
				return v * maxLength / v.Length ();
			} else {
				return v;
			}
		}

		public static Vector2 PrimaryDirection (this Vector2 v)
		{
			if (v.X.Abs () > v.Y.Abs ())
				return Vector2.Normalize (new Vector2 (v.X, 0));
			else if (v.Y.Abs () > v.X.Abs ())
				return Vector2.Normalize (new Vector2 (0, v.Y));
			else
				return Vector2.Zero;
		}

		public static Vector3 PrimaryDirection (this Vector3 v)
		{
			if (v.X.Abs () > v.Y.Abs () && v.X.Abs () > v.Z.Abs ())
				return Vector3.Normalize (new Vector3 (v.X, 0, 0));
			else if (v.Y.Abs () > v.X.Abs () && v.Y.Abs () > v.Z.Abs ())
				return Vector3.Normalize (new Vector3 (0, v.Y, 0));
			else if (v.Z.Abs () > v.Y.Abs () && v.Z.Abs () > v.X.Abs ())
				return Vector3.Normalize (new Vector3 (0, 0, v.Z));
			else
				return Vector3.Zero;
		}

		public static Vector3 PrimaryDirectionExcept (this Vector3 v, Vector3 wrongDirection)
		{
			Vector3 copy = v;
			if (wrongDirection.X != 0)
				copy.X = 0;
			else if (wrongDirection.Y != 0)
				copy.Y = 0;
			else if (wrongDirection.Z != 0)
				copy.Z = 0;
			return copy.PrimaryDirection ();
		}

		public static float Abs (this float v)
		{
			return Math.Abs (v);
		}

		public static float Clamp (this float v, int min, int max)
		{
			return MathHelper.Clamp (v, min, max);
		}

		public static BoundingSphere[] Bounds (this Model model)
		{
			BoundingSphere[] bounds = new BoundingSphere[model.Meshes.Count];
			int i = 0;
			foreach (ModelMesh mesh in model.Meshes) {
				bounds [i++] = mesh.BoundingSphere;
			}
			return bounds;
		}

		public static BoundingBox Bounds (this Vector3 a, Vector3 diff)
		{
			return new BoundingBox (a, a + diff);
		}

		public static BoundingSphere Scale (this BoundingSphere sphere, float scale)
		{
			return new BoundingSphere (sphere.Center, sphere.Radius * scale);
		}

		public static BoundingSphere Translate (this BoundingSphere sphere, Vector3 position)
		{
			return new BoundingSphere (Vector3.Transform (sphere.Center, Matrix.CreateTranslation (position)), sphere.Radius);
		}

		public static Vector2 ToVector2 (this MouseState state)
		{
			return new Vector2 (state.X, state.Y);
		}

		public static Vector2 Center (this Viewport viewport)
		{
			return new Vector2 (viewport.Width, viewport.Height) / 2;
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

