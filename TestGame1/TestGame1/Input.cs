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
	public abstract class Input : GameClass
	{
		// state atributes
		protected static bool FullscreenToggled;
		public static KeyboardState PreviousKeyboardState;
		public static MouseState PreviousMouseState;

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
			: base(state)
		{
			FullscreenToggled = false;
			WASDMode = WASDMode.ArcballMode;
			CurrentInputAction = InputAction.FreeMouse;
		}

		public void Update (GameTime gameTime)
		{
			UpdateKeys (gameTime);
			UpdateMouse (gameTime);
		}

		protected virtual void UpdateKeys (GameTime gameTime)
		{
			// fullscreen
			if (Keys.F.IsDown () || Keys.F11.IsDown ()) {
				Console.WriteLine ("Fullscreen Toggle");
				if (graphics.IsFullScreen == false) {
					graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.DisplayMode.Width;
					graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.DisplayMode.Height;
				} else {
					graphics.PreferredBackBufferWidth = Game.DefaultSize.Width;
					graphics.PreferredBackBufferHeight = Game.DefaultSize.Height;
				}
				graphics.ToggleFullScreen ();
				graphics.ApplyChanges ();
				FullscreenToggled = true;
			}
		}

		protected virtual void UpdateMouse (GameTime gameTime)
		{
		}

		public virtual void SaveStates ()
		{
			// update saved state.
			PreviousMouseState = MouseState;
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
		FirstPersonMode,
		RotationMode
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

