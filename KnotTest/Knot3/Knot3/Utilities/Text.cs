using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Knot3.Utilities
{
	public static class Text
	{
		private static Keys lastKey = Keys.None;
		private static double lastMillis = 0;

		public static bool TryTextInput (ref string str, GameTime gameTime)
		{
			bool catched = false;
			if (lastKey != Keys.None) {
				if (Input.KeyboardState.IsKeyUp (lastKey))
					lastKey = Keys.None;
				else if ((gameTime.TotalGameTime.TotalMilliseconds - lastMillis) > 200)
					lastKey = Keys.None;
			}
			Keys[] keys = Input.KeyboardState.GetPressedKeys ();
			if (lastKey == Keys.None) {
				for (int i = 0; i < keys.Length; ++i) {
					if (keys [i] != Keys.LeftShift && keys [i] != Keys.RightShift) {
						lastKey = keys [i];
					}
				}
				if (lastKey != Keys.None) {
					if (lastKey == Keys.Back) {
						if (str.Length != 0)
							str = str.Substring (0, str.Length - 1);
						catched = true;
					} else if (str.Length < 100) {
						char c;
						if (TryConvertKey (lastKey, out c)) {
							str += c;
						}
						catched = true;
					}
				}

				lastMillis = gameTime.TotalGameTime.TotalMilliseconds;
			}
			return catched;
		}

		private static bool TryConvertKey (Keys keyPressed, out char key)
		{
			bool shift = Keys.LeftShift.IsHeldDown () || Keys.RightShift.IsHeldDown ();            
           
			char c = (char)keyPressed.GetHashCode ();
			if (c >= 'A' && c <= 'Z') {
				if (shift)
					key = char.ToUpper (c);
				else
					key = char.ToLower (c);
				return true;
			}

			switch (keyPressed) {
			//Decimal keys
			case Keys.D0:
				if (shift)
					key = ')';
				else
					key = '0';
				return true;
			case Keys.D1:
				if (shift)
					key = '!';
				else
					key = '1';
				return true;
			case Keys.D2:
				if (shift)
					key = '@';
				else
					key = '2';
				return true;
			case Keys.D3:
				if (shift)
					key = '#';
				else
					key = '3';
				return true;
			case Keys.D4:
				if (shift)
					key = '$';
				else
					key = '4';
				return true;
			case Keys.D5:
				if (shift)
					key = '%';
				else
					key = '5';
				return true;
			case Keys.D6:
				if (shift)
					key = '^';
				else
					key = '6';
				return true;
			case Keys.D7:
				if (shift)
					key = '&';
				else
					key = '7';
				return true;
			case Keys.D8:
				if (shift)
					key = '*';
				else
					key = '8';
				return true;
			case Keys.D9:
				if (shift)
					key = '(';
				else
					key = '9';
				return true;
 
			//Decimal numpad keys
			case Keys.NumPad0:
				key = '0';
				return true;
			case Keys.NumPad1:
				key = '1';
				return true;
			case Keys.NumPad2:
				key = '2';
				return true;
			case Keys.NumPad3:
				key = '3';
				return true;
			case Keys.NumPad4:
				key = '4';
				return true;
			case Keys.NumPad5:
				key = '5';
				return true;
			case Keys.NumPad6:
				key = '6';
				return true;
			case Keys.NumPad7:
				key = '7';
				return true;
			case Keys.NumPad8:
				key = '8';
				return true;
			case Keys.NumPad9:
				key = '9';
				return true;
                    
			//Special keys
			case Keys.OemTilde:
				if (shift)
					key = '~';
				else
					key = '`';
				return true;
			case Keys.OemSemicolon:
				if (shift)
					key = ':';
				else
					key = ';';
				return true;
			case Keys.OemQuotes:
				if (shift)
					key = '"';
				else
					key = '\'';
				return true;
			case Keys.OemQuestion:
				if (shift)
					key = '?';
				else
					key = '/';
				return true;
			case Keys.OemPlus:
				if (shift)
					key = '+';
				else
					key = '=';
				return true;
			case Keys.OemPipe:
				if (shift)
					key = '|';
				else
					key = '\\';
				return true;
			case Keys.OemPeriod:
				if (shift)
					key = '>';
				else
					key = '.';
				return true;
			case Keys.OemOpenBrackets:
				if (shift)
					key = '{';
				else
					key = '[';
				return true;
			case Keys.OemCloseBrackets:
				if (shift)
					key = '}';
				else
					key = ']';
				return true;
			case Keys.OemMinus:
				if (shift)
					key = '_';
				else
					key = '-';
				return true;
			case Keys.OemComma:
				if (shift)
					key = '<';
				else
					key = ',';
				return true;
			case Keys.Space:
				key = ' ';
				return true;                                       
			}
 
			key = (char)0;
			return false;           
		}
	}
}

