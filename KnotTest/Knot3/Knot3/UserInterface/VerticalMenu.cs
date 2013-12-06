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
using Knot3.Utilities;

namespace Knot3.UserInterface
{
	public class VerticalMenu : Menu
	{
		// fonts and colors
		private Vector2 RelativeItemSize;
		protected Border Border;

		// textures
		protected SpriteBatch spriteBatch;

		public VerticalMenu (GameState state, DisplayLayer drawOrder)
			: base(state, drawOrder)
		{
			RelativeSize = () => new Vector2 (
				RelativeItemSize.X,
				RelativeItemSize.Y * Items.Count + RelativePadding ().Y * (Items.Count - 1)
			);
			RelativeItemSize = new Vector2 (300, 0);
			Border = Border.Zero;

			spriteBatch = new SpriteBatch (state.device);
		}

		public void Initialize (LazyItemColor fgColor, LazyItemColor bgColor,
				HAlign alignX, Border border)
		{
			Initialize (fgColor, bgColor, alignX);
			if (border != null) {
				Border = border;
			}
		}

		public override MenuButton AddButton (MenuItemInfo info)
		{
			info.Position = RelativeItemPosition;
			info.Size = (int n) => RelativeItemSize;
			return base.AddButton (info);
		}

		public override void AddDropDown (MenuItemInfo info, DropDownMenuItem[] items, DropDownMenuItem defaultItem)
		{
			info.Position = RelativeItemPosition;
			info.Size = (int n) => RelativeItemSize;
			base.AddDropDown (info, items, defaultItem);
		}

		public override void AddDropDown (MenuItemInfo info, DistinctOptionInfo option)
		{
			info.Position = RelativeItemPosition;
			info.Size = (int n) => RelativeItemSize;
			base.AddDropDown (info, option);
		}

		public void Align (Viewport viewport, float scale, Vector2? position = null, Vector2? itemSize = null,
		                   float padding = 0.15f)
		{
			SpriteFont font = HfGDesign.MenuFont (state);
			RelativePadding = () => Vector2.One * font.LineSpacing * scale * padding;
			RelativeItemSize = Vector2.Zero;
			foreach (MenuItem item in Items) {
				Vector2 minSize = item.MinimumSize (font) * scale;
				if (minSize.X > RelativeItemSize.X || RelativeItemSize == Vector2.Zero) {
					RelativeItemSize = minSize;
				}
			}
			// ItemSize += new Vector2 (0, Font.LineSpacing * scale * 0.5f / 1000f);
			if (itemSize.HasValue) {
				if (itemSize.Value.X > 0 && itemSize.Value.Y > 0) {
					RelativeItemSize = itemSize.Value;
				} else {
					RelativeItemSize.X *= itemSize.Value.Y / RelativeItemSize.Y;
					RelativeItemSize.Y = itemSize.Value.Y;
				}
			}
			if (position.HasValue) {
				RelativePosition = () => position.Value;
			} else {
				RelativePosition = () => (Vector2.One * 1000f - RelativeSize ()) / 2;
			}
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

		public Vector2 RelativeItemPosition (int n)
		{
			return RelativePosition () + new Vector2 (0, (RelativeItemSize.Y + RelativePadding ().Y) * n);
		}

		public override void Draw (GameTime gameTime)
		{
			base.Draw (gameTime);

			if (IsVisible) {
				Point min = ScaledPosition.ToPoint ();
				Point size = ScaledSize.ToPoint ();
				Rectangle[] borders = new Rectangle[]{
					new Rectangle (min.X - (int)Border.Size.X, min.Y - (int)Border.Size.Y,
					               (int)Border.Size.X, size.Y + (int)Border.Size.Y * 2),
					new Rectangle (min.X - (int)Border.Size.X, min.Y - (int)Border.Size.Y,
					               size.X + (int)Border.Size.X * 2, (int)Border.Size.Y),
					new Rectangle (min.X + size.X, min.Y - (int)Border.Size.Y,
				                   (int)Border.Size.X, size.Y + (int)Border.Size.Y * 2),
					new Rectangle (min.X - (int)Border.Size.X, min.Y + size.Y,
				                   size.X + (int)Border.Size.X * 2, (int)Border.Size.Y)
				};
				Texture2D borderTexture = Textures.Create (state.device, Color.White);
			
				spriteBatch.Begin ();
				foreach (Rectangle rect in borders) {
					spriteBatch.Draw (borderTexture, rect, null, Border.Color, 0f,
			                  Vector2.Zero, SpriteEffects.None, 1f);
				}
				spriteBatch.End ();
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

