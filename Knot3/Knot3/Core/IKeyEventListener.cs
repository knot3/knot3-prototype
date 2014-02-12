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
	public interface IKeyEventListener
	{
		void OnKeyEvent (List<Keys> key, KeyEvent keyEvent, GameTime time);

		List<Keys> ValidKeys { get; }

		DisplayLayer Index { get; }

		bool IsKeyEventEnabled { get; }
	}

	public enum KeyEvent {
		None = 0, KeyDown, KeyHeldDown
	}
}
