using System;

using Microsoft.Xna.Framework;

namespace Knot3.KnotData
{
	public enum Direction
	{
		Zero = 0,
		Left,
		Right,
		Up,
		Down,
		Forward,
		Backward
	}
	;

	public static class DirectionExtensions {

		public static Direction ToDirection (this Vector3 v)
		{
			if (v == Vector3.Up)
				return Direction.Up;
			else if (v == Vector3.Down)
				return Direction.Down;
			else if (v == Vector3.Left)
				return Direction.Left;
			else if (v == Vector3.Right)
				return Direction.Right;
			else if (v == Vector3.Forward)
				return Direction.Forward;
			else if (v == Vector3.Backward)
				return Direction.Backward;
			else
				return Direction.Zero;
		}

		public static Vector3 ToVector3 (this Direction d)
		{
			if (d == Direction.Up)
				return Vector3.Up;
			else if (d == Direction.Down)
				return Vector3.Down;
			else if (d == Direction.Left)
				return Vector3.Left;
			else if (d == Direction.Right)
				return Vector3.Right;
			else if (d == Direction.Forward)
				return Vector3.Forward;
			else if (d == Direction.Backward)
				return Vector3.Backward;
			else 
				return Vector3.Zero;
		}

		public static Direction ReverseDirection (this Direction dir)
		{
			return (-dir.ToVector3 ()).ToDirection ();
		}
	}

}

