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
	public class ConfirmDialog : Dialog
	{
		// text
		protected string[] Text;
		protected bool CanClose = true;

		// actions
		protected Action OnYesClick = () => {};
		protected Action OnNoClick = () => {};
		protected Action OnCancelClick = () => {};

		public ConfirmDialog (GameScreen screen, WidgetInfo info, DisplayLayer drawOrder)
			: base(screen, info, drawOrder)
		{
			// text
			Text = new string[]{};

			// actions
			Action onYesClick = () => {
				OnYesClick ();
				if (CanClose) {
					Done ();
				}
			};
			Action onNoClick = () => {
				OnNoClick ();
				Done ();
			};
			Action onCancelClick = () => {
				OnCancelClick ();
				Done ();
			};

			// buttons
			buttons.RelativeItemPosition = RelativeButtonPosition;
			buttons.RelativeItemSize = RelativeButtonSize;
			var itemInfo = new MenuItemInfo () {
				Text = "Yes",
				OnClick = onYesClick
			};
			buttons.AddButton (itemInfo);
			itemInfo = new MenuItemInfo () {
				Text = "No",
				OnClick = onNoClick
			};
			buttons.AddButton (itemInfo);
			itemInfo = new MenuItemInfo () {
				Text = "Cancel",
				OnClick = onCancelClick
			};
			buttons.AddButton (itemInfo);
		}

		protected override void DrawDialog (GameTime time)
		{
			SpriteFont font = HfGDesign.MenuFont (screen);
			// text
			for (int i = 0; i < Text.Length; ++i) {
				string line = Text [i];
				float scale = 0.15f * screen.viewport.ScaleFactor ().Length ();
				Vector2 size = font.MeasureString (line).RelativeTo (screen.viewport) * scale;
				Vector2 pos = new Vector2 (
					(Info.RelativeSize ().X - size.X) / 2,
					Info.RelativePadding ().Y + size.Y * i
				);
				spriteBatch.DrawString (
					font, line, Info.ScaledPosition (screen.viewport) + pos.Scale (screen.viewport),
					Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0
				);
			}
		}
	}
}

