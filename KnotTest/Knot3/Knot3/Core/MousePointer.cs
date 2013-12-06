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
	/// Ein DrawableGameStateComponent, der einen Mauszeiger zeichnet.
	/// </summary>
	public class MousePointer : DrawableGameStateComponent
	{
		// graphics-related classes
		private SpriteBatch spriteBatch;

		/// <summary>
		/// Initializes a new mouse pointer.
		/// </summary>
		/// <param name='state'>
		/// State.
		/// </param>
		public MousePointer (GameState state)
			: base(state, DisplayLayer.Cursor)
		{
			// create a new SpriteBatch, which can be used to draw textures
			spriteBatch = new SpriteBatch (state.device);
		}

		/// <summary>
		/// Draw the Overlay.
		/// </summary>
		/// <param name='gameTime'>
		/// Game time.
		/// </param>
		public override void Draw (GameTime gameTime)
		{
			DrawCursor (gameTime);
		}

		private void DrawCursor (GameTime gameTime)
		{
			if (!Utilities.Mono.IsRunningOnMono ()) {
				spriteBatch.Begin ();
            
				Texture2D cursorTex = state.content.Load<Texture2D> ("cursor");
				if (state.input.GrabMouseMovement || state.input.CurrentInputAction == InputAction.TargetMove
					|| (state.input.CurrentInputAction == InputAction.ArcballMove
                    && (Input.MouseState.LeftButton == ButtonState.Pressed || Input.MouseState.RightButton == ButtonState.Pressed)))
                {
					spriteBatch.Draw (cursorTex, state.device.Viewport.Center (), Color.White);
				} else {
					spriteBatch.Draw (cursorTex, new Vector2 (Input.MouseState.X, Input.MouseState.Y), Color.White);
				}

				spriteBatch.End ();
			}
		}
	}
}

