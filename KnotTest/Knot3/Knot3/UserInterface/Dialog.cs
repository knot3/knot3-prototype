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
using Knot3.Utilities;

namespace Knot3.UserInterface
{
	public abstract class Dialog : Widget
	{
		// menu
		public Menu buttons;
		public Action Done;

		// textures
		protected SpriteBatch spriteBatch;

		public Dialog (GameScreen screen, WidgetInfo info, DisplayLayer drawOrder)
			: base(screen, info, drawOrder)
		{
			IsVisible = true;
			Done = () => {
				IsVisible = false;
			};

			// create a new SpriteBatch, which can be used to draw textures
			spriteBatch = new SpriteBatch (screen.device);

			// menu
			WidgetInfo menuInfo = new WidgetInfo ();
			buttons = new Menu (screen, menuInfo, DisplayLayer.Menu);
			buttons.ItemForegroundColor = ButtonForegroundColor;
			buttons.ItemBackgroundColor = ButtonBackgroundColor;
			buttons.ItemAlignX = HAlign.Center;
			buttons.ItemAlignY = VAlign.Center;
		}

		protected Vector2 RelativeButtonPosition (int n)
		{
			Vector2 buttonSize = RelativeButtonSize (n);
			return new Vector2 (
				Info.RelativePosition ().X + Info.RelativePadding ().X * (1 + n) + buttonSize.X * n,
				Info.RelativePosition ().Y + Info.RelativeSize ().Y - buttonSize.Y - Info.RelativePadding ().Y
			);
		}

		protected Vector2 RelativeButtonSize (int n)
		{
			float x = (Info.RelativeSize ().X - Info.RelativePadding ().X * (1 + buttons.Count)) / buttons.Count;
			return new Vector2 (x, 0.06f);
		}

		public override IEnumerable<IGameScreenComponent> SubComponents (GameTime gameTime)
		{
			foreach (DrawableGameScreenComponent component in base.SubComponents(gameTime)) {
				yield return component;
			}
			yield return buttons;
		}

		public override void Draw (GameTime gameTime)
		{
			if (IsVisible) {
				spriteBatch.Begin ();

				// background
				Rectangle rect = Info.ScaledRectangle (screen.viewport);
				spriteBatch.Draw (
					Textures.Create (screen.device, HfGDesign.LineColor), rect.Grow (3), Color.White
				);
				spriteBatch.Draw (
					Textures.Create (screen.device, Info.BackgroundColor ()), rect, Color.Black * 0.95f
				);

				DrawDialog (gameTime);
			
				spriteBatch.End ();
			}
		}

		protected abstract void DrawDialog (GameTime gameTime);

		private Color ButtonBackgroundColor (ItemState itemState)
		{
			if (itemState == ItemState.Selected)
				return HfGDesign.LineColor * 0.6f;
			else
				return HfGDesign.LineColor * 0.8f;
		}

		private Color ButtonForegroundColor (ItemState itemState)
		{
			if (itemState == ItemState.Selected)
				return Color.Black;
			else
				return Color.Black;
		}
	}
}

