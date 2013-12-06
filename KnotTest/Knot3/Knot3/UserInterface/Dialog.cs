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

		public Dialog (GameState state, DisplayLayer drawOrder)
			: base(state, drawOrder, ()=>Color.White, ()=>Color.Black, HAlign.Left, VAlign.Top)
		{
			RelativePosition = () => (Vector2.One - RelativeSize ()) / 2;
			RelativeSize = () => Vector2.Zero;
			RelativePadding = () => new Vector2 (0.016f, 0.016f);
			IsVisible = true;
			Done = () => {
				IsVisible = false;
			};

			// create a new SpriteBatch, which can be used to draw textures
			spriteBatch = new SpriteBatch (state.device);

			// menu
			buttons = new Menu (state, DisplayLayer.Menu);
			buttons.Initialize (ButtonForegroundColor, ButtonBackgroundColor, HAlign.Center);
		}

		protected Vector2 RelativeButtonPosition (int n)
		{
			Vector2 buttonSize = RelativeButtonSize (n);
			return new Vector2 (
				RelativePosition ().X + RelativePadding ().X * (1 + n) + buttonSize.X * n,
				RelativePosition ().Y + RelativeSize ().Y - buttonSize.Y - RelativePadding ().Y
			);
		}

		protected Vector2 RelativeButtonSize (int n)
		{
			float x = (RelativeSize ().X - RelativePadding ().X * (1 + buttons.Count)) / buttons.Count;
			return new Vector2 (x, 0.06f);
		}

		public override IEnumerable<IGameStateComponent> SubComponents (GameTime gameTime)
		{
			foreach (DrawableGameStateComponent component in base.SubComponents(gameTime)) {
				yield return component;
			}
			yield return buttons;
		}

		public override void Draw (GameTime gameTime)
		{
			if (IsVisible) {
				spriteBatch.Begin ();

				// background
				Rectangle rect = HfGDesign.CreateRectangle (0, ScaledPosition, ScaledSize);
				spriteBatch.Draw (Textures.Create (state.device, HfGDesign.LineColor),
				                 rect.Grow (3),
				                 Color.White);
				spriteBatch.Draw (Textures.Create (state.device, BackgroundColor),
				                 rect,
				                 Color.Black * 0.95f);

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

