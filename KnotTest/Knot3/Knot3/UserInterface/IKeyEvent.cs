using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Knot3.UserInterface
{
	public interface IKeyEvent
	{
		void Activate (GameTime gameTime);

		List<Keys> ValidKeys { get; }

		int Index { get; }

		bool IsKeyEventEnabled { get; }
	}
}

