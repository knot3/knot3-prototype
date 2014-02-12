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
using Knot3.Utilities;

namespace Knot3.UserInterface
{
	public class MenuItemInfo : WidgetInfo
	{
		// item data
		public string Text = "";

		// keys to listen on
		public List<Keys> Keys = new List<Keys> ();

		// click action
		public Action OnClick = () => {};

		public MenuItemInfo (string text, Vector2 topLeft, Vector2 bottomRight, Action onClick)
		: this(text, onClick)
		{
			RelativePosition = () => topLeft;
			RelativeSize = () => (bottomRight - topLeft);
		}

		public MenuItemInfo (string text, float left, float top, float right, float bottom, Action onClick)
		: this(text, onClick)
		{
			RelativePosition = () => new Vector2 (left, top);
			RelativeSize = () => new Vector2 (right - left, bottom - top);
		}

		public MenuItemInfo (string text, Action onClick)
		: this()
		{
			Text = text;
			OnClick = onClick;
		}

		public MenuItemInfo (string text)
		: this()
		{
			Text = text;
		}

		public MenuItemInfo ()
		: base()
		{
		}

		public MenuItemInfo AddKey (Keys key)
		{
			Keys.Add (key);
			return this;
		}
	}

	public abstract class MenuItem : ItemWidget, IMouseEventListener, IKeyEventListener
	{
		// info
		public new MenuItemInfo Info
		{
			get { return base.Info as MenuItemInfo; }
			set { base.Info = value; }
		}

		// textures
		protected SpriteBatch spriteBatch;

		/// <summary>
		/// Initializes a new instance of the <see cref="TestGame1.MenuItem"/> class.
		/// </summary>
		public MenuItem (GameScreen screen, DisplayLayer drawOrder, int itemNum, MenuItemInfo info)
		: base(screen, info, drawOrder, itemNum)
		{
			// create a sprite batch
			spriteBatch = new SpriteBatch (screen.device);
		}

		public override void Draw (GameTime time)
		{
			base.Draw (time);

			if (IsVisible) {
				spriteBatch.Begin ();
				Texture2D paneTexture = TextureHelper.Create (screen.device, Color.White);
				//spriteBatch.Draw (paneTexture, bounds (), Color.Black);
				spriteBatch.Draw (
				    paneTexture, bounds (), null, Info.BackgroundColor (), 0f, Vector2.Zero, SpriteEffects.None, 0.5f
				);

				SpriteFont font = HfGDesign.MenuFont (screen);
				try {
					Vector2 scale = Info.ScaledSize (screen.viewport) / MinimumSize (font) * 0.9f;
					//Vector2 scale = Info.ScaledSize / MinimumSize (font) * 0.9f;
					scale.Y = scale.X = MathHelper.Min (scale.X, scale.Y);
					spriteBatch.DrawString (font, Info.Text, TextPosition (font, scale), Info.ForegroundColor (),
					                        0, Vector2.Zero, scale, SpriteEffects.None, 0.6f);
				}
				catch (ArgumentException exp) {
					Console.WriteLine (exp.ToString ());
				}
				catch (InvalidOperationException exp) {
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
			Vector2 position = Info.ScaledPosition (screen.viewport);
			Vector2 size = Info.ScaledSize (screen.viewport);
			Vector2 minimumSize = MinimumSize (font);
			switch ((Info as WidgetInfo).AlignX) {
			case HorizontalAlignment.Left:
				position.Y += (size.Y - minimumSize.Y * scale.Y) / 2;
				//textPosition.X += font.LineSpacing * scale.Y * 0.5f;
				break;
			case HorizontalAlignment.Center:
				position += (size - minimumSize * scale) / 2;
				break;
			case HorizontalAlignment.Right:
				position.Y += (size.Y - minimumSize.Y * scale.Y) / 2;
				position.X += size.X - minimumSize.X * scale.X;
				break;
			}
			return position;
		}

		public Vector2 MinimumSize (SpriteFont font)
		{
			return font.MeasureString (Info.Text);
		}

		public List<Keys> ValidKeys { get { return Info.Keys; } }

		public void OnLeftClick (Vector2 position, ClickState click, GameTime time)
		{
			Info.OnClick ();
		}

		public void OnRightClick (Vector2 position, ClickState click, GameTime time)
		{
		}

		public void OnKeyEvent (List<Keys> key, KeyEvent keyEvent, GameTime time)
		{
			if (keyEvent == KeyEvent.KeyDown) {
				Info.OnClick ();
			}
		}

		public void SetHovered (bool hovered)
		{
			ItemState = hovered ? ItemState.Selected : ItemState.Unselected;
		}

		public bool IsKeyEventEnabled { get { return IsVisible; } }

		public bool IsMouseEventEnabled { get { return IsVisible; } }

		public virtual void Collapse ()
		{
		}
	}
}
