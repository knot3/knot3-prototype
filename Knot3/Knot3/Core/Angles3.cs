using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Knot3.Utilities
{
	public struct Angles3 : IEquatable<Angles3> {
		#region Public Fields

		public float X;
		public float Y;
		public float Z;

		#endregion

		#region Constructors

		public Angles3 (float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public Angles3 (Vector3 v)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
		}

		#endregion

		#region Private Fields

		private static Angles3 zero = new Angles3 (0f, 0f, 0f);

		#endregion

		public static Angles3 Zero
		{
			get { return zero; }
		}

		#region Public Methods

		public Angles3 ToDegrees ()
		{
			return new Angles3 (
			           (int)MathHelper.ToDegrees (X) % 360,
			           (int)MathHelper.ToDegrees (Y) % 360,
			           (int)MathHelper.ToDegrees (Z) % 360
			       );
		}

		public Vector3 ToVector ()
		{
			return new Vector3 (X, Y, Z);
		}

		public static Angles3 FromDegrees (float x, float y, float z)
		{
			return new Angles3 (
			           MathHelper.ToRadians (x),
			           MathHelper.ToRadians (y),
			           MathHelper.ToRadians (z)
			       );
		}

		public override bool Equals (object obj)
		{
			return (obj is Angles3) ? this == (Angles3)obj : false;
		}

		public bool Equals (Angles3 other)
		{
			return this == other;
		}

		public override int GetHashCode ()
		{
			return (int)(this.X + this.Y + this.Z);
		}

		#endregion

		#region Operators

		public static bool operator == (Angles3 value1, Angles3 value2)
		{
			return value1.X == value2.X
			       && value1.Y == value2.Y
			       && value1.Z == value2.Z;
		}

		public static bool operator != (Angles3 value1, Angles3 value2)
		{
			return !(value1 == value2);
		}

		public static Angles3 operator + (Angles3 value1, Angles3 value2)
		{
			value1.X += value2.X;
			value1.Y += value2.Y;
			value1.Z += value2.Z;
			return value1;
		}

		public static Angles3 operator - (Angles3 value)
		{
			value = new Angles3 (-value.X, -value.Y, -value.Z);
			return value;
		}

		public static Angles3 operator - (Angles3 value1, Angles3 value2)
		{
			value1.X -= value2.X;
			value1.Y -= value2.Y;
			value1.Z -= value2.Z;
			return value1;
		}

		public static Angles3 operator * (Angles3 value1, Angles3 value2)
		{
			value1.X *= value2.X;
			value1.Y *= value2.Y;
			value1.Z *= value2.Z;
			return value1;
		}

		public static Angles3 operator * (Angles3 value, float scaleFactor)
		{
			value.X *= scaleFactor;
			value.Y *= scaleFactor;
			value.Z *= scaleFactor;
			return value;
		}

		public static Angles3 operator * (float scaleFactor, Angles3 value)
		{
			value.X *= scaleFactor;
			value.Y *= scaleFactor;
			value.Z *= scaleFactor;
			return value;
		}

		public static Angles3 operator / (Angles3 value1, Angles3 value2)
		{
			value1.X /= value2.X;
			value1.Y /= value2.Y;
			value1.Z /= value2.Z;
			return value1;
		}

		public static Angles3 operator / (Angles3 value, float divider)
		{
			float factor = 1 / divider;
			value.X *= factor;
			value.Y *= factor;
			value.Z *= factor;
			return value;
		}

		#endregion
	}
}
