using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Input;

namespace Knot3.UserInterface
{
	public interface IKeyEvent
	{
		void Activate ();

		List<Keys> Keys { get; }

		int Index { get; }

		bool IsVisible { get; }
	}
}

