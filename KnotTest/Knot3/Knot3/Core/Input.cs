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
using Knot3.Utilities;

namespace Knot3.Core
{
	/// <summary>
	/// Die abstrakte Klasse Input stellt eine Basisklasse für Input-Handler bereit.
	/// Sie ist ein GameStateComponent, existiert daher einmal pro GameState, und fängt
	/// in der Rolle als IKeyEvent Tastatureingaben ab.
	/// </summary>
	public abstract class Input : GameStateComponent, IKeyEventListener
	{
		#region Attributes

		// state atributes
		protected static bool FullscreenToggled;
		/// <summary>
		/// Der Status der Tastatur zur Zeit des vorherigen Frames.
		/// </summary>
		public static KeyboardState PreviousKeyboardState;
		/// <summary>
		/// Der Status der Maus zur Zeit des vorherigen Frames.
		/// </summary>
		public static MouseState PreviousMouseState;
		private static double LeftButtonClickTimer;
		private static double RightButtonClickTimer;
		private static MouseState PreviousClickMouseState;
		/// <summary>
		/// Der aktuelle ClickState des linken Mouse Buttons.
		/// </summary>
		public static ClickState LeftButton;
		/// <summary>
		/// Der aktuelle ClickState des rechten Mouse Buttons.
		/// </summary>
		public static ClickState RightButton;

		#endregion

		#region Properties

		public bool GrabMouseMovement { get; set; }

		public InputAction CurrentInputAction { get; protected set; }

		public WASDMode WASDMode { get; protected set; }

		#endregion

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
				bool mouseMoved;
				if (MouseState != PreviousMouseState) {
					// mouse movements
					Vector2 mouseMove = MouseState.ToVector2 () - PreviousClickMouseState.ToVector2 ();
					mouseMoved = mouseMove.Length () > 3;
				} else {
					mouseMoved = false;
				}

				LeftButtonClickTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
				if (MouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton != ButtonState.Pressed) {
					LeftButton = LeftButtonClickTimer < 500 && !mouseMoved
						? ClickState.DoubleClick : ClickState.SingleClick;
					LeftButtonClickTimer = 0;
					PreviousClickMouseState = PreviousMouseState;
					Console.WriteLine ("LeftButton=" + LeftButton.ToString ());
				} else {
					LeftButton = ClickState.None;
				}
				RightButtonClickTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
				if (MouseState.RightButton == ButtonState.Pressed && PreviousMouseState.RightButton != ButtonState.Pressed) {
					RightButton = RightButtonClickTimer < 500 && !mouseMoved
						? ClickState.DoubleClick : ClickState.SingleClick;
					RightButtonClickTimer = 0;
					PreviousClickMouseState = PreviousMouseState;
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

		/// <summary>
		/// Der aktuelle Status der Maus.
		/// </summary>
		public static MouseState MouseState { get; protected set; }

		/// <summary>
		/// Der aktuelle Status der Tastatur.
		/// </summary>
		public static KeyboardState KeyboardState { get; private set; }
	}

	/// <summary>
	/// Die aktuelle Eingabeaktion.
	/// </summary>
	public enum InputAction
	{
		/// <summary>
		/// Keine Eingabeaktion.
		/// </summary>
		None = 0,
		/// <summary>
		/// Bewegung der Kamera "wie auf einer Kugel-Oberfläche", das heißt in einem festen Radius, um ein Objekt.
		/// </summary>
		ArcballMove,
		/// <summary>
		/// Bewegung des Kamera-Targets.
		/// </summary>
		TargetMove,
		/// <summary>
		/// Freie Bewegung der Maus ohne einen Effekt auf die Spielwelt.
		/// </summary>
		FreeMouse,
		/// <summary>
		/// Bewegung der Kamera wie in einem First Person Shooter.
		/// </summary>
		FPSMove,
		/// <summary>
		/// Bewegung des ausgewählten Spielobjekts (siehe GameObjects.World).
		/// </summary>
		SelectedObjectMove,
		/// <summary>
		/// Schattenhafte Darstellung der Bewegung des ausgewählten Spielobjekts vor dem Einrasten in ein 3D-Raster.
		/// </summary>
		SelectedObjectShadowMove
	}

	/// <summary>
	/// Die aktuelle Belegung für die Tasten W,A,S,D und angrenzende Tasten.
	/// </summary>
	public enum WASDMode
	{
		/// <summary>
		/// W,A,S,D bewegen die Kamera "wie auf einer Kugel-Oberfläche", das heißt in einem festen Radius, um ein Objekt.
		/// </summary>
		ArcballMode,
		/// <summary>
		/// W,A,S,D bewegen die Kamera wie in einem First Person Shooter.
		/// </summary>
		FirstPersonMode
	}

	/// <summary>
	/// Das Enum ClickState steht entweder für einen Doppelklick, einen einfachen Klick oder gar kein Klick.
	/// </summary>
	public enum ClickState
	{
		/// <summary>
		/// Kein Klick.
		/// </summary>
		None = 0,
		/// <summary>
		/// Ein einfacher Klick.
		/// </summary>
		SingleClick,
		/// <summary>
		/// Ein Doppelklick.
		/// </summary>
		DoubleClick
	}

	public static class InputExtensions
	{
		/// <summary>
		/// Wurde die aktuelle Taste gedrückt und war sie im letzten Frame nicht gedrückt?
		/// </summary>
		/// <returns>
		/// <c>true</c> if the specified key is down; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='key'>
		/// If set to <c>true</c> key.
		/// </param>
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

		/// <summary>
		/// Wird die aktuelle Taste gedrückt gehalten?
		/// </summary>
		/// <returns>
		/// <c>true</c> if the specified key is held down; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='key'>
		/// If set to <c>true</c> key.
		/// </param>
		public static bool IsHeldDown (this Keys key)
		{
			// Is the key down?
			return Input.KeyboardState.IsKeyDown (key);
		}
	}
}

