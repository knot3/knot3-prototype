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

using Knot3.Core;

namespace Knot3.UserInterface
{
	public class TextInput : Widget, IKeyEventListener
	{
		// text input
		public string InputText = "";
		
		// textures
		private SpriteFont font;

		// textures
		protected SpriteBatch spriteBatch;

		public TextInput (GameScreen screen, WidgetInfo info, DisplayLayer drawOrder)
			: base(screen, info, drawOrder)
		{
			// load fonts
			font = HfGDesign.MenuFont (screen);

			spriteBatch = new SpriteBatch (screen.device);
		}

		public override void Update (GameTime gameTime)
		{

		}

		public override void Draw (GameTime gameTime)
		{
			spriteBatch.Begin ();
			// background
			Rectangle rect = Info.ScaledRectangle (screen.viewport);
			spriteBatch.Draw (
				Textures.Create (screen.device, HfGDesign.LineColor), rect.Grow (1), Color.White
			);
			spriteBatch.Draw (
				Textures.Create (screen.device, Info.BackgroundColor ()), rect, Color.White
			);

			// text
			Vector2 scale =
				(Info.ScaledSize (screen.viewport) - Info.ScaledPadding (screen.viewport) * 2)
				/ font.MeasureString (InputText);
			spriteBatch.DrawString (
				font, InputText, (Info.RelativePosition () + Info.RelativePadding ()).Scale (screen.viewport),
				Info.ForegroundColor (), 0, Vector2.Zero, MathHelper.Min (scale.X, scale.Y),
				SpriteEffects.None, 1f
			);
			spriteBatch.End ();
		}

		public void OnKeyEvent (List<Keys> key, KeyEvent keyEvent, GameTime gameTime)
		{
			Text.TryTextInput (ref InputText, gameTime);
		}

		public List<Keys> ValidKeys { get { return Text.ValidKeys; } }

		public bool IsKeyEventEnabled { get { return IsVisible; } }
	}
}

