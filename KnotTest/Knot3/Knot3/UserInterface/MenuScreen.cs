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

using Knot3.Core;

namespace Knot3.UserInterface
{
	public abstract class MenuScreen : GameScreen
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

		public MenuScreen (Core.Game game)
			: base(game)
		{
			LinePoints = new List<Vector2> ();
			LineWidth = 6;
		}

		public override void Initialize ()
		{
			// create a new SpriteBatch, which can be used to draw textures
			spriteBatch = new SpriteBatch (graphics.GraphicsDevice);

			// pointer
			pointer = new MousePointer (this);
		}

		public override void Update (GameTime gameTime)
		{
			// subclass...
			UpdateMenu (gameTime);
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

			PostProcessing.End (gameTime);
		}

		public abstract void DrawMenu (GameTime gameTime);

		public override void Activate (GameTime gameTime)
		{
			base.Activate (gameTime);
			AddGameComponents (gameTime, pointer);
		}

		public override void Unload ()
		{
		}

		protected Color BackgroundColor (ItemState itemState)
		{
			return Color.Black * 0f;
		}

		protected Color ForegroundColor (ItemState itemState)
		{
			if (itemState == ItemState.Selected)
				return Color.White;
			else
				return Color.White * 0.7f;
		}
	}
}

