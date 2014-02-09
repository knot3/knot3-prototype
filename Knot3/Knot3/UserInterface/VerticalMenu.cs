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
		public Border Border { get; set; }
		
		// textures
		protected SpriteBatch spriteBatch;

		public VerticalMenu (GameScreen screen, WidgetInfo info, DisplayLayer drawOrder)
			: base(screen, info, drawOrder)
		{
			info.RelativeSize = () => new Vector2 (
				RelativeItemSize (-1).X,
				RelativeItemSize (-1).Y * Items.Count + Info.RelativePadding ().Y * (Items.Count - 1)
			);
			RelativeItemSize = (i) => new Vector2 (300, 0);
			RelativeItemPosition = (n) => {
				return Info.RelativePosition () + new Vector2 (0, (RelativeItemSize (-1).Y + Info.RelativePadding ().Y) * n);
			};
			Border = Border.Zero;

			spriteBatch = new SpriteBatch (screen.device);
		}

		public override MenuButton AddButton (MenuItemInfo info)
		{
			int num = Items.Count;
			info.RelativePosition = () => RelativeItemPosition (num);
			info.RelativeSize = () => RelativeItemSize (num);
			return base.AddButton (info);
		}

		public override void AddDropDown (MenuItemInfo info, DropDownMenuItem[] items, DropDownMenuItem defaultItem)
		{
			base.AddDropDown (info, items, defaultItem);
		}

		public override void AddDropDown (MenuItemInfo info, DistinctOptionInfo option)
		{
			int num = Items.Count;
			info.RelativePosition = () => RelativeItemPosition (num);
			info.RelativeSize = () => RelativeItemSize (num);
			base.AddDropDown (info, option);
		}

		public void Align (Viewport viewport, float scale, Vector2? givenPosition = null, Vector2? givenItemSize = null,
		                   float padding = 0.15f)
		{
			SpriteFont font = HfGDesign.MenuFont (screen);
			(Info as WidgetInfo).RelativePadding = () => Vector2.One * font.LineSpacing * scale * padding;
			Vector2 bestItemSize = Vector2.Zero;
			foreach (MenuItem item in Items) {
				Vector2 minSize = item.MinimumSize (font) * scale;
				if (minSize.X > bestItemSize.X || bestItemSize == Vector2.Zero) {
					bestItemSize = minSize;
				}
			}
			// ItemSize += new Vector2 (0, Font.LineSpacing * scale * 0.5f / 1000f);
			if (givenItemSize.HasValue) {
				if (givenItemSize.Value.X > 0 && givenItemSize.Value.Y > 0) {
					bestItemSize = givenItemSize.Value;
				} else {
					bestItemSize.X *= givenItemSize.Value.Y / bestItemSize.Y;
					bestItemSize.Y = givenItemSize.Value.Y;
				}
			}
			RelativeItemSize = (num) => bestItemSize;
			if (givenPosition.HasValue) {
				(Info as WidgetInfo).RelativePosition = () => givenPosition.Value;
			} else {
				(Info as WidgetInfo).RelativePosition = () => (Vector2.One * 1000f - Info.RelativeSize ()) / 2;
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

		public override void Draw (GameTime time)
		{
			base.Draw (time);

			if (IsVisible) {
				Point min = Info.ScaledPosition (screen.viewport).ToPoint ();
				Point size = Info.ScaledSize (screen.viewport).ToPoint ();
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
				Texture2D borderTexture = TextureHelper.Create (screen.device, Color.White);
			
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

