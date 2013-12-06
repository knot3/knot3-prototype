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

using Knot3.Core;
using Knot3.Utilities;
using Knot3.GameObjects;

namespace Knot3.CreativeMode
{
	public class KnotModeInput : Core.Input
	{
		// game world
		private World World { get; set; }

		private Camera camera { get { return World.Camera; } }

		// ...
		private int wasdSpeed = 10;

		public KnotModeInput (GameState state, World world)
			: base(state)
		{
			// game world
			World = world;

			// default values
			GrabMouseMovement = false;
			ResetMousePosition ();

			// keys to accept
			ValidKeys.AddRange (
				new []{
					Keys.A, Keys.D, Keys.W, Keys.S, Keys.R, Keys.F, Keys.Q, Keys.E, Keys.A, Keys.D, Keys.W,
					Keys.S, Keys.R, Keys.F, Keys.Q, Keys.E,
					Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.OemPlus, Keys.OemMinus, Keys.Enter,
					Keys.LeftAlt, Keys.Tab
				}
			);
		}

		protected override void UpdateKeys (GameTime gameTime)
		{
			Vector3 keyboardMove = Vector3.Zero;
			Vector2 arcballMove = Vector2.Zero;
			Vector2 selfRotate = Vector2.Zero;

			// W,A,S,D,Q,E
			if (WASDMode == WASDMode.ArcballMode) {
				if (Keys.A.IsHeldDown ())
					arcballMove += new Vector2 (-1, 0);
				if (Keys.D.IsHeldDown ())
					arcballMove += new Vector2 (1, 0);
				if (Keys.W.IsHeldDown ())
					arcballMove += new Vector2 (0, -1);
				if (Keys.S.IsHeldDown ())
					arcballMove += new Vector2 (0, 1);
				if (Keys.R.IsHeldDown ())
					keyboardMove += new Vector3 (0, 0, -1);
				if (Keys.F.IsHeldDown ())
					keyboardMove += new Vector3 (0, 0, 1);
				if (Keys.Q.IsHeldDown ())
					selfRotate += new Vector2 (-1, 0);
				if (Keys.E.IsHeldDown ())
					selfRotate += new Vector2 (1, 0);
			} else if (WASDMode == WASDMode.FirstPersonMode) {
				if (Keys.A.IsHeldDown ())
					keyboardMove += new Vector3 (-1, 0, 0);
				if (Keys.D.IsHeldDown ())
					keyboardMove += new Vector3 (1, 0, 0);
				if (Keys.W.IsHeldDown ())
					keyboardMove += new Vector3 (0, -1, 0);
				if (Keys.S.IsHeldDown ())
					keyboardMove += new Vector3 (0, 1, 0);
				if (Keys.R.IsHeldDown ())
					keyboardMove += new Vector3 (0, 0, -1);
				if (Keys.F.IsHeldDown ())
					keyboardMove += new Vector3 (0, 0, 1);
				if (Keys.Q.IsHeldDown ())
					selfRotate += new Vector2 (-1, 0);
				if (Keys.E.IsHeldDown ())
					selfRotate += new Vector2 (1, 0);
			}

			// Arrow Keys
			if (Keys.Left.IsHeldDown ())
				keyboardMove += new Vector3 (-1, 0, 0);
			if (Keys.Right.IsHeldDown ())
				keyboardMove += new Vector3 (1, 0, 0);
			if (Keys.Up.IsHeldDown ())
				keyboardMove += new Vector3 (0, -1, 0);
			if (Keys.Down.IsHeldDown ())
				keyboardMove += new Vector3 (0, 1, 0);

			// apply keyboard movements
			if (keyboardMove.Length () > 0) {
				keyboardMove *= wasdSpeed;
				// linear move, target and position
				camera.Target = camera.Target.MoveLinear (keyboardMove, camera.UpVector, camera.TargetDirection);
				camera.Position = camera.Position.MoveLinear (keyboardMove, camera.UpVector, camera.TargetDirection);
				CurrentInputAction = InputAction.FPSMove;
			}

			// apply arcball movements to the target
			if (arcballMove.Length () > 0) {
				arcballMove *= 3;
				camera.Target = new Vector3 (camera.ArcballTarget.X, camera.Target.Y, camera.ArcballTarget.Z);
				camera.TargetDistance = camera.TargetDistance.Clamp (500, 10000);
				camera.Position = camera.ArcballTarget + (camera.Position - camera.ArcballTarget).ArcBallMove (
						arcballMove, camera.UpVector, camera.TargetDirection
				);
				CurrentInputAction = InputAction.ArcballMove;
			}

			// apply arcball movements to the camera
			if (selfRotate.Length () > 0) {
				camera.Target = camera.Position + (camera.Target - camera.Position).ArcBallMove (
						selfRotate, camera.UpVector, camera.TargetDirection
				);
				CurrentInputAction = InputAction.ArcballMove;
			}

			// Plus/Minus Keys
			if (Keys.OemPlus.IsHeldDown () && wasdSpeed < 20) {
				wasdSpeed += 1;
			}
			if (Keys.OemMinus.IsHeldDown () && wasdSpeed > 1) {
				wasdSpeed -= 1;
			}

			// Enter key
			if (Keys.Enter.IsHeldDown ()) {
				// set target to (0,0,0)
				camera.Target = Vector3.Zero;
				// set position to default
				camera.Position = camera.DefaultPosition;
				// don't select a game object
				World.SelectObject (null, gameTime);
			}

			// grab mouse movent
			if (Keys.LeftAlt.IsDown ()) {
				GrabMouseMovement = !GrabMouseMovement;
				World.SelectObject (null, gameTime);
			}

			// switch WASD mode
			if (Keys.Tab.IsDown ()) {
				switch (WASDMode) {
				case WASDMode.ArcballMode:
					WASDMode = WASDMode.FirstPersonMode;
					break;
				case WASDMode.FirstPersonMode:
					WASDMode = WASDMode.ArcballMode;
					break;
				}
			}
			
			base.UpdateKeys (gameTime);
		}

		public override void Update (GameTime gameTime)
		{
			base.Update (gameTime);
			ResetMousePosition ();
		}

		protected override void UpdateMouse (GameTime gameTime)
		{

			// fullscreen recently toggled?
			if (FullscreenToggled) {
				FullscreenToggled = false;

			} else if (MouseState != PreviousMouseState) {
				// mouse movements
				Vector2 mouseMove = new Vector2 (MouseState.X - PreviousMouseState.X, MouseState.Y - PreviousMouseState.Y);
				//Console.WriteLine ("mouseMove=" + mouseMove);

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
						if (World.SelectedObject != null && World.SelectedObject.Info.IsMovable)
							action = InputAction.SelectedObjectShadowMove;
						else
							action = InputAction.TargetMove;
					} else if (MouseState.LeftButton == ButtonState.Released && PreviousMouseState.LeftButton == ButtonState.Pressed) {
						if (World.SelectedObject != null && World.SelectedObject.Info.IsMovable)
							action = InputAction.SelectedObjectMove;
						else
							action = InputAction.TargetMove;
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
					camera.TargetDistance = camera.TargetDistance.Clamp (500, 10000);
					camera.Position = camera.ArcballTarget + (camera.Position - camera.ArcballTarget).ArcBallMove (
						mouseMove, camera.UpVector, camera.TargetDirection
					);
					break;
				// move the target vector
				case InputAction.TargetMove:
					camera.Target = camera.Target.MoveLinear (mouseMove, camera.UpVector, camera.TargetDirection);
					break;
				}
				CurrentInputAction = action;

				// scroll wheel zoom
				if (MouseState.ScrollWheelValue < PreviousMouseState.ScrollWheelValue) {
					camera.TargetDistance += 40;
				} else if (MouseState.ScrollWheelValue > PreviousMouseState.ScrollWheelValue) {
					camera.TargetDistance -= 40;
				}
			}

			base.UpdateMouse (gameTime);
		}

		private void ResetMousePosition ()
		{
			if (MouseState != PreviousMouseState) {
				if (GrabMouseMovement || (CurrentInputAction == InputAction.ArcballMove)) {
					Mouse.SetPosition (device.Viewport.Width / 2, device.Viewport.Height / 2);
					Core.Input.MouseState = Mouse.GetState ();
				}
			}
		}
	}
}

