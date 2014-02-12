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

using Knot3.Utilities;

namespace Knot3.Core
{
	/// <summary>
	/// Ein DrawableGameScreenComponent, der einen Mauszeiger zeichnet.
	/// </summary>
	public class MousePointer : DrawableGameScreenComponent
	{
		// graphics-related classes
		private SpriteBatch spriteBatch;

		/// <summary>
		/// Initializes a new mouse pointer.
		/// </summary>
		/// <param name='screen'>
		/// State.
		/// </param>
		public MousePointer (GameScreen screen)
		: base(screen, DisplayLayer.Cursor)
		{
			// create a new SpriteBatch, which can be used to draw textures
			spriteBatch = new SpriteBatch (screen.device);
		}

		/// <summary>
		/// Draw the Overlay.
		/// </summary>
		/// <param name='time'>
		/// Game time.
		/// </param>
		public override void Draw (GameTime time)
		{
			DrawCursor (time);
		}

		private void DrawCursor (GameTime time)
		{
			if (!Utilities.Mono.IsRunningOnMono ()) {
				spriteBatch.Begin ();

				Texture2D cursorTex = screen.content.Load<Texture2D> ("cursor");
				if (screen.input.GrabMouseMovement || screen.input.CurrentInputAction == InputAction.TargetMove
				        || (screen.input.CurrentInputAction == InputAction.ArcballMove
				            && (InputManager.CurrentMouseState.LeftButton == ButtonState.Pressed || InputManager.CurrentMouseState.RightButton == ButtonState.Pressed))) {
					spriteBatch.Draw (cursorTex, screen.device.Viewport.Center (), Color.White);
				}
				else {
					spriteBatch.Draw (cursorTex, new Vector2 (InputManager.CurrentMouseState.X, InputManager.CurrentMouseState.Y), Color.White);
				}

				spriteBatch.End ();
			}
		}
	}
}
