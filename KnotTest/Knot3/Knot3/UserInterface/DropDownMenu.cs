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
using Knot3.Settings;

namespace Knot3.UserInterface
{
	public class DropDownMenu : MenuItem
	{
		// drop-down menu
		private VerticalMenu dropdown;
		private MenuButton selected;

		public DropDownMenu (GameState state, DisplayLayer drawOrder, int itemNum, MenuItemInfo info,
		                 LazyItemColor fgColor, LazyItemColor bgColor, HAlign alignX)
			: base(state, drawOrder, itemNum, info, fgColor, bgColor, alignX)
		{
			// drop-down menu
			dropdown = new VerticalMenu (state, DisplayLayer.SubMenu);
			dropdown.Initialize (DropDownForegroundColor, DropDownBackgroundColor,
			                     HAlign.Left, new Border (new Color (0xb4, 0xff, 0x00), 5, 5, 0, 0));
			dropdown.IsVisible = false;

			// selected value
			MenuItemInfo valueInfo = new MenuItemInfo (text: "---", position: ValuePosition,
						size: ValueSize, onClick: () => info.OnClick ());
			selected = new MenuButton (state, DisplayLayer.MenuItem, 0, valueInfo, fgColor, bgColor, HAlign.Left);

			// action to open the drop-down menu
			info.OnClick = () =>
			{
				GameStates.VideoOptionScreen.Collapse (this);
				if (dropdown.IsVisible == true) {
					dropdown.IsVisible = false;
				} else {
					dropdown.IsVisible = true;
				}
			};
		}

		public void AddEntries (DropDownMenuItem[] entries, DropDownMenuItem defaultEntry)
		{
			foreach (DropDownMenuItem entry in entries) {
				Action onSelected = entry.OnSelected;
				onSelected += () => dropdown.IsVisible = false;
				dropdown.AddButton (new MenuItemInfo (entry.Text, onSelected));
			}
			selected.Info.Text = defaultEntry.Text;
		}

		public void AddEntries (DistinctOptionInfo option)
		{
			foreach (string _value in option.ValidValues) {
				string value = _value; // create a copy for the action
				Action onSelected = () => {
					Console.WriteLine ("OnClick: " + value);
					option.Value = value;
					selected.Info.Text = value;
					dropdown.IsVisible = false;
				};
				dropdown.AddButton (new MenuItemInfo (value, onSelected));
			}
			selected.Info.Text = option.Value;
		}
		
		public override IEnumerable<IGameStateComponent> SubComponents (GameTime gameTime)
		{
			foreach (DrawableGameStateComponent component in base.SubComponents(gameTime)) {
				yield return component;
			}
			yield return selected;
			yield return dropdown;
		}

		public override void Collapse ()
		{
			dropdown.IsVisible = false;
		}

		public override void Draw (GameTime gameTime)
		{
			base.Draw (gameTime);

			if (IsVisible && dropdown.IsVisible) {
				// draw dropdown menu
				Vector2 position = ValuePosition ();
				Vector2 size = ValueSize ();
				dropdown.Align (state.viewport, 1f, (int)position.X, (int)position.Y, (int)size.X, (int)size.Y, 0f);
			}
		}

		private Vector2 ValuePosition (int dummy = 0)
		{
			Vector2 size = Info.Size (ItemNum);
			return Info.Position (ItemNum) + new Vector2 (size.X / 2, 0);
		}

		private Vector2 ValueSize (int dummy = 0)
		{
			Vector2 size = Info.Size (ItemNum);
			return new Vector2 (size.X / 2, size.Y);
		}

		private Color DropDownBackgroundColor (ItemState itemState)
		{
			return Color.Black; //new Color(0.2f,0.2f,0.2f);
		}

		private Color DropDownForegroundColor (ItemState itemState)
		{
			if (itemState == ItemState.Selected)
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

