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

		public Vector3 camPosition;
		public Vector3 camTarget;
		private Vector3 camUpVector;
		private float foV;

		public float FoV {
			get { return foV;}
			set { foV = value > 100 ? 100 : value < 40 ? 40 : value;}
		}

		private float aspectRatio;
		private float nearPlane;
		private float farPlane;
		private float angleX = 0f;//MathHelper.Pi / 4;
		private float angleY = -0.4f;
		private float angleZ = -0.4f;//MathHelper.Pi/2;
		private bool rotateX = false;
		private bool rotateY = false;
		private bool rotateZ = false;
		public int wasdSpeed = 10;
		private MouseState previousMouseState;
		public bool FullscreenToggled;

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
			camPosition = new Vector3 (0, 0, -500);
			camTarget = new Vector3 (0, 0, 0);
			camUpVector = Vector3.Up;
			ViewMatrix = Matrix.CreateLookAt (camPosition, camTarget, camUpVector);
 
			FoV = MathHelper.PiOver4;
			aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
			nearPlane = 0.5f;
			farPlane = 5000.0f;

			FullscreenToggled = false;
			ResetMousePosition ();
		}

		public void ResetMousePosition ()
		{
			Mouse.SetPosition (graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);
			previousMouseState = Mouse.GetState ();
		}

		public void LoadContent ()
		{
			SetUpCamera ();
		}

        public float TargetDistance
        {
            get
            {
                Vector3 toTarget = camTarget - camPosition;
                return toTarget.Length();
            }
            set
            {
                Vector3 toTarget = camTarget - camPosition;
                toTarget.Normalize();
                camPosition -= toTarget * (TargetDistance - value);

                if (TargetDistance < 100)
                {
                    camPosition -= toTarget * (100 - TargetDistance);
                }
            }
        }

        public Vector3 TargetVector
        {
            get
            {
                Vector3 toTarget = camTarget - camPosition;
                toTarget.Normalize();
                return toTarget;
            }
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

			//Vector3 targetDiffLR = new Vector3 (0, 0, 10);
			//Vector3 targetDiffUD = new Vector3 (10, 0, 0);
			if (keyboardState.IsKeyDown (Keys.Left)) {
				camTarget -= wasdSpeed * Vector3.Left;
				camPosition -= wasdSpeed * Vector3.Left;
			}
			if (keyboardState.IsKeyDown (Keys.Right)) {
				camTarget -= wasdSpeed * Vector3.Right;
				camPosition -= wasdSpeed * Vector3.Right;
			}
			if (keyboardState.IsKeyDown (Keys.Up)) {
				camTarget -= wasdSpeed * Vector3.Forward;
                camPosition -= wasdSpeed * Vector3.Forward;
			}
			if (keyboardState.IsKeyDown (Keys.Down)) {
				camTarget -= wasdSpeed * Vector3.Backward;
                camPosition -= wasdSpeed * Vector3.Backward;
			}

			if (keyboardState.IsKeyDown (Keys.OemPlus) && wasdSpeed < 20) {
				wasdSpeed += 1;
			}
			if (keyboardState.IsKeyDown (Keys.OemMinus) && wasdSpeed > 1) {
				wasdSpeed -= 1;
			}

			if (rotateY)
				angleY += 0.005f;
			if (rotateZ)
				angleZ += 0.005f;

			MouseState currentMouseState = Mouse.GetState ();

			if (currentMouseState != previousMouseState) {
				float xDifference = currentMouseState.X - previousMouseState.X;
				float yDifference = currentMouseState.Y - previousMouseState.Y;
				if (FullscreenToggled)
					FullscreenToggled = false;
                else if (currentMouseState.RightButton == ButtonState.Pressed)
                {
                    Console.WriteLine("camPosition=(" + camPosition.X + "," + camPosition.Y + "," + camPosition.Z
                        + ") -= (" + xDifference + "," + yDifference+",0)");
                    float distance = TargetDistance;
                    float zp = (float)Math.Pow(camPosition.X + xDifference - camTarget.X, 2)
                        + (float)Math.Pow(camPosition.Y + yDifference - camTarget.Y, 2) - distance;
                    if (zp > 0)
                        zp = (float)Math.Sqrt(zp) / camTarget.Z;
                    if (zp < 0)
                        zp = -(float)Math.Sqrt(-zp) / camTarget.Z;
                    camPosition -= new Vector3(xDifference, yDifference, zp);
                        //(distance * distance - (camPosition.X + xDifference) * camTarget.X - 
                    //(camPosition.Y + yDifference) * camTarget.Y) /(1+ camTarget.Z));

                    //while (TargetDistance > distance)
                    //    camPosition -= new Vector3(0, 0, TargetVector.Z);
                }
                else
                    camTarget -= new Vector3(xDifference, yDifference, 0);
				ResetMousePosition ();
			}
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
			ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView (MathHelper.ToRadians (FoV), aspectRatio, nearPlane, farPlane);

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

		public Vector3 Degrees {
			get {
				return new Vector3 (
					(int)MathHelper.ToDegrees (AngleX) % 360,
					(int)MathHelper.ToDegrees (AngleY) % 360,
					(int)MathHelper.ToDegrees (AngleZ) % 360
				);
			}
		}
	}
}

