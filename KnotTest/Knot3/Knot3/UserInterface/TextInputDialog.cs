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
	public class TextInputDialog : ConfirmDialog
	{
		// text input
		protected TextInput TextInput;

		public TextInputDialog (GameState state, DisplayLayer drawOrder)
			: base(state, drawOrder)
		{
			TextInput = new TextInput (
				state, DisplayLayer.SubMenu, TextInputPosition, TextInputSize, () => new Vector2 (0.005f, 0.005f),
				() => Color.Black, () => Color.White
			);
		}

		public override void Activate (GameTime gameTime) {
			state.AddGameComponents(TextInput);
		}

		public override void Deactivate (GameTime gameTime) {
			state.RemoveGameComponents(TextInput);
		}

		protected Vector2 TextInputPosition ()
		{
			Vector2 buttonPosition = RelativeButtonPosition (0);
			Vector2 textInputSize = TextInputSize ();
			return new Vector2 (
				buttonPosition.X,
				buttonPosition.Y - textInputSize.Y - RelativePadding ().Y
			);
		}

		protected Vector2 TextInputSize ()
		{
			float x = (RelativeSize ().X - RelativePadding ().X * 2);
			return new Vector2 (x, 0.06f);
		}
	}
}

