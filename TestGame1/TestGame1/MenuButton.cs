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
	public enum MenuItemState
	{
		Selected,
		Normal
	}

	public enum HorizontalAlignment
	{
		Left,
		Center,
		Right
	}

	// delegates
	public delegate Vector2 LazyVector2 (int n);

	public class MenuItem : GameClass
	{
		// item data
		private int ItemNum;
		private string Text;

		// state, position and sizes
		public MenuItemState ItemState;
		private LazyVector2 PositionFunc;
		private LazyVector2 SizeFunc;
		private HorizontalAlignment AlignX;

		private Vector2 Position {
			get { return PositionFunc (ItemNum); }
		}

		private Vector2 Size {
			get { return SizeFunc (ItemNum); }
		}

		// keys to listen on
		public List<Keys> Keys = new List<Keys> ();

		// click action
		public Action OnClick = () => {};

		/// <summary>
		/// Initializes a new instance of the <see cref="TestGame1.MenuItem"/> class.
		/// </summary>
		public MenuItem (GameState state, string text, int itemNum, LazyVector2 positionFunc,
		                 LazyVector2 sizeFunc, HorizontalAlignment alignX)
			: base(state)
		{
			Text = text;
			PositionFunc = positionFunc;
			SizeFunc = sizeFunc;
			ItemNum = itemNum;
			AlignX = alignX;
		}

		public void Update (GameTime gameTime)
		{
			foreach (Keys key in Keys) {
				if (key.IsDown ()) {
					Activate ();
				}
			}
		}

		public void Draw (SpriteBatch spriteBatch, SpriteFont font, GameTime gameTime)
		{
			Texture2D paneTexture = Textures.Create (device, Color.White);
			spriteBatch.Draw (paneTexture, bounds (), BackgroundColor ());

			try {
				//spriteBatch.DrawString (font, Text, TextPosition (font), ForegroundColor ());

				Vector2 scale = Size / MinimumSize (font) * 0.9f;
				scale.Y = scale.X = MathHelper.Min(scale.X, scale.Y);
				spriteBatch.DrawString (font, Text, TextPosition (font, scale), ForegroundColor (),
				                        0, Vector2.Zero, scale, SpriteEffects.None, 0);

			} catch (ArgumentException exp) {
				Console.WriteLine (exp.ToString ());
			} catch (InvalidOperationException exp) {
				Console.WriteLine (exp.ToString ());
			}
		}

		public Color BackgroundColor ()
		{
			switch (ItemState) {
			case MenuItemState.Selected:
				return Color.White * 0.0f;
			case MenuItemState.Normal:
			default:
				return Color.White * 0.0f;
			}
		}

		public Color ForegroundColor ()
		{
			switch (ItemState) {
			case MenuItemState.Selected:
				return Color.White * 0.5f;
			case MenuItemState.Normal:
			default:
				return Color.White * 1.0f;
			}
		}

		public Vector2 TextPosition (SpriteFont font)
		{
			return TextPosition(font, new Vector2(1,1));
		}

		public Vector2 TextPosition (SpriteFont font, Vector2 scale)
		{
			Vector2 textPosition = Position;
			switch (AlignX) {
			case HorizontalAlignment.Left:
				textPosition.Y += (Size.Y - MinimumSize (font).Y * scale.Y) / 2;
				textPosition.X += font.LineSpacing / 2;
				break;
			case HorizontalAlignment.Center:
				textPosition += (Size - MinimumSize (font) * scale) / 2;
				break;
			case HorizontalAlignment.Right:
				textPosition.Y += (Size.Y - MinimumSize (font).Y * scale.Y) / 2;
				textPosition.X += Size.X - font.LineSpacing / 2;
				break;
			}
			return textPosition;
		}

		public Vector2 MinimumSize (SpriteFont font)
		{
			return font.MeasureString (Text);
		}

		public Rectangle bounds ()
		{
			Point topLeft = Position.ToPoint ();
			Point size = Size.ToPoint ();
			return new Rectangle (topLeft.X, topLeft.Y, size.X, size.Y);
		}

		public void Activate ()
		{
			OnClick ();
		}
	}
}

