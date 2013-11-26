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

using Knot3.Utilities;

namespace Knot3.UserInterface
{
	public class MenuItemInfo
	{
		// item data
		public string Text;
		
		// state, position and sizes
		public LazyItemPosition Position = (i) => Vector2.Zero;
		public LazyItemSize Size = (i) => Vector2.Zero;

		// keys to listen on
		public List<Keys> Keys = new List<Keys> ();
		
		// click action
		public Action OnClick = () => {};

		public MenuItemInfo (string text, LazyItemPosition position, LazyItemSize size, Action onClick)
		{
			Text = text;
			Position = position;
			Size = size;
			OnClick = onClick;
		}

		public MenuItemInfo (string text, Vector2 topLeft, Vector2 bottomRight, Action onClick)
		{
			Text = text;
			Position = (i) => topLeft;
			Size = (i) => (bottomRight - topLeft);
			OnClick = onClick;
		}

		public MenuItemInfo (string text, float left, float top, float right, float bottom, Action onClick)
		{
			Text = text;
			Position = (i) => new Vector2 (left, top);
			Size = (i) => new Vector2 (right - left, bottom - top);
			OnClick = onClick;
		}

		public MenuItemInfo (string text, Action onClick)
		{
			Text = text;
			OnClick = onClick;
		}

		public MenuItemInfo (string text)
		{
			Text = text;
		}

		public MenuItemInfo AddKey (Keys key)
		{
			Keys.Add (key);
			return this;
		}
	}

	public abstract class MenuItem : ItemWidget, IMouseEvent, IKeyEvent
	{
		// item data
		public MenuItemInfo Info;

		// textures
		protected SpriteBatch spriteBatch;

		// state, position and sizes
		public override LazyPosition RelativePosition { get { return () => Info.Position (ItemNum); } }

		public override LazySize RelativeSize { get { return () => Info.Size (ItemNum); } }

		/// <summary>
		/// Initializes a new instance of the <see cref="TestGame1.MenuItem"/> class.
		/// </summary>
		public MenuItem (GameState state, DisplayLayer drawOrder, int itemNum, MenuItemInfo info,
		                 LazyItemColor fgColor, LazyItemColor bgColor, HAlign alignX)
			: base(state, drawOrder, itemNum, fgColor, bgColor, alignX, VAlign.Center)
		{
			Info = info;

			spriteBatch = new SpriteBatch (device);
		}

		public override void Draw (GameTime gameTime)
		{
			base.Draw (gameTime);

			if (IsVisible) {
				spriteBatch.Begin ();
				Texture2D paneTexture = Textures.Create (device, Color.White);
				//spriteBatch.Draw (paneTexture, bounds (), Color.Black);
				spriteBatch.Draw (paneTexture, bounds (), null, BackgroundColor, 0f,
			                  Vector2.Zero, SpriteEffects.None, 0.5f);

				SpriteFont font = HfGDesign.MenuFont (state);
				try {
					Vector2 scale = ScaledSize / MinimumSize (font) * 0.9f;
					scale.Y = scale.X = MathHelper.Min (scale.X, scale.Y);
					spriteBatch.DrawString (font, Info.Text, TextPosition (font, scale), ForegroundColor,
						0, Vector2.Zero, scale, SpriteEffects.None, 0.6f);
				} catch (ArgumentException exp) {
					Console.WriteLine (exp.ToString ());
				} catch (InvalidOperationException exp) {
					Console.WriteLine (exp.ToString ());
				}
				spriteBatch.End ();
			}
		}

		public Vector2 TextPosition (SpriteFont font)
		{
			return TextPosition (font, Vector2.One);
		}

		public Vector2 TextPosition (SpriteFont font, Vector2 scale)
		{
			Vector2 textPosition = ScaledPosition;
			switch (AlignX) {
			case HAlign.Left:
				textPosition.Y += (ScaledSize.Y - MinimumSize (font).Y * scale.Y) / 2;
				//textPosition.X += font.LineSpacing * scale.Y * 0.5f;
				break;
			case HAlign.Center:
				textPosition += (ScaledSize - MinimumSize (font) * scale) / 2;
				break;
			case HAlign.Right:
				textPosition.Y += (ScaledSize.Y - MinimumSize (font).Y * scale.Y) / 2;
				textPosition.X += ScaledSize.X - MinimumSize (font).X * scale.X;
				break;
			}
			return textPosition;
		}

		public Vector2 MinimumSize (SpriteFont font)
		{
			return font.MeasureString (Info.Text);
		}

		public Rectangle bounds ()
		{
			Point topLeft = ScaledPosition.ToPoint ();
			Point size = ScaledSize.ToPoint ();
			return new Rectangle (topLeft.X, topLeft.Y, size.X, size.Y);
		}

		public List<Keys> Keys { get { return Info.Keys; } }

		public void Activate ()
		{
			Info.OnClick ();
		}

		public void SetHovered (bool hovered)
		{
			ItemState = hovered ? ItemState.Selected : ItemState.Unselected;
		}

		public virtual void Collapse ()
		{
		}
	}
}

