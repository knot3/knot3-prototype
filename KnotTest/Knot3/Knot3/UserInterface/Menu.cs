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
		public SpriteFont Font { get; protected set; }

		protected LazyItemColor ItemForegroundColor;
		protected LazyItemColor ItemBackgroundColor;
		protected HAlign ItemAlignX;

		// menu-related attributes
		protected List<MenuItem> Items;

		public Menu (GameState state, LazyColor foregroundColor = null, LazyColor backgroundColor = null,
		             HAlign alignX = HAlign.Left, VAlign alignY = VAlign.Center)
			: base(state, foregroundColor, backgroundColor, alignX, alignY)
		{
			Items = new List<MenuItem> ();
		}

		public virtual MenuButton AddButton (MenuItemInfo info)
		{
			MenuButton item = new MenuButton (state, Items.Count, info, ItemForegroundColor,
			                                  ItemBackgroundColor, ItemAlignX);
			Items.Add (item);
			return item;
		}

		public virtual void AddDropDown (MenuItemInfo info, DropDownMenuItem[] items, DropDownMenuItem defaultItem)
		{
			DropDownMenu item = new DropDownMenu (state, Items.Count, info, ItemForegroundColor,
			                                      ItemBackgroundColor, ItemAlignX);
			item.AddEntries (items, defaultItem);
			Items.Add (item);
		}

		public virtual void AddDropDown (MenuItemInfo info, DistinctOptionInfo option)
		{
			DropDownMenu item = new DropDownMenu (state, Items.Count, info, ItemForegroundColor,
			                                      ItemBackgroundColor, ItemAlignX);
			item.AddEntries (option);
			Items.Add (item);
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

			// load fonts
			try {
				Font = content.Load<SpriteFont> ("MenuFont");
			} catch (ContentLoadException ex) {
				Font = null;
				Console.WriteLine (ex.Message);
			}
		}

		public bool Update (GameTime gameTime)
		{
			foreach (MenuItem item in Items) {
				if (item.Update (gameTime)) {
					return true;
				}
			}
			return false;
		}
       
		public void CollapseMenus (MenuItem menu)
		{
			foreach (MenuItem item in Items) {
				if (item != menu) {
					item.Collapse ();
				}
			}
		}

		public virtual void Draw (float layerDepth, SpriteBatch spriteBatch, GameTime gameTime)
		{
			foreach (MenuItem item in Items) {
				item.Draw (layerDepth, spriteBatch, Font, gameTime);
			}
		}
	}
}

