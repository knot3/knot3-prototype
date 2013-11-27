using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Xna = Microsoft.Xna.Framework;

namespace Knot3.Core
{
	/// <summary>
	/// Erbt von dem IGameComponent-Interface von XNA und von IGameClass und stellt somit einen GameComponent bereit,
	/// der immer nur im Zusammenhang mit einem bestimmten GameState verwendet wird.
	/// </summary>
	public interface IGameStateComponent : Xna.IGameComponent
	{
		IEnumerable<IGameStateComponent> SubComponents (GameTime gameTime);

		int Index { get; }
	}
}

