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

using Knot3.UserInterface;
using Knot3.Utilities;

namespace Knot3.Core
{
	public static class InputHelper
	{
		/// <summary>
		/// Wurde die aktuelle Taste gedrückt und war sie im letzten Frame nicht gedrückt?
		/// </summary>
		/// <returns>
		/// <c>true</c> if the specified key is down; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='key'>
		/// If set to <c>true</c> key.
		/// </param>
		public static bool IsDown (this Keys key)
		{
			// Is the key down?
			if (InputManager.CurrentKeyboardState.IsKeyDown (key)) {
				// If not down last update, key has just been pressed.
				if (!InputManager.PreviousKeyboardState.IsKeyDown (key)) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Wird die aktuelle Taste gedrückt gehalten?
		/// </summary>
		/// <returns>
		/// <c>true</c> if the specified key is held down; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='key'>
		/// If set to <c>true</c> key.
		/// </param>
		public static bool IsHeldDown (this Keys key)
		{
			// Is the key down?
			return InputManager.CurrentKeyboardState.IsKeyDown (key);
		}
	}
}
