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

using Knot3.Utilities;

namespace Knot3
{
	public class Camera : GameClass
	{

		public Matrix WorldMatrix { get; private set; }

		public Matrix ViewMatrix { get; private set; }

		public Matrix ProjectionMatrix { get; private set; }

		public Vector3 DefaultPosition { get; private set; }

		public Vector3 Position { get; set; }

		public Vector3 Target { get; set; }

		public Vector3 UpVector { get; private set; }

		private float foV;

		public float FoV {
			get { return foV; }
			set { foV = MathHelper.Clamp (value, 40, 100); }
		}

		public Angles3 RotationAngle = Angles3.Zero;
		private Angles3 AutoRotation = Angles3.Zero;
		private float aspectRatio;
		private float nearPlane;
		private float farPlane;

		public Camera (GameState state)
			: base(state)
		{
		}
 
		private void SetUpCamera ()
		{
			DefaultPosition = new Vector3 (400, 400, 700);
			Position = DefaultPosition;
			Target = new Vector3 (0, 0, 0);
			UpVector = Vector3.Up;
			ViewMatrix = Matrix.CreateLookAt (Position, Target, UpVector);
 
			FoV = MathHelper.PiOver4;
			aspectRatio = device.Viewport.AspectRatio;
			nearPlane = 0.5f;
			farPlane = 10000.0f;
		}

		public void LoadContent ()
		{
			SetUpCamera ();
		}

		public void Update (GameTime gameTime)
		{
			UpdateRotation (gameTime);
			UpdateMatrices (gameTime);
		}

		private void UpdateRotation (GameTime gameTime)
		{
			// auto rotation
			RotationAngle += AutoRotation;
		}

		private void UpdateMatrices (GameTime gameTime)
		{ 
			// setting up rotation
			ViewMatrix = Matrix.CreateLookAt (Position, Target, UpVector);
			WorldMatrix = Matrix.CreateFromYawPitchRoll (RotationAngle.Y, RotationAngle.X, RotationAngle.Z);
			ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView (MathHelper.ToRadians (FoV), aspectRatio, nearPlane, farPlane);
		}

		public float TargetDistance {
			get {
				Vector3 toTarget = Target - Position;
				return toTarget.Length ();
			}
			set {
				Vector3 toPosition = Position - Target;
				if (Math.Abs (value) > 300) {
					Position = Target + toPosition * value / toPosition.Length ();
				} else {
					Position = Target + toPosition * 300 / toPosition.Length ();
				}
			}
		}

		public Vector3 TargetVector {
			get {
				Vector3 toTarget = Target - Position;
				toTarget.Normalize ();
				return toTarget;
			}
		}

		public Ray GetMouseRay (Vector2 mouse)
		{
			Viewport viewport = device.Viewport;

			Vector3 nearPoint = new Vector3 (mouse, 0);
			Vector3 farPoint = new Vector3 (mouse, 1);

			nearPoint = viewport.Unproject (nearPoint, ProjectionMatrix, ViewMatrix, Matrix.Identity);
			farPoint = viewport.Unproject (farPoint, ProjectionMatrix, ViewMatrix, Matrix.Identity);

			Vector3 direction = farPoint - nearPoint;
			direction.Normalize ();

			return new Ray (nearPoint, direction);
		}

		public Vector3 ArcballTarget {
			get {
				if (world.SelectedObject != null)
					return world.SelectedObject.Center ();
				else
					return Vector3.Zero;
			}
		}
	}
}

