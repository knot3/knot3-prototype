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

		public TextInput (GameScreen state, WidgetInfo info, DisplayLayer drawOrder)
			: base(state, info, drawOrder)
		{
			// load fonts
			font = HfGDesign.MenuFont (state);

			spriteBatch = new SpriteBatch (state.device);
		}

		public override void Update (GameTime gameTime)
		{

		}

		public override void Draw (GameTime gameTime)
		{
			spriteBatch.Begin ();
			// background
			Rectangle rect = Info.ScaledRectangle (state.viewport);
			spriteBatch.Draw (
				Textures.Create (state.device, HfGDesign.LineColor), rect.Grow (1), Color.White
			);
			spriteBatch.Draw (
				Textures.Create (state.device, Info.BackgroundColor ()), rect, Color.White
			);

			// text
			Vector2 scale =
				(Info.ScaledSize (state.viewport) - Info.ScaledPadding (state.viewport) * 2)
				/ font.MeasureString (InputText);
			spriteBatch.DrawString (
				font, InputText, (Info.RelativePosition () + Info.RelativePadding ()).Scale (state.viewport),
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

