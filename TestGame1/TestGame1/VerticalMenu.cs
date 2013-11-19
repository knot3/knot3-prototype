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
		// fonts and colors
		private Vector2 Position;
		private Vector2 ItemSize;
		private Vector2 Padding;
		protected Border Border;

		public VerticalMenu (GameState state)
			: base(state)
		{
		}
		
		public override void Initialize (MenuItemColor fgColor, MenuItemColor bgColor,
				HAlign alignX)
		{
			base.Initialize (fgColor, bgColor, alignX);
			Position = Vector2.Zero;
			ItemSize = new Vector2 (300, 0);
			Padding = Vector2.Zero;
			Border = Border.Zero;
		}

		public void Initialize (MenuItemColor fgColor, MenuItemColor bgColor,
				HAlign alignX, Border border)
		{
			Initialize (fgColor, bgColor, alignX);
			if (border != null) {
				Border = border;
			}
		}

		public override MenuButton AddButton (MenuItemInfo info)
		{
			info.PositionFunc = ItemPosition;
			info.SizeFunc = (int n) => ItemSize;
			return base.AddButton (info);
		}

		public override void AddDropDown (MenuItemInfo info, DropDownMenuItem[] items)
		{
			info.PositionFunc = ItemPosition;
			info.SizeFunc = (int n) => ItemSize;
			base.AddDropDown (info, items);
		}

		public override void AddDropDown (MenuItemInfo info, DistinctOptionInfo option)
		{
			info.PositionFunc = ItemPosition;
			info.SizeFunc = (int n) => ItemSize;
			base.AddDropDown (info, option);
		}

		public void Align (Viewport viewport, float scale, Vector2? position = null, Vector2? itemSize = null,
		                   float padding = 0.15f)
		{
			Padding = Vector2.One * Font.LineSpacing * scale * padding;
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
				Position = (Vector2.One * 1000f - Size ()) / 2;
			}
			// Console.WriteLine ("viewport=" + viewport.ToVector2 () + ", size=" + size () + " => position=" + Position);
		}

		public void Align (Viewport viewport, float scale, int posX, int posY,
		                   int? sizeX = null, int? sizeY = null, float padding = 0.15f)
		{
			Vector2? itemSize;
			if (sizeX.HasValue && sizeY.HasValue) 
				itemSize = new Vector2 (sizeX.Value, sizeY.Value);
			else 
				itemSize = null;
			Align (viewport, scale, new Vector2 (posX, posY), itemSize, padding);
		}

		public Vector2 Size ()
		{
			return new Vector2 (ItemSize.X, ItemSize.Y * Items.Count + Padding.Y * (Items.Count - 1));
		}

		public Vector2 ItemPosition (int n)
		{
			return Position + new Vector2 (0, (ItemSize.Y + Padding.Y) * n);
		}

		public override void Draw (float layerDepth, SpriteBatch spriteBatch, GameTime gameTime)
		{
			base.Draw (layerDepth, spriteBatch, gameTime);

			Point min = Position.ToPoint ();
			Point size = Size ().ToPoint ();
			Rectangle[] borders = new Rectangle[]{
					new Rectangle (min.X - (int)Border.Size.X, min.Y - (int)Border.Size.Y,
					               (int)Border.Size.X, size.Y + (int)Border.Size.Y * 2),
					new Rectangle (min.X - (int)Border.Size.X, min.Y - (int)Border.Size.Y,
					               size.X + (int)Border.Size.X * 2, (int)Border.Size.Y),
					new Rectangle (min.X + size.X, min.Y- (int)Border.Size.Y,
				                   (int)Border.Size.X, size.Y + (int)Border.Size.Y * 2),
					new Rectangle (min.X- (int)Border.Size.X, min.Y + size.Y,
				                   size.X + (int)Border.Size.X * 2, (int)Border.Size.Y)
				};
			Texture2D borderTexture = Textures.Create (device, Color.White);
			foreach (Rectangle rect in borders) {
				spriteBatch.Draw (borderTexture, rect.Scale (viewport), null, Border.Color, 0f,
			                  Vector2.Zero, SpriteEffects.None, layerDepth);
			}
		}
	}
	
	public class Border
	{
		public Color Color;
		public Vector2 Size;
		public Vector2 Padding;

		public Border (Color color, Vector2 size, Vector2 padding)
		{
			Color = color;
			Size = size;
			Padding = padding;
		}

		public Border (Color color, int sizeX, int sizeY, int paddingX, int paddingY)
		{
			Color = color;
			Size = new Vector2 (sizeX, sizeY);
			Padding = new Vector2 (paddingX, paddingY);
		}

		public static Border Zero { get { return new Border (Color.White * 0f, Vector2.Zero, Vector2.Zero); } }
	}
}

