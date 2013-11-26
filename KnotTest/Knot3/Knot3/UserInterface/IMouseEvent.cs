using System;

using Microsoft.Xna.Framework;

namespace Knot3.UserInterface
{
	public interface IMouseEvent
	{
		void Activate ();

		void SetHovered (bool hovered);

		Rectangle bounds ();

		int Index { get; }

		bool IsVisible { get; }
	}
}

