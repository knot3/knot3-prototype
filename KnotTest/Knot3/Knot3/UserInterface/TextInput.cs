using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Knot3.UserInterface;
using Knot3.KnotData;
using Knot3.RenderEffects;
using Knot3.GameObjects;
using Knot3.Settings;
using Knot3.Utilities;

namespace Knot3.UserInterface
{
	public class TextInput : Widget
	{
		// text input
		public string InputText = "";
		
		// textures
		private SpriteFont font;

		public TextInput (GameState state, LazyPosition position, LazySize size, LazySize padding,
		                  LazyColor fgColor, LazyColor bgColor)
			: base(state, fgColor, bgColor, HAlign.Left, VAlign.Center)
		{
			RelativePosition = position;
			RelativeSize = size;
			RelativePadding = padding;

			// load fonts
			try {
				font = content.Load<SpriteFont> ("MenuFont");
			} catch (ContentLoadException ex) {
				font = null;
				Console.WriteLine (ex.Message);
			}
		}

		public bool Update (GameTime gameTime)
		{
			return UpdateText (gameTime);
		}

		private bool UpdateText (GameTime gameTime)
		{
			return Text.TryTextInput(ref InputText, gameTime);
		}

		public void Draw (SpriteBatch spriteBatch, GameTime gameTime)
		{
			// background
			Rectangle rect = HfGDesign.CreateRectangle (0, ScaledPosition, ScaledSize);
			spriteBatch.Draw (Textures.Create (device, HfGDesign.LineColor),
				                 rect.Grow (1),
				                 Color.White);
			spriteBatch.Draw (Textures.Create (device, BackgroundColor),
				                 rect,
				                 Color.White);

			// text
			Vector2 scale = (ScaledSize - ScaledPadding * 2) / font.MeasureString (InputText);
			spriteBatch.DrawString (font, InputText, (RelativePosition () + RelativePadding ()).Scale (viewport),
			                        ForegroundColor, 0, Vector2.Zero, MathHelper.Min (scale.X, scale.Y),
			                        SpriteEffects.None, 1f);
		}
	}
}

