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

namespace Knot3.UserInterface
{
	public abstract class MenuScreen : GameState
	{
		// graphics-related attributes
		private SpriteBatch spriteBatch;

		// colors
		private Color backColor = Color.Black;

		// ...
		private MousePointer pointer;

		// lines
		protected List<Vector2> LinePoints;
		protected int LineWidth;

		public MenuScreen (Game game)
			: base(game)
		{
			LinePoints = new List<Vector2> ();
			LineWidth = 6;
		}

		public override void Initialize ()
		{
			// create a new SpriteBatch, which can be used to draw textures
			spriteBatch = new SpriteBatch (graphics.GraphicsDevice);

			// input
			input = new StartScreenInput (this);
			input.SaveStates (null);

			// pointer
			pointer = new MousePointer (this);
		}

		public override void Update (GameTime gameTime)
		{
			// subclass...
			UpdateMenu (gameTime);

			// input
			input.Update (gameTime);
			input.SaveStates (gameTime);

			// pointer
			pointer.Update (gameTime);
		}

		public abstract void UpdateMenu (GameTime gameTime);
		
		public override void Draw (GameTime gameTime)
		{
			PostProcessing.Begin (backColor, gameTime);

			// subclass...
			DrawMenu (gameTime);

			// lines
			spriteBatch.Begin ();
			HfGDesign.DrawLines (ref LinePoints, LineWidth, spriteBatch, this, gameTime);
			spriteBatch.End ();

			// pointer
			pointer.Draw (gameTime);

			PostProcessing.End (gameTime);
		}

		public abstract void DrawMenu (GameTime gameTime);

		public override void Unload ()
		{
		}

		public override void Activate (GameTime gameTime)
		{
		}

		public override void Deactivate (GameTime gameTime)
		{
		}

		protected Color BackgroundColor (MenuItemState itemState)
		{
			switch (itemState) {
			case MenuItemState.Selected:
				return Color.Black * 0f;
			case MenuItemState.Normal:
			default:
				return Color.Black * 0f;
			}
		}

		protected Color ForegroundColor (MenuItemState itemState)
		{
			switch (itemState) {
			case MenuItemState.Selected:
				return Color.White;
			case MenuItemState.Normal:
			default:
				return Color.White * 0.7f;
			}
		}
	}
}

