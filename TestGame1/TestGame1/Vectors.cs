using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TestGame1
{
	public static class Vectors
	{
		public static Vector3 MoveLinear (this Vector3 vectorToMove, Vector2 mouse, Vector3 up, Vector3 forward)
		{
			Vector3 side = Vector3.Cross (up, forward);
			Vector3 movedVector = vectorToMove - side * mouse.X - up * mouse.Y;
			return movedVector;
		}
		
		public static Vector3 RotateX (this Vector3 vectorToRotate, float angleRadians )
		{
			return Vector3.Transform (vectorToRotate, Matrix.CreateRotationX (angleRadians));
		}
		
		public static Vector3 RotateY (this Vector3 vectorToRotate, float angleRadians )
		{
			return Vector3.Transform (vectorToRotate, Matrix.CreateRotationY (angleRadians));
		}
		
		public static Vector3 RotateZ (this Vector3 vectorToRotate, float angleRadians )
		{
			return Vector3.Transform (vectorToRotate, Matrix.CreateRotationZ (angleRadians));
		}
	}
}

