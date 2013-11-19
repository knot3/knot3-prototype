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
	public class VerticalMenu : Menu
	{
		// menu-related attributes
		private Vector2 Position;
		private Vector2 ItemSize;
		private Vector2 Padding;

		public VerticalMenu (GameState state)
			: base(state)
		{
		}
		
		public override void Initialize (MenuItemColor fgColor, MenuItemColor bgColor,
				HAlign alignX = HAlign.Left)
		{
			base.Initialize (fgColor, bgColor, alignX);

			Position = Vector2.Zero;
			ItemSize = new Vector2 (300, 0);
			Padding = Vector2.Zero;
		}
		
		public override MenuButton AddButton (MenuItemInfo info)
		{
			info.PositionFunc = ItemPosition;
			info.SizeFunc = (int n) => ItemSize;
			return base.AddButton(info);
		}
		

		public override void AddDropDown (MenuItemInfo info, DropDownMenuItem[] items)
		{
			info.PositionFunc = ItemPosition;
			info.SizeFunc = (int n) => ItemSize;
			base.AddDropDown(info, items);
		}

		public override void AddDropDown (MenuItemInfo info, DistinctOptionInfo option)
		{
			info.PositionFunc = ItemPosition;
			info.SizeFunc = (int n) => ItemSize;
			base.AddDropDown(info, option);
		}

		public void Align (Viewport viewport, float scale, Vector2? position = null, Vector2? itemSize = null)
		{
			Padding = Vector2.One * Font.LineSpacing * scale * 0.15f;
			ItemSize = Vector2.Zero;
			foreach (MenuItem item in Items) {
				Vector2 minSize = item.MinimumSize (Font) * scale;
				if (minSize.X > ItemSize.X || ItemSize == Vector2.Zero) {
					ItemSize = minSize;
				}
			}
			// ItemSize += new Vector2 (0, Font.LineSpacing * scale * 0.5f / 1000f);
			if (itemSize.HasValue) {
				if (itemSize.Value.X > 0 && itemSize.Value.Y > 0) {
					ItemSize = itemSize.Value;
				} else {
					ItemSize.X *= itemSize.Value.Y / ItemSize.Y;
					ItemSize.Y = itemSize.Value.Y;
				}
			}
			if (position.HasValue) {
				Position = position.Value;
			} else {
				Position = (Vector2.One * 1000f - size ()) / 2;
			}
			// Console.WriteLine ("viewport=" + viewport.ToVector2 () + ", size=" + size () + " => position=" + Position);
		}

		public void Align (Viewport viewport, float scale, int posX, int posY, int? sizeX = null, int? sizeY = null)
		{
			Vector2? itemSize;
			if (sizeX.HasValue && sizeY.HasValue) 
				itemSize = new Vector2 (sizeX.Value, sizeY.Value);
			else 
				itemSize = null;
			Align (viewport, scale, new Vector2 (posX, posY), itemSize);
		}

		public Vector2 size ()
		{
			return new Vector2 (ItemSize.X, ItemSize.Y * Items.Count + Padding.Y * (Items.Count - 1));
		}

		public Vector2 ItemPosition (int n)
		{
			return Position + new Vector2 (0, (ItemSize.Y + Padding.Y) * n);
		}
	}
}

