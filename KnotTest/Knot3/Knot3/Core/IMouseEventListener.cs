using System;

using Microsoft.Xna.Framework;

namespace Knot3.Core
{
	/// <summary>
	/// Dieses Interface wird von Klassen implementiert, die Mausklicks abfangen. Es enthält Informationen über
	/// den Bereich des Bildschirms, auf dem sich das Objekt befindet, sowie über den Z-Index des 
	/// anklickbaren Objekts.
	/// </summary>
	public interface IMouseEventListener
	{
		void Activate (GameTime gameTime);

		void SetHovered (bool hovered);

		Rectangle bounds ();

		int Index { get; }

		bool IsMouseEventEnabled { get; }
	}
}

