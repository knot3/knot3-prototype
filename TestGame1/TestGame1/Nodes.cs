using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TestGame1
{
	public class Node
	{
		#region Properties

		public int X { get; private set; }

		public int Y { get; private set; }

		public int Z { get; private set; }

		public static int Scale { get; set; }

		#endregion

		#region Constructors

		public Node (int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		#endregion

		#region Operators

		public static Node operator + (Node a, Vector3 b)
		{
			return new Node (a.X + (int)b.X, a.Y + (int)b.Y, a.Z + (int)b.Z);
		}

		public static Vector3 operator - (Node a, Node b)
		{
			return new Vector3 (a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}

		public static bool operator == (Node a, Node b)
		{
			// If both are null, or both are same instance, return true.
			if (System.Object.ReferenceEquals (a, b)) {
				return true;
			}

			// If one is null, but not both, return false.
			if (((object)a == null) || ((object)b == null)) {
				return false;
			}

			// Return true if the fields match:
			return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
		}

		public static bool operator != (Node a, Node b)
		{
			return !(a == b);
		}
		
		public override bool Equals (object obj)
		{
			Node other = obj as Node;
			return this.X == other.X && this.Y == other.Y && this.Z == other.Z;
		}

		public override int GetHashCode ()
		{
			return X * 10000 + Y * 100 + Z;
		}

		public override string ToString ()
		{
			return "(" + X + "," + Y + "," + Z + ")";
		}

		#endregion

		#region Public Methods

		public Vector3 Vector ()
		{
			return new Vector3 (X * Scale, Y * Scale, Z * Scale);
		}

		#endregion
	}
}

