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
	public class Menu : Widget
	{
		// fonts and colors
		protected LazyItemColor ItemForegroundColor;
		protected LazyItemColor ItemBackgroundColor;
		protected HAlign ItemAlignX;

		// menu-related attributes
		protected List<MenuItem> Items;

		public Menu (GameState state, DisplayLayer drawOrder, LazyColor foregroundColor = null,
		             LazyColor backgroundColor = null, HAlign alignX = HAlign.Left, VAlign alignY = VAlign.Center)
			: base(state, drawOrder, foregroundColor, backgroundColor, alignX, alignY)
		{
			Items = new List<MenuItem> ();
		}

		public virtual MenuButton AddButton (MenuItemInfo info)
		{
			MenuButton item = new MenuButton (
				state, ItemDisplayLayer, Items.Count, info, ItemForegroundColor, ItemBackgroundColor, ItemAlignX
			);
			Items.Add (item);
			return item;
		}

		public virtual void AddDropDown (MenuItemInfo info, DropDownMenuItem[] items, DropDownMenuItem defaultItem)
		{
			DropDownMenu item = new DropDownMenu (
				state, ItemDisplayLayer, Items.Count, info, ItemForegroundColor, ItemBackgroundColor, ItemAlignX
			);
			item.AddEntries (items, defaultItem);
			Items.Add (item);
		}

		public virtual void AddDropDown (MenuItemInfo info, DistinctOptionInfo option)
		{
			DropDownMenu item = new DropDownMenu (
				state, ItemDisplayLayer, Items.Count, info, ItemForegroundColor, ItemBackgroundColor, ItemAlignX
			);
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
		
		public virtual void Initialize (LazyItemColor itemFgColor, LazyItemColor itemBgColor, HAlign itemAlignX)
		{
			ItemForegroundColor = itemFgColor;
			ItemBackgroundColor = itemBgColor;
			ItemAlignX = itemAlignX;
		}

		public override IEnumerable<GameComponent> SubComponents (GameTime gameTime)
		{
			foreach (GameComponent component in base.SubComponents(gameTime)) {
				yield return component;
			}
			foreach (GameComponent item in Items) {
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

