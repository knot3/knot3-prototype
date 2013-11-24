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

namespace Knot3.UserInterface
{
	public class ConfirmDialog : Dialog
	{
		// text
		protected string[] Text;

		// actions
		protected Action OnYesClick = () => {};
		protected Action OnNoClick = () => {};
		protected Action OnCancelClick = () => {};

		public ConfirmDialog (GameState state)
			: base(state)
		{
			// text
			Text = new string[]{};

			// actions
			Action onYesClick = () => {
				OnYesClick ();
				Done ();
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
			buttons.AddButton (new MenuItemInfo ("Yes", ButtonPosition, ButtonSize, onYesClick));
			buttons.AddButton (new MenuItemInfo ("No", ButtonPosition, ButtonSize, onNoClick));
			buttons.AddButton (new MenuItemInfo ("Cancel", ButtonPosition, ButtonSize, onCancelClick));
		}

		protected override bool UpdateDialog (GameTime gameTime)
		{
			return false;
		}

		protected override void DrawDialog (GameTime gameTime)
		{
			// text
			for (int i = 0; i < Text.Length; ++i) {
				string line = Text [i];
				float scale = 0.15f * viewport.ScaleFactor ().Length ();
				Vector2 size = buttons.Font.MeasureString (line).RelativeTo (viewport) * scale;
				Vector2 pos = Position () + new Vector2 ((Size ().X - size.X) / 2,
				                                       Padding ().Y + size.Y * i);
				spriteBatch.DrawString (buttons.Font, line, pos.Scale (viewport), Color.White,
						0, Vector2.Zero, scale, SpriteEffects.None, 0);
			}
		}
	}
}

