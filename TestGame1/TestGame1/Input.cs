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
	public class Input : GameClass
	{
		private int wasdSpeed = 10;
		private bool FullscreenToggled;
		public WasdMode wasdMode = WasdMode.ARCBALL;
		public static KeyboardState PreviousKeyboardState;
		public static MouseState PreviousMouseState;

		public bool GrabMouseMovement { get; set; }

		public Input (Game game)
			: base(game)
		{
			FullscreenToggled = false;
			GrabMouseMovement = true;
		}

		public void Update (GameTime gameTime)
		{
			UpdateKeys (gameTime);
			UpdateMouse (gameTime);
		}

		private void UpdateKeys (GameTime gameTime)
		{
			KeyboardState keyboardState = Keyboard.GetState ();
			Vector3 keyboardMove = Vector3.Zero;
			Vector2 arcballMove = Vector2.Zero;

			// W,A,S,D,Q,E
			if (wasdMode == WasdMode.ARCBALL) {
				if (keyboardState.IsKeyDown (Keys.A))
					arcballMove += new Vector2 (-1, 0);
				if (keyboardState.IsKeyDown (Keys.D))
					arcballMove += new Vector2 (1, 0);
				if (keyboardState.IsKeyDown (Keys.W))
					arcballMove += new Vector2 (0, -1);
				if (keyboardState.IsKeyDown (Keys.S))
					arcballMove += new Vector2 (0, 1);
				if (keyboardState.IsKeyDown (Keys.LeftShift))
					keyboardMove += new Vector3 (0, -1, 0);
				if (keyboardState.IsKeyDown (Keys.LeftControl))
					keyboardMove += new Vector3 (0, 1, 0);
			} else if (wasdMode == WasdMode.FPS) {
				if (keyboardState.IsKeyDown (Keys.A))
					keyboardMove += new Vector3 (-1, 0, 0);
				if (keyboardState.IsKeyDown (Keys.D))
					keyboardMove += new Vector3 (1, 0, 0);
				if (keyboardState.IsKeyDown (Keys.W))
					keyboardMove += new Vector3 (0, 0, -1);
				if (keyboardState.IsKeyDown (Keys.S))
					keyboardMove += new Vector3 (0, 0, 1);
				if (keyboardState.IsKeyDown (Keys.LeftShift))
					keyboardMove += new Vector3 (0, -1, 0);
				if (keyboardState.IsKeyDown (Keys.LeftControl))
					keyboardMove += new Vector3 (0, 1, 0);
			} else if (wasdMode == WasdMode.ROTATION) {
				float wasdAngle = 0.01f;
				if (keyboardState.IsKeyDown (Keys.W))
					camera.RotationAngle.Z += wasdAngle;
				if (keyboardState.IsKeyDown (Keys.S))
					camera.RotationAngle.Z -= wasdAngle;
				if (keyboardState.IsKeyDown (Keys.A))
					camera.RotationAngle.Y -= wasdAngle;
				if (keyboardState.IsKeyDown (Keys.D))
					camera.RotationAngle.Y += wasdAngle;
				if (keyboardState.IsKeyDown (Keys.Q))
					camera.RotationAngle.X += wasdAngle;
				if (keyboardState.IsKeyDown (Keys.E))
					camera.RotationAngle.X -= wasdAngle;
			}

			// Arrow Keys
			if (keyboardState.IsKeyDown (Keys.Left))
				keyboardMove += new Vector3 (-1, 0, 0);
			if (keyboardState.IsKeyDown (Keys.Right))
				keyboardMove += new Vector3 (1, 0, 0);
			if (keyboardState.IsKeyDown (Keys.Up))
				keyboardMove += new Vector3 (0, -1, 0);
			if (keyboardState.IsKeyDown (Keys.Down))
				keyboardMove += new Vector3 (0, 1, 0);

			// apply keyboard movements
			if (keyboardMove.Length () > 0) {
				keyboardMove *= wasdSpeed;
				// linear move, target and position
				camera.Target = camera.Target.MoveLinear (keyboardMove, camera.UpVector, camera.TargetVector);
				camera.Position = camera.Position.MoveLinear (keyboardMove, camera.UpVector, camera.TargetVector);
			}

			// apply arcball movements
			if (arcballMove.Length () > 0) {
				arcballMove *= 3;
				camera.Position = camera.ArcballTarget + (camera.Position - camera.ArcballTarget).ArcBallMove (
						arcballMove, camera.UpVector, camera.TargetVector
				);
			}

			// Plus/Minus Keys
			if (keyboardState.IsKeyDown (Keys.OemPlus) && wasdSpeed < 20) {
				wasdSpeed += 1;
			}
			if (keyboardState.IsKeyDown (Keys.OemMinus) && wasdSpeed > 1) {
				wasdSpeed -= 1;
			}

			// Enter key
			if (keyboardState.IsKeyDown (Keys.Enter)) {
				// set target to (0,0,0)
				camera.Target = Vector3.Zero;
				camera.ArcballTarget = new Vector3 (0, 0, 0);
				// set position to default
				camera.Position = camera.DefaultPosition;
			}

			// grab mouse movent
			if (Keys.LeftAlt.IsDown ()) {
				GrabMouseMovement = !GrabMouseMovement;
			}

			// fullscreen
			if (Keys.F.IsDown () || Keys.F11.IsDown ()) {
				Console.WriteLine ("Fullscreen Toggle");
				if (graphics.IsFullScreen == false) {
					graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.DisplayMode.Width;
					graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.DisplayMode.Height;
				} else {
					graphics.PreferredBackBufferWidth = Game.defaultSize.Width;
					graphics.PreferredBackBufferHeight = Game.defaultSize.Height;
				}
				graphics.ToggleFullScreen ();
				graphics.ApplyChanges ();
				FullscreenToggled = true;
			}

			// switch WASD mode
			if (Keys.Tab.IsDown ()) {
				switch (wasdMode) {
				case WasdMode.ARCBALL:
					wasdMode = WasdMode.FPS;
					break;
				case WasdMode.FPS:
					wasdMode = WasdMode.ROTATION;
					break;
				case WasdMode.ROTATION:
					wasdMode = WasdMode.ARCBALL;
					break;
				}
			}

			// Test Textures
			if (Keys.T.IsDown ()) {
				World.Enabled = !World.Enabled;
			}

			// allows the game to exit
			if (Keys.Escape.IsDown ()) {
				game.Exit ();
			}
		}

		private void UpdateMouse (GameTime gameTime)
		{
			// fullscreen recently toggled?
			if (FullscreenToggled) {
				FullscreenToggled = false;
				camera.Target += Vector3.Up;

			} else if (MouseState != PreviousMouseState) {
				// mouse movements
				Vector2 mouseMove = new Vector2 (MouseState.X - PreviousMouseState.X, MouseState.Y - PreviousMouseState.Y);

				if (Game.Debug) {
					Console.WriteLine ("mouseMove: " + mouseMove + ", MouseState=(" +
						MouseState.X + "," + MouseState.Y +
						"), PreviousMouseState=(" + PreviousMouseState.X + "," + PreviousMouseState.Y + ")"
					);
				}

				bool moveTarget = false, moveArcball = false;
				// grab mouse movement
				if (GrabMouseMovement) {
					// left mouse button pressed
					if (MouseState.LeftButton == ButtonState.Pressed)
						moveArcball = true;
					// right mouse button pressed
					else if (MouseState.RightButton == ButtonState.Pressed)
						moveArcball = true;
					// no mouse button
					else
						moveTarget = true;
				}
				// don't grab mouse movement
				else {
					// left mouse button pressed
					if (MouseState.LeftButton == ButtonState.Pressed)
						moveTarget = true;
					// right mouse button pressed
					else if (MouseState.RightButton == ButtonState.Pressed)
						moveArcball = true;
				}

				// arcball move
				if (moveArcball) {
					// camera.Target = new Vector3 (0, 0, 0);
					// camera.arcball.Yaw += (mouseMove.X / 1000);
					// camera.arcball.Pitch += (mouseMove.Y / 1000);
					camera.Target = new Vector3(camera.ArcballTarget.X, camera.Target.Y, camera.ArcballTarget.Z);
					camera.Position = camera.ArcballTarget + (camera.Position - camera.ArcballTarget).ArcBallMove (
						mouseMove, camera.UpVector, camera.TargetVector
					);
				}
				// move the target vector
				if (moveTarget) {
					camera.Target = camera.Target.MoveLinear (mouseMove, camera.UpVector, camera.TargetVector);
				}

				// scroll wheel zoom
				if (MouseState.ScrollWheelValue < PreviousMouseState.ScrollWheelValue) {
					camera.TargetDistance -= 10;
					// arcball
					// camera.arcball.Zoom -= 10; 
				} else if (MouseState.ScrollWheelValue > PreviousMouseState.ScrollWheelValue) {
					camera.TargetDistance += 10;
					// arcball
					// camera.arcball.Zoom += 10; 
				}
			}
		}

		public void ResetMousePosition ()
		{
			if (MouseState != PreviousMouseState) {
				if (GrabMouseMovement
					|| (MouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton == ButtonState.Pressed)
					|| (MouseState.RightButton == ButtonState.Pressed && PreviousMouseState.RightButton == ButtonState.Pressed)
				    ) {
					Mouse.SetPosition (graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);
				}
			}
			PreviousMouseState = MouseState;
		}

		public void SaveStates ()
		{
			ResetMousePosition ();
			// Update saved state.
			PreviousKeyboardState = KeyboardState;
		}

		public MouseState MouseState {
			get {
				return Mouse.GetState ();
			}
		}

		public KeyboardState KeyboardState {
			get {
				return Keyboard.GetState ();
			}
		}

		public enum WasdMode
		{
			ARCBALL,
			FPS,
			ROTATION
		}
	}

	public static class InputExtensions
	{
		public static bool IsDown (this Keys key)
		{
			KeyboardState keyboardState = Keyboard.GetState ();
			// Is the key down?
			if (keyboardState.IsKeyDown (key)) {
				// If not down last update, key has just been pressed.
				if (!Input.PreviousKeyboardState.IsKeyDown (key)) {
					return true;
				}
			}
			return false;
		}
	}
}

