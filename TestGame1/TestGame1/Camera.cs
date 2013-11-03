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
	public class Camera
	{
		private Game game;
		private GraphicsDeviceManager graphics;
		private BasicEffect basicEffect;

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

		// arcball
		public ArcBallCamera arcball;

		public Camera (GraphicsDeviceManager graphics, BasicEffect basicEffect, Game game)
		{
			this.graphics = graphics;
			this.basicEffect = basicEffect;
			this.game = game;
		}
 
		private void SetUpCamera ()
        {
            DefaultPosition = new Vector3(400, 400, 1000);
            Position = DefaultPosition;
			Target = new Vector3 (0, 0, 0);
			UpVector = Vector3.Up;
			ViewMatrix = Matrix.CreateLookAt (Position, Target, UpVector);
 
			FoV = MathHelper.PiOver4;
			aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
			nearPlane = 0.5f;
			farPlane = 10000.0f;

			// arcball
			arcball = new ArcBallCamera (aspectRatio, FoV, Target, UpVector, nearPlane, farPlane);

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

			if (Mouse.GetState ().RightButton == ButtonState.Pressed) {
				// arcball
				WorldMatrix = Matrix.CreateFromYawPitchRoll (RotationAngle.Y, RotationAngle.X, RotationAngle.Z);
				ViewMatrix = arcball.ViewMatrix;
				ProjectionMatrix = arcball.ProjectionMatrix;

			} else {
				// setting up rotation
				ViewMatrix = Matrix.CreateLookAt (Position, Target, UpVector);
				WorldMatrix = Matrix.CreateFromYawPitchRoll (RotationAngle.Y, RotationAngle.X, RotationAngle.Z);
				ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView (MathHelper.ToRadians (FoV), aspectRatio, nearPlane, farPlane);
			}

			basicEffect.World = WorldMatrix;
			basicEffect.View = ViewMatrix;
			basicEffect.Projection = ProjectionMatrix;
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
	}
}

