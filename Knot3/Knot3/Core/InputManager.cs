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
		public static ClickState LeftMouseButton;
		/// <summary>
		/// Der aktuelle ClickState des rechten Mouse Buttons.
		/// </summary>
		public static ClickState RightMouseButton;

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

			PreviousKeyboardState = CurrentKeyboardState = Keyboard.GetState ();
			PreviousMouseState = CurrentMouseState = Mouse.GetState ();
		}

		public override void Update (GameTime time)
		{
			// update saved screen.
			PreviousKeyboardState = CurrentKeyboardState;
			PreviousMouseState = CurrentMouseState;
			CurrentKeyboardState = Keyboard.GetState ();
			CurrentMouseState = Mouse.GetState ();

			if (time != null) {
				bool mouseMoved;
				if (CurrentMouseState != PreviousMouseState) {
					// mouse movements
					Vector2 mouseMove = CurrentMouseState.ToVector2 () - PreviousClickMouseState.ToVector2 ();
					mouseMoved = mouseMove.Length () > 3;
				}
				else {
					mouseMoved = false;
				}

				LeftButtonClickTimer += time.ElapsedGameTime.TotalMilliseconds;
				if (CurrentMouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton != ButtonState.Pressed) {
					LeftMouseButton = LeftButtonClickTimer < 500 && !mouseMoved
					                  ? ClickState.DoubleClick : ClickState.SingleClick;
					LeftButtonClickTimer = 0;
					PreviousClickMouseState = PreviousMouseState;
					Console.WriteLine ("LeftButton=" + LeftMouseButton.ToString ());
				}
				else {
					LeftMouseButton = ClickState.None;
				}
				RightButtonClickTimer += time.ElapsedGameTime.TotalMilliseconds;
				if (CurrentMouseState.RightButton == ButtonState.Pressed && PreviousMouseState.RightButton != ButtonState.Pressed) {
					RightMouseButton = RightButtonClickTimer < 500 && !mouseMoved
					                   ? ClickState.DoubleClick : ClickState.SingleClick;
					RightButtonClickTimer = 0;
					PreviousClickMouseState = PreviousMouseState;
					Console.WriteLine ("RightButton=" + RightMouseButton.ToString ());
				}
				else {
					RightMouseButton = ClickState.None;
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
		public static MouseState CurrentMouseState { get; set; }

		/// <summary>
		/// Der aktuelle Status der Tastatur.
		/// </summary>
		public static KeyboardState CurrentKeyboardState { get; private set; }
	}

	/// <summary>
	/// Die aktuelle Eingabeaktion.
	/// </summary>
	public enum InputAction {
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
}
