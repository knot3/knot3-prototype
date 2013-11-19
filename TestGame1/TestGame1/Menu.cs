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

namespace TestGame1
{
	public class Menu : GameClass
	{
		// fonts and colors
		public SpriteFont Font { get; protected set; }
		protected MenuItemColor ForegroundColor;
		protected MenuItemColor BackgroundColor;
		protected HAlign AlignX;

		// menu-related attributes
		protected List<MenuItem> Items;

		public Menu (GameState state)
			: base(state)
		{
			Items = new List<MenuItem> ();
		}

		public virtual MenuButton AddButton (MenuItemInfo info)
		{
			MenuButton item = new MenuButton (state, Items.Count, info, ForegroundColor, BackgroundColor, AlignX);
			Items.Add (item);
			return item;
		}

		public virtual void AddDropDown (MenuItemInfo info, DropDownMenuItem[] items)
		{
			DropDownMenu item = new DropDownMenu (state, Items.Count, info, ForegroundColor, BackgroundColor, AlignX);
			item.AddEntries (items);
			Items.Add (item);
		}

		public virtual void AddDropDown (MenuItemInfo info, DistinctOptionInfo option)
		{
			DropDownMenu item = new DropDownMenu (state, Items.Count, info, ForegroundColor, BackgroundColor, AlignX);
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
		
		public virtual void Initialize (MenuItemColor fgColor, MenuItemColor bgColor, HAlign alignX)
		{
			ForegroundColor = fgColor;
			BackgroundColor = bgColor;
			AlignX = alignX;

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

		public virtual void Draw (float layerDepth, SpriteBatch spriteBatch, GameTime gameTime)
		{
			foreach (MenuItem item in Items) {
				item.Draw (layerDepth, spriteBatch, Font, gameTime);
			}
		}
	}
}

