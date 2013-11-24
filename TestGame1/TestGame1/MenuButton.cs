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
	public class MenuItemInfo
	{
		// item data
		public string Text;
		
		// state, position and sizes
		public LazyItemPosition PositionFunc = (n) => Vector2.Zero;
		public LazyItemSize SizeFunc = (n) => Vector2.Zero;

		// keys to listen on
		public List<Keys> Keys = new List<Keys> ();
		
		// click action
		public Action OnClick = () => {};

		public MenuItemInfo (string text, LazyItemPosition position, LazyItemSize size, Action onClick)
		{
			Text = text;
			PositionFunc = position;
			SizeFunc = size;
			OnClick = onClick;
		}

		public MenuItemInfo (string text, Vector2 topLeft, Vector2 bottomRight, Action onClick)
		{
			Text = text;
			PositionFunc = (i) => topLeft;
			SizeFunc = (i) => (bottomRight - topLeft);
			OnClick = onClick;
		}

		public MenuItemInfo (string text, float left, float top, float right, float bottom, Action onClick)
		{
			Text = text;
			PositionFunc = (i) => new Vector2 (left, top);
			SizeFunc = (i) => new Vector2 (right-left, bottom-top);
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

	public abstract class MenuItem : GameClass
	{
		// item data
		public MenuItemInfo Info;
		protected int ItemNum;

		// state, position and sizes
		public MenuItemState ItemState;
		protected HAlign AlignX;
		protected MenuItemColor ForegroundColor;
		protected MenuItemColor BackgroundColor;
		private Texture2D texture;

		protected Vector2 Position { get { return Info.PositionFunc (ItemNum).Scale (viewport); } }

		protected Vector2 Size { get { return Info.SizeFunc (ItemNum).Scale (viewport); } }

		/// <summary>
		/// Initializes a new instance of the <see cref="TestGame1.MenuItem"/> class.
		/// </summary>
		public MenuItem (GameState state, int itemNum, MenuItemInfo info,
		                 MenuItemColor fgColor, MenuItemColor bgColor, HAlign alignX)
			: base(state)
		{
			ItemNum = itemNum;
			Info = info;
			AlignX = alignX;
			ForegroundColor = fgColor;
			BackgroundColor = bgColor;
			texture = Textures.Create(device, Color.White);
		}

		public virtual bool Update (GameTime gameTime)
		{
			// keyboard input
			foreach (Keys key in Info.Keys) {
				if (key.IsDown ()) {
					Activate ();
					return true;
				}
			}

			// mouse input
			bool selected = bounds ().Contains (Input.MouseState.ToPoint ());
			ItemState = selected ? MenuItemState.Selected : MenuItemState.Normal;
			if (selected) {
				if (Input.MouseState.IsLeftClick (gameTime)) {
					Activate ();
					return true;
				}
			}
			
			return false;
		}

		public virtual void Draw (float layerDepth, SpriteBatch spriteBatch, SpriteFont font, GameTime gameTime)
		{
			Texture2D paneTexture = Textures.Create (device, Color.White);
			//spriteBatch.Draw (paneTexture, bounds (), Color.Black);
			spriteBatch.Draw (paneTexture, bounds (), null, BackgroundColor (ItemState), 0f,
			                  Vector2.Zero, SpriteEffects.None, layerDepth);

			try {
				Vector2 scale = Size / MinimumSize (font) * 0.9f;
				scale.Y = scale.X = MathHelper.Min (scale.X, scale.Y);
				spriteBatch.DrawString (font, Info.Text, TextPosition (font, scale), ForegroundColor (ItemState),
						0, Vector2.Zero, scale, SpriteEffects.None, layerDepth+0.001f);
			} catch (ArgumentException exp) {
				Console.WriteLine (exp.ToString ());
			} catch (InvalidOperationException exp) {
				Console.WriteLine (exp.ToString ());
			}
		}

		public Vector2 TextPosition (SpriteFont font)
		{
			return TextPosition (font, Vector2.One);
		}

		public Vector2 TextPosition (SpriteFont font, Vector2 scale)
		{
			Vector2 textPosition = Position;
			switch (AlignX) {
			case HAlign.Left:
				textPosition.Y += (Size.Y - MinimumSize (font).Y * scale.Y) / 2;
				//textPosition.X += font.LineSpacing * scale.Y * 0.5f;
				break;
			case HAlign.Center:
				textPosition += (Size - MinimumSize (font) * scale) / 2;
				break;
			case HAlign.Right:
				textPosition.Y += (Size.Y - MinimumSize (font).Y * scale.Y) / 2;
				textPosition.X += Size.X - MinimumSize (font).X * scale.X;
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
			Point topLeft = Position.ToPoint ();
			Point size = Size.ToPoint ();
			return new Rectangle (topLeft.X, topLeft.Y, size.X, size.Y);
		}

		public void Activate ()
		{
			Info.OnClick ();
		}

        public virtual void Collapse()
        {
        }
	}

	public class MenuButton : MenuItem
	{
		public MenuButton (GameState state, int itemNum, MenuItemInfo info,
		                 MenuItemColor fgColor, MenuItemColor bgColor, HAlign alignX)
			: base(state, itemNum, info, fgColor, bgColor, alignX)
		{
		}
	}

	public enum MenuItemState
	{
		Selected,
		Normal
	}

	public enum HAlign
	{
		Left,
		Center,
		Right
	}

	// delegates
	public delegate Vector2 LazyItemSize (int n);
	public delegate Vector2 LazyItemPosition (int n);
	public delegate Vector2 LazySize ();
	public delegate Vector2 LazyPosition ();
	public delegate Color MenuItemColor (MenuItemState itemState);
}

