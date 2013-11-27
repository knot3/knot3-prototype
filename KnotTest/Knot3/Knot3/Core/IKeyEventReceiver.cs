using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Knot3.Core
{
	/// <summary>
	/// Dieses Interface wird von Klassen implementiert, die Tastatureingaben abfangen. Es stellt eine Liste
	/// der abzufangenden Tasten sowie eine Priorität zur Verfügung.
	/// </summary>
	public interface IKeyEventReceiver
	{
		void OnKeyEvent (List<Keys> key, KeyEvent keyEvent, GameTime gameTime);

		List<Keys> ValidKeys { get; }

		int Index { get; }

		bool IsKeyEventEnabled { get; }
	}

	public enum KeyEvent {
		None = 0, KeyDown, KeyHeldDown
	}
}

