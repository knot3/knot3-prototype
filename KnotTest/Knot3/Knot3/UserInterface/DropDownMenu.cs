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

using Knot3.Settings;

namespace Knot3.UserInterface
{
	public class DropDownMenu : MenuItem
	{
		// drop-down menu
		private VerticalMenu dropdown;
		private MenuButton selected;
		private bool dropdownVisible;

		public DropDownMenu (GameState state, int itemNum, MenuItemInfo info,
		                 MenuItemColor fgColor, MenuItemColor bgColor, HAlign alignX)
			: base(state, itemNum, info, fgColor, bgColor, alignX)
		{
			// drop-down menu
			dropdown = new VerticalMenu (state);
			dropdown.Initialize (DropDownForegroundColor, DropDownBackgroundColor,
			                     HAlign.Left, new Border(new Color (0xb4, 0xff, 0x00), 5, 5, 0, 0));

			// selected value
			MenuItemInfo valueInfo = new MenuItemInfo (text: "---", position: ValuePosition,
						size: ValueSize, onClick: () => info.OnClick ());
			selected = new MenuButton (state, 0, valueInfo, fgColor, bgColor, HAlign.Left);

			// action to open the drop-down menu
            info.OnClick = () =>
            {
                if (dropdownVisible == true) { dropdownVisible = false; }
                else { dropdownVisible = true; }
                GameStates.VideoOptionScreen.Collapse(this);
            }
            ;
		}

		public void AddEntries (DropDownMenuItem[] entries, DropDownMenuItem defaultEntry)
		{
			foreach (DropDownMenuItem entry in entries) {
				Action onSelected = entry.OnSelected;
				onSelected += () => dropdownVisible = false;
				dropdown.AddButton (new MenuItemInfo (entry.Text, onSelected));
			}
			selected.Info.Text = defaultEntry.Text;
		}

		public void AddEntries (DistinctOptionInfo option)
		{
			foreach (string _value in option.ValidValues) {
				string value = _value; // create a copy for the action
				Action onSelected = () => {
					Console.WriteLine("OnClick: "+value);
					option.Value = value;
					selected.Info.Text = value;
					dropdownVisible = false;
				};
				dropdown.AddButton (new MenuItemInfo (value, onSelected));
			}
			selected.Info.Text = option.Value;
		}

		public override bool Update (GameTime gameTime)
		{
			bool activated = false;
			if (dropdownVisible) {
				// update dropdown menu
				activated = dropdown.Update (gameTime);
			} else {
				// update selected value
				activated = selected.Update (gameTime);
			}

			// update dropdown menu name
			if (!activated) {
				activated = base.Update (gameTime);
			}

			return activated;
		}

        public override void Collapse()
        {
            dropdownVisible = false;

        }

		public override void Draw (float layerDepth, SpriteBatch spriteBatch, SpriteFont font, GameTime gameTime)
		{
			base.Draw (layerDepth, spriteBatch, font, gameTime);

			if (dropdownVisible) {
				// draw dropdown menu
				Vector2 position = ValuePosition ();
				Vector2 size = ValueSize ();
				dropdown.Align (viewport, 1f, (int)position.X, (int)position.Y, (int)size.X, (int)size.Y, 0f);
				dropdown.Draw (layerDepth + 0.1f, spriteBatch, gameTime);
				
			} else {
				// draw selected value
				selected.Draw (layerDepth + 0.001f, spriteBatch, font, gameTime);
			}
		}

		private Vector2 ValuePosition (int dummy = 0)
		{
			Vector2 size = Info.SizeFunc (ItemNum);
			return Info.PositionFunc (ItemNum) + new Vector2 (size.X / 2, 0);
		}

		private Vector2 ValueSize (int dummy = 0)
		{
			Vector2 size = Info.SizeFunc (ItemNum);
			return new Vector2 (size.X / 2, size.Y);
		}

		private Color DropDownBackgroundColor (MenuItemState itemState)
		{
			return Color.Black; //new Color(0.2f,0.2f,0.2f);
		}

		private Color DropDownForegroundColor (MenuItemState itemState)
		{
			if (itemState == MenuItemState.Selected)
				return Color.White;
			else
				return Color.Gray;
		}
	}

	public class DropDownMenuItem
	{
		public string Text;
		public Action OnSelected;

		public DropDownMenuItem (string text, Action onSelected)
		{
			Text = text;
			OnSelected = onSelected;
		}
	}
}

