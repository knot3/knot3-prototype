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
		private GraphicsDeviceManager graphics;
		private BasicEffect basicEffect;

		Matrix WorldMatrix { get; set; }

		Matrix ViewMatrix { get; set; }

		Matrix ProjectionMatrix { get; set; }

		private Vector3 camPosition;
		private Vector3 camTarget;
		private Vector3 camUpVector;
		private float viewAngle;
		private float aspectRatio;
		private float nearPlane;
		private float farPlane;
		private float angleX = 0f;//MathHelper.Pi / 4;
		private float angleY = -0.4f;
		private float angleZ = -0.4f;//MathHelper.Pi/2;
		private bool rotateX = false;
		private bool rotateY = false;
		private bool rotateZ = false;

		public void zoom (int i)
		{
			camPosition += new Vector3 (i, 0, 0);
		}

		public float AngleX { get { return angleX; } }

		public float AngleY { get { return angleY; } }

		public float AngleZ { get { return angleZ; } }
 
		public Camera (GraphicsDeviceManager graphics, BasicEffect basicEffect)
		{
			this.graphics = graphics;
			this.basicEffect = basicEffect;
		}
 
		private void SetUpCamera ()
		{
			camPosition = new Vector3 (700, 0, 0);
			camTarget = new Vector3 (0, 0, 1);
			camUpVector = new Vector3 (0, 1, 0);
			ViewMatrix = Matrix.CreateLookAt (camPosition, camTarget, camUpVector);
 
			viewAngle = MathHelper.PiOver4;
			aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
			nearPlane = 0.5f;
			farPlane = 5000.0f;
			ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView (viewAngle, aspectRatio, nearPlane, farPlane);
		}

		public void LoadContent ()
		{
			SetUpCamera ();
		}

		public void Update (GameTime gameTime)
		{
			KeyboardState keyboardState = Keyboard.GetState ();

			float wasdAngle = 0.01f;
			if (keyboardState.IsKeyDown (Keys.W))
				angleZ += wasdAngle;
			if (keyboardState.IsKeyDown (Keys.S))
				angleZ -= wasdAngle;
			if (keyboardState.IsKeyDown (Keys.A))
				angleY -= wasdAngle;
			if (keyboardState.IsKeyDown (Keys.D))
				angleY += wasdAngle;
			if (keyboardState.IsKeyDown (Keys.Q))
				angleX += wasdAngle;
			if (keyboardState.IsKeyDown (Keys.E))
				angleX -= wasdAngle;

			Vector3 targetDiffLR = new Vector3 (0, 0, 10);
			Vector3 targetDiffUD = new Vector3 (0, 10, 0);
			if (keyboardState.IsKeyDown (Keys.Left))
				camTarget += targetDiffLR;
			if (keyboardState.IsKeyDown (Keys.Right))
				camTarget -= targetDiffLR;
			if (keyboardState.IsKeyDown (Keys.Up))
				camTarget += targetDiffUD;
			if (keyboardState.IsKeyDown (Keys.Down))
				camTarget -= targetDiffUD;

			if (rotateY)
				angleY += 0.005f;
			if (rotateZ)
				angleZ += 0.005f;
		}

		public void Draw (GameTime gameTime)
		{ 
			// setting up rotation

			/*
			Matrix rotationMatrixX = Matrix.CreateRotationX (angleX);
			Matrix rotationMatrixY = Matrix.CreateRotationY (angleY);
			Matrix rotationMatrixZ = Matrix.CreateRotationZ (angleZ);
			ViewMatrix = Matrix.CreateTranslation (angleX, angleY, angleZ)
				* rotationMatrixX * rotationMatrixY * rotationMatrixZ *
				Matrix.CreateLookAt (camPosition, camTarget, camUpVector);
			*/

			ViewMatrix = Matrix.CreateLookAt (camPosition, camTarget, camUpVector);
			WorldMatrix = Matrix.CreateFromYawPitchRoll (angleY, angleX, angleZ);

			basicEffect.World = WorldMatrix;
			basicEffect.View = ViewMatrix;
			basicEffect.Projection = ProjectionMatrix;
		}

		public Ray GetMouseRay (Vector2 mousePosition)
		{
			Viewport viewport = graphics.GraphicsDevice.Viewport;

			Vector3 nearPoint = new Vector3 (mousePosition, 0);
			Vector3 farPoint = new Vector3 (mousePosition, 1);
 
			nearPoint = viewport.Unproject (nearPoint, ProjectionMatrix, ViewMatrix, Matrix.Identity);
			farPoint = viewport.Unproject (farPoint, ProjectionMatrix, ViewMatrix, Matrix.Identity);
 
			Vector3 direction = farPoint - nearPoint;
			direction.Normalize ();
 
			return new Ray (nearPoint, direction);
		}
	}
}

