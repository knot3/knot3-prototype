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
			GrabMouseMovement = false;
		}

		public void Update (GameTime gameTime)
		{
			UpdateKeys (gameTime);
			UpdateMouse (gameTime);
		}

		private void UpdateKeys (GameTime gameTime)
		{
			Vector3 keyboardMove = Vector3.Zero;
			Vector2 arcballMove = Vector2.Zero;

			// W,A,S,D,Q,E
			if (wasdMode == WasdMode.ARCBALL) {
				if (Keys.A.IsHeldDown())
					arcballMove += new Vector2 (-1, 0);
				if (Keys.D.IsHeldDown())
					arcballMove += new Vector2 (1, 0);
				if (Keys.W.IsHeldDown())
					arcballMove += new Vector2 (0, -1);
				if (Keys.S.IsHeldDown())
					arcballMove += new Vector2 (0, 1);
				if (Keys.LeftShift.IsHeldDown())
					keyboardMove += new Vector3 (0, -1, 0);
				if (Keys.LeftControl.IsHeldDown())
					keyboardMove += new Vector3 (0, 1, 0);
			} else if (wasdMode == WasdMode.FPS) {
				if (Keys.A.IsHeldDown())
					keyboardMove += new Vector3 (-1, 0, 0);
				if (Keys.D.IsHeldDown())
					keyboardMove += new Vector3 (1, 0, 0);
				if (Keys.W.IsHeldDown())
					keyboardMove += new Vector3 (0, 0, -1);
				if (Keys.S.IsHeldDown())
					keyboardMove += new Vector3 (0, 0, 1);
				if (Keys.LeftShift.IsHeldDown())
					keyboardMove += new Vector3 (0, -1, 0);
				if (Keys.LeftControl.IsHeldDown())
					keyboardMove += new Vector3 (0, 1, 0);
			} else if (wasdMode == WasdMode.ROTATION) {
				float wasdAngle = 0.01f;
				if (Keys.W.IsHeldDown())
					camera.RotationAngle.Z += wasdAngle;
				if (Keys.S.IsHeldDown())
					camera.RotationAngle.Z -= wasdAngle;
				if (Keys.A.IsHeldDown())
					camera.RotationAngle.Y -= wasdAngle;
				if (Keys.D.IsHeldDown())
					camera.RotationAngle.Y += wasdAngle;
				if (Keys.Q.IsHeldDown())
					camera.RotationAngle.X += wasdAngle;
				if (Keys.E.IsHeldDown())
					camera.RotationAngle.X -= wasdAngle;
			}

			// Arrow Keys
			if (Keys.Left.IsHeldDown())
				keyboardMove += new Vector3 (-1, 0, 0);
			if (Keys.Right.IsHeldDown())
				keyboardMove += new Vector3 (1, 0, 0);
			if (Keys.Up.IsHeldDown())
				keyboardMove += new Vector3 (0, -1, 0);
			if (Keys.Down.IsHeldDown())
				keyboardMove += new Vector3 (0, 1, 0);

			// apply keyboard movements
			if (keyboardMove.Length () > 0) {
				keyboardMove *= wasdSpeed;
				// linear move, target and position
				camera.Target = camera.Target.MoveLinear (keyboardMove, camera.UpVector, camera.TargetVector);
				camera.Position = camera.Position.MoveLinear (keyboardMove, camera.UpVector, camera.TargetVector);
				CurrentInputAction = InputAction.FPSMove;
			}

			// apply arcball movements
			if (arcballMove.Length () > 0) {
				arcballMove *= 3;
				camera.Target = new Vector3 (camera.ArcballTarget.X, camera.Target.Y, camera.ArcballTarget.Z);
				camera.TargetDistance = camera.TargetDistance.Clamp (200, 10000);
				camera.Position = camera.ArcballTarget + (camera.Position - camera.ArcballTarget).ArcBallMove (
						arcballMove, camera.UpVector, camera.TargetVector
				);
				CurrentInputAction = InputAction.ArcballMove;
			}

			// Plus/Minus Keys
			if (Keys.OemPlus.IsHeldDown() && wasdSpeed < 20) {
				wasdSpeed += 1;
			}
			if (Keys.OemMinus.IsHeldDown() && wasdSpeed > 1) {
				wasdSpeed -= 1;
			}

			// Enter key
			if (Keys.Enter.IsHeldDown()) {
				// set target to (0,0,0)
				camera.Target = Vector3.Zero;
				// set position to default
				camera.Position = camera.DefaultPosition;
				// don't select a game object
				world.SelectedObject = null;
			}

			// grab mouse movent
			if (Keys.LeftAlt.IsDown ()) {
				GrabMouseMovement = !GrabMouseMovement;
				world.SelectedObject = null;
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

				InputAction action;
				// grab mouse movement
				if (GrabMouseMovement) {
					// left mouse button pressed
					if (MouseState.LeftButton == ButtonState.Pressed)
						action = InputAction.ArcballMove;
					// right mouse button pressed
					else if (MouseState.RightButton == ButtonState.Pressed)
						action = InputAction.ArcballMove;
					// no mouse button
					else
						action = InputAction.TargetMove;
				}
				// don't grab mouse movement
				else {
					// left mouse button pressed
					if (MouseState.LeftButton == ButtonState.Pressed) {
						if (world.SelectedObject != null && (world.SelectedObject.IsMovable
						    	|| world.SelectedObject is Pipe))
							action = InputAction.SelectedObjectMove;
						else
							action = InputAction.TargetMove;
						//mouseMove *= -1;
					}
					// right mouse button pressed
					else if (MouseState.RightButton == ButtonState.Pressed)
						action = InputAction.ArcballMove;
					// no mouse button
					else
						action = InputAction.FreeMouse;
				}

				switch (action) {
				// arcball move
				case InputAction.ArcballMove:
					camera.Target = new Vector3 (camera.ArcballTarget.X, camera.Target.Y, camera.ArcballTarget.Z);
					camera.TargetDistance = camera.TargetDistance.Clamp (200, 10000);
					camera.Position = camera.ArcballTarget + (camera.Position - camera.ArcballTarget).ArcBallMove (
						mouseMove, camera.UpVector, camera.TargetVector
					);
					break;
				// move the target vector
				case InputAction.TargetMove:
					camera.Target = camera.Target.MoveLinear (mouseMove, camera.UpVector, camera.TargetVector);
					break;
				}
				CurrentInputAction = action;

				// scroll wheel zoom
				if (MouseState.ScrollWheelValue < PreviousMouseState.ScrollWheelValue) {
					camera.TargetDistance -= 20;
					// arcball
					// camera.arcball.Zoom -= 10; 
				} else if (MouseState.ScrollWheelValue > PreviousMouseState.ScrollWheelValue) {
					camera.TargetDistance += 20;
					// arcball
					// camera.arcball.Zoom += 10; 
				}
			}
		}

		public void ResetMousePosition ()
		{
			if (MouseState != PreviousMouseState) {
				if (GrabMouseMovement || CurrentInputAction == InputAction.ArcballMove
					//|| (MouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton == ButtonState.Pressed)
					//|| (MouseState.RightButton == ButtonState.Pressed && PreviousMouseState.RightButton == ButtonState.Pressed)
				    ) {
					Mouse.SetPosition (device.Viewport.Width / 2, device.Viewport.Height / 2);
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

		public InputAction CurrentInputAction { get; private set; }
	}

	public enum InputAction
	{
		ArcballMove,
		TargetMove,
		FreeMouse,
		FPSMove,
		SelectedObjectMove
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
		public static bool IsHeldDown (this Keys key)
		{
			KeyboardState keyboardState = Keyboard.GetState ();
			// Is the key down?
			return keyboardState.IsKeyDown (key);
		}
	}
}

