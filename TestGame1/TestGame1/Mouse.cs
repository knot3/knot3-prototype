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
	public class MousePointer : GameClass
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
			: base(state)
		{
			// create a new SpriteBatch, which can be used to draw textures
			spriteBatch = new SpriteBatch (graphics.GraphicsDevice);
		}

		/// <summary>
		/// Loads the content.
		/// </summary>
		public void LoadContent ()
		{
		}

		/// <summary>
		/// Draw the Overlay.
		/// </summary>
		/// <param name='gameTime'>
		/// Game time.
		/// </param>
		public void Draw (GameTime gameTime)
		{
			DrawCursor (gameTime);
		}
		
		public void Update (GameTime gameTime)
		{
		}

		private void DrawCursor (GameTime gameTime)
		{
			if (!Mono.IsRunningOnMono ()) {
				spriteBatch.Begin ();
            
				Texture2D cursorTex = content.Load<Texture2D> ("cursor");
				if (input.GrabMouseMovement || input.CurrentInputAction == InputAction.TargetMove
					|| input.CurrentInputAction == InputAction.ArcballMove) {
					spriteBatch.Draw (cursorTex, graphics.GraphicsDevice.Viewport.Center (), Color.White);
				} else {
					spriteBatch.Draw (cursorTex, new Vector2 (Input.MouseState.X, Input.MouseState.Y), Color.White);
				}

				spriteBatch.End ();
			}
		}
	}
}

