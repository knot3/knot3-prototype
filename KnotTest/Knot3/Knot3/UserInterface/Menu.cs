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
	public class Menu : Widget
	{
		// size and position
		public Func<int, Vector2> RelativeItemSize = null;
		public Func<int, Vector2> RelativeItemPosition = null;

		// fonts and colors
		public Func<ItemState, Color> ItemForegroundColor = null;
		public Func<ItemState, Color> ItemBackgroundColor = null;
		public HAlign? ItemAlignX = null;
		public VAlign? ItemAlignY = null;

		// menu-related attributes
		protected List<MenuItem> Items;

		public Menu (GameState state, WidgetInfo info, DisplayLayer drawOrder)
			: base(state, info, drawOrder)
		{
			Items = new List<MenuItem> ();
		}

		private void assignMenuItemInfo (ref MenuItemInfo info, int num, MenuItem item)
		{
			if (RelativeItemPosition != null)
				info.RelativePosition = () => RelativeItemPosition (num);
			if (RelativeItemSize != null)
				info.RelativeSize = () => RelativeItemSize (num);
			if (ItemForegroundColor != null)
				info.ForegroundColor = () => ItemForegroundColor (item.ItemState);
			if (ItemBackgroundColor != null)
				info.BackgroundColor = () => ItemBackgroundColor (item.ItemState);
			if (ItemAlignX.HasValue)
				info.AlignX = ItemAlignX.Value;
			if (ItemAlignY.HasValue)
				info.AlignY = ItemAlignY.Value;
		}

		public virtual MenuButton AddButton (MenuItemInfo info)
		{
			int num = Items.Count;
			MenuButton item = new MenuButton (state, ItemDisplayLayer, num, info);
			assignMenuItemInfo (ref info, num, item);
			Items.Add (item);
			return item;
		}

		public virtual void AddDropDown (MenuItemInfo info, DropDownMenuItem[] items, DropDownMenuItem defaultItem)
		{
			int num = Items.Count;
			DropDownMenu item = new DropDownMenu (state, ItemDisplayLayer, num, info);
			assignMenuItemInfo (ref info, num, item);
			item.AddEntries (items, defaultItem);
			Items.Add (item);
		}

		public virtual void AddDropDown (MenuItemInfo info, DistinctOptionInfo option)
		{
			int num = Items.Count;
			DropDownMenu item = new DropDownMenu (state, ItemDisplayLayer, num, info);
			assignMenuItemInfo (ref info, num, item);
			item.AddEntries (option);
			Items.Add (item);
		}

		private DisplayLayer ItemDisplayLayer {
			get {
				if (DrawOrder == (int)DisplayLayer.SubMenu)
					return DisplayLayer.SubMenuItem;
				else
					return DisplayLayer.MenuItem;
			}
		}

		public MenuItem this [int i] {
			get {
				while (i < 0) {
					i += Items.Count;
				}
				return Items [i % Items.Count];
			}
			set {
				while (i < 0) {
					i += Items.Count;
				}
				Items [i % Items.Count] = value;
			}
		}

		public int Count { get { return Items.Count (); } }

		public void Clear ()
		{
			Items.Clear ();
		}

		public override IEnumerable<IGameStateComponent> SubComponents (GameTime gameTime)
		{
			foreach (DrawableGameStateComponent component in base.SubComponents(gameTime)) {
				yield return component;
			}
			foreach (DrawableGameStateComponent item in Items) {
				yield return item;
			}
		}
       
		public void CollapseMenus (MenuItem menu)
		{
			foreach (MenuItem item in Items) {
				if (item != menu) {
					item.Collapse ();
				}
			}
		}

		private bool isVisible;

		public override bool IsVisible {
			get {
				return isVisible;
			}
			set {
				isVisible = value;
				if (Items != null) {
					foreach (MenuItem item in Items) {
						item.IsVisible = value;
					}
				}
			}
		}
	}
}

