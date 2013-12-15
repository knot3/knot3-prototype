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

		public DropDownMenu (GameScreen screen, DisplayLayer drawOrder, int itemNum, MenuItemInfo info)
			: base(screen, drawOrder, itemNum, info)
		{
			// drop-down menu
			dropdown = new VerticalMenu (screen, new WidgetInfo (), DisplayLayer.SubMenu);
			
			dropdown.ItemForegroundColor = DropDownForegroundColor;
			dropdown.ItemBackgroundColor = DropDownBackgroundColor;
			dropdown.ItemAlignX = HAlign.Left;
			dropdown.ItemAlignY = VAlign.Center;
			dropdown.Border = new Border (new Color (0xb4, 0xff, 0x00), 5, 5, 0, 0);
			dropdown.IsVisible = false;

			// selected value
			MenuItemInfo valueInfo = new MenuItemInfo () {
				Text = "---",
				RelativePosition = () => ValuePosition (0),
				RelativeSize = () => ValueSize (0),
				OnClick = () => info.OnClick (),
			};
			selected = new MenuButton (screen, DisplayLayer.MenuItem, 0, valueInfo);
			selected.Info.ForegroundColor = () => DropDownForegroundColor (selected.ItemState);
			selected.Info.BackgroundColor = () => DropDownBackgroundColor (selected.ItemState);

			// action to open the drop-down menu
			info.OnClick = () =>
			{
				GameScreens.VideoOptionScreen.Collapse (this);
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
				dropdown.AddButton (new MenuItemInfo (text: entry.Text, onClick: onSelected));
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
				dropdown.AddButton (new MenuItemInfo (text: value, onClick: onSelected));
			}
			selected.Info.Text = option.Value;
		}
		
		public override IEnumerable<IGameScreenComponent> SubComponents (GameTime time)
		{
			foreach (DrawableGameScreenComponent component in base.SubComponents(time)) {
				yield return component;
			}
			yield return selected;
			yield return dropdown;
		}

		public override void Collapse ()
		{
			dropdown.IsVisible = false;
		}

		public override void Draw (GameTime time)
		{
			base.Draw (time);

			if (IsVisible && dropdown.IsVisible) {
				// draw dropdown menu
				Vector2 position = ValuePosition ();
				Vector2 size = ValueSize ();
				dropdown.Align (screen.viewport, 1f, (int)position.X, (int)position.Y, (int)size.X, (int)size.Y, 0f);
			}
		}

		private Vector2 ValuePosition (int dummy = 0)
		{
			Vector2 size = Info.RelativeSize ();
			return Info.RelativePosition () + new Vector2 (size.X / 2, 0);
		}

		private Vector2 ValueSize (int dummy = 0)
		{
			Vector2 size = Info.RelativeSize ();
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

