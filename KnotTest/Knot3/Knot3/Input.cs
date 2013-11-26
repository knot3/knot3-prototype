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

namespace Knot3
{
	public abstract class Input : GameComponent
	{
		// state atributes
		protected static bool FullscreenToggled;
		public static KeyboardState PreviousKeyboardState;
		public static MouseState PreviousMouseState;
		public static long LastLeftButtonPress;
		public static long LastRightButtonPress;

		public bool GrabMouseMovement { get; set; }

		// input modes
		public InputAction CurrentInputAction { get; protected set; }

		public WASDMode WASDMode { get; protected set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TestGame1.Input"/> class.
		/// </summary>
		/// <param name='state'>
		/// Game State.
		/// </param>
		public Input (GameState state)
			: base(state, DisplayLayer.None)
		{
			FullscreenToggled = false;
			WASDMode = WASDMode.ArcballMode;
			CurrentInputAction = InputAction.FreeMouse;

			PreviousKeyboardState = KeyboardState = Keyboard.GetState ();
			PreviousMouseState = MouseState = Mouse.GetState ();
		}

		public override void Update (GameTime gameTime)
		{
			// update saved state.
			PreviousKeyboardState = KeyboardState;
			PreviousMouseState = MouseState;
			KeyboardState = Keyboard.GetState ();
			MouseState = Mouse.GetState ();

			if (gameTime != null) {
				if (MouseState.LeftButton == ButtonState.Pressed) {
					LastLeftButtonPress = gameTime.TotalGameTime.Milliseconds;
				} else if (MouseState.RightButton == ButtonState.Pressed) {
					LastRightButtonPress = gameTime.TotalGameTime.Milliseconds;
				}
			}

			UpdateKeys (gameTime);
			UpdateMouse (gameTime);
		}

		protected virtual void UpdateKeys (GameTime gameTime)
		{
			// fullscreen
			if (Keys.G.IsDown () || Keys.F11.IsDown ()) {
				state.game.IsFullscreen = !state.game.IsFullscreen;
				FullscreenToggled = true;
			}
		}

		protected virtual void UpdateMouse (GameTime gameTime)
		{
		}

		public static MouseState MouseState { get; protected set; }

		public static KeyboardState KeyboardState { get; private set; }
	}

	public enum InputAction
	{
		ArcballMove,
		TargetMove,
		FreeMouse,
		FPSMove,
		SelectedObjectMove
	}

	public enum WASDMode
	{
		ArcballMode,
		FirstPersonMode
	}

	public static class InputExtensions
	{
		public static bool IsDown (this Keys key)
		{
			// Is the key down?
			if (Input.KeyboardState.IsKeyDown (key)) {
				// If not down last update, key has just been pressed.
				if (!Input.PreviousKeyboardState.IsKeyDown (key)) {
					return true;
				}
			}
			return false;
		}

		public static bool IsHeldDown (this Keys key)
		{
			// Is the key down?
			return Input.KeyboardState.IsKeyDown (key);
		}

		public static bool IsLeftClick (this MouseState state, GameTime gameTime)
		{
			if (state.LeftButton == ButtonState.Pressed && Input.PreviousMouseState.LeftButton != ButtonState.Pressed) {
				Console.WriteLine ("IsLeftClick=true");
				return true;
			} else {
				return false;
			}
		}

		public static bool IsDoubleClick (this MouseState state, GameTime gameTime)
		{
			if (state.IsLeftClick (gameTime)) {
				long timeDiff = gameTime.TotalGameTime.Milliseconds - Input.LastLeftButtonPress;
				if (timeDiff < 1000 && timeDiff > 10) {
					Console.WriteLine ("IsLeftDoubleClick=true");
					return true;
				} else {
					return false;
				}
			} else {
				return false;
			}
		}
	}
}

