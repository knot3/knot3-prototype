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
	public class Camera : GameClass
	{

		public Matrix WorldMatrix { get; private set; }

		public Matrix ViewMatrix { get; private set; }

		public Matrix ProjectionMatrix { get; private set; }

		public Vector3 DefaultPosition { get; private set; }

		public Vector3 Position { get; set; }

		public Vector3 Target { get; set; }

		public Vector3 ArcballTarget { get; set; }

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

		public Camera (Game game)
			: base(game)
		{
		}
 
		private void SetUpCamera ()
		{
			DefaultPosition = new Vector3 (400, 400, 1000);
			Position = DefaultPosition;
			Target = new Vector3 (0, 0, 0);
			ArcballTarget = new Vector3 (0, 0, 0);
			UpVector = Vector3.Up;
			ViewMatrix = Matrix.CreateLookAt (Position, Target, UpVector);
 
			FoV = MathHelper.PiOver4;
			aspectRatio = device.Viewport.AspectRatio;
			nearPlane = 0.5f;
			farPlane = 10000.0f;

			game.Input.ResetMousePosition ();
		}

		public void LoadContent ()
		{
			SetUpCamera ();
		}

		public void Update (GameTime gameTime)
		{
			UpdateRotation (gameTime);
		}

		public void UpdateRotation (GameTime gameTime)
		{
			// auto rotation
			RotationAngle += AutoRotation;
		}

		public void Draw (GameTime gameTime)
		{ 

			//if (Mouse.GetState ().RightButton == ButtonState.Pressed) {
			//	// arcball
			//	WorldMatrix = Matrix.CreateFromYawPitchRoll (RotationAngle.Y, RotationAngle.X, RotationAngle.Z);
			//	ViewMatrix = arcball.ViewMatrix;
			//	ProjectionMatrix = arcball.ProjectionMatrix;
			//}



			// setting up rotation
			ViewMatrix = Matrix.CreateLookAt (Position, Target, UpVector);
			WorldMatrix = Matrix.CreateFromYawPitchRoll (RotationAngle.Y, RotationAngle.X, RotationAngle.Z);
			ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView (MathHelper.ToRadians (FoV), aspectRatio, nearPlane, farPlane);
			
			game.basicEffect.World = WorldMatrix;
			game.basicEffect.View = ViewMatrix;
			game.basicEffect.Projection = ProjectionMatrix;
		}

		public float TargetDistance {
			get {
				Vector3 toTarget = Target - Position;
				return toTarget.Length ();
			}
			set {
				Vector3 toTarget = Target - Position;
				toTarget.Normalize ();
				Position -= toTarget * (TargetDistance - value);

				if (TargetDistance < 100) {
					Position -= toTarget * (100 - TargetDistance);
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
	}
}

