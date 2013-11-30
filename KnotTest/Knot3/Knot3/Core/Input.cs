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

using Knot3.UserInterface;

namespace Knot3.Core
{
	/// <summary>
	/// Die abstrakte Klasse Input stellt eine Basisklasse für Input-Handler bereit.
	/// Sie ist ein GameStateComponent, existiert daher einmal pro GameState, und fängt
	/// in der Rolle als IKeyEvent Tastatureingaben ab.
	/// </summary>
	public abstract class Input : GameStateComponent, IKeyEventListener
	{
		// state atributes
		protected static bool FullscreenToggled;
		public static KeyboardState PreviousKeyboardState;
		public static MouseState PreviousMouseState;
		private static double LeftButtonClickTimer;
		private static double RightButtonClickTimer;
		public static ClickState LeftButton;
		public static ClickState RightButton;

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

			ValidKeys = new List<Keys> ();
			ValidKeys.AddRange (new []{ Keys.G, Keys.F11 });
		}

		public override void Update (GameTime gameTime)
		{
			// update saved state.
			PreviousKeyboardState = KeyboardState;
			PreviousMouseState = MouseState;
			KeyboardState = Keyboard.GetState ();
			MouseState = Mouse.GetState ();

			if (gameTime != null) {
				LeftButtonClickTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
				if (Mouse.GetState ().LeftButton == ButtonState.Pressed) {
					LeftButton = LeftButtonClickTimer < 500 ? ClickState.DoubleClick : ClickState.SingleClick;
					LeftButtonClickTimer = 0;
					Console.WriteLine ("LeftButton=" + LeftButton.ToString ());
				} else {
					LeftButton = ClickState.None;
				}
				RightButtonClickTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
				if (Mouse.GetState ().LeftButton == ButtonState.Pressed) {
					RightButton = RightButtonClickTimer < 500 ? ClickState.DoubleClick : ClickState.SingleClick;
					RightButtonClickTimer = 0;
					Console.WriteLine ("RightButton=" + RightButton.ToString ());
				} else {
					RightButton = ClickState.None;
				}
			}

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

		public void OnKeyEvent (List<Keys> key, KeyEvent keyEvent, GameTime gameTime)
		{
			UpdateKeys (gameTime);
		}

		public List<Keys> ValidKeys { get; set; }

		public bool IsKeyEventEnabled { get { return true; } }

		public static MouseState MouseState { get; protected set; }

		public static KeyboardState KeyboardState { get; private set; }
	}

	public enum InputAction
	{
		None = 0,
		ArcballMove,
		TargetMove,
		FreeMouse,
		FPSMove,
		SelectedObjectMove,
		SelectedObjectShadowMove
	}

	public enum WASDMode
	{
		ArcballMode,
		FirstPersonMode
	}

	public enum ClickState
	{
		None = 0,
		SingleClick,
		DoubleClick
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
	}
}

