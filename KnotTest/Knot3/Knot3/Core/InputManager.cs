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
	/// Die Klasse Input stellt eine statische Methoden bereit, die von allen Input-Handlern benötigt werden.
	/// Sie ist ein GameScreenComponent und existiert daher einmal pro GameScreen.
	/// </summary>
	public class InputManager : GameScreenComponent
	{
		#region Attributes

		// screen atributes
		public static bool FullscreenToggled;
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

		public InputAction CurrentInputAction { get; set; }

		public WASDMode WASDMode { get; set; }

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="TestGame1.Input"/> class.
		/// </summary>
		/// <param name='screen'>
		/// Game State.
		/// </param>
		public InputManager (GameScreen screen)
			: base(screen, DisplayLayer.None)
		{
			FullscreenToggled = false;
			WASDMode = WASDMode.ArcballMode;
			CurrentInputAction = InputAction.FreeMouse;

			PreviousKeyboardState = KeyboardState = Keyboard.GetState ();
			PreviousMouseState = MouseState = Mouse.GetState ();
		}

		public override void Update (GameTime gameTime)
		{
			// update saved screen.
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

			// fullscreen
			if (Keys.G.IsDown () || Keys.F11.IsDown ()) {
				screen.game.IsFullscreen = !screen.game.IsFullscreen;
				FullscreenToggled = true;
			}
		}

		/// <summary>
		/// Der aktuelle Status der Maus.
		/// </summary>
		public static MouseState MouseState { get; set; }

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
			if (InputManager.KeyboardState.IsKeyDown (key)) {
				// If not down last update, key has just been pressed.
				if (!InputManager.PreviousKeyboardState.IsKeyDown (key)) {
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
			return InputManager.KeyboardState.IsKeyDown (key);
		}
	}
}

