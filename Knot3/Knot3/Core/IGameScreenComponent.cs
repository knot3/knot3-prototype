using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Xna = Microsoft.Xna.Framework;

namespace Knot3.Core
{
	/// <summary>
	/// Erbt von dem IGameComponent-Interface von XNA und von IGameClass und stellt somit einen GameComponent bereit,
	/// der immer nur im Zusammenhang mit einem bestimmten GameScreen verwendet wird.
	/// </summary>
	public interface IGameScreenComponent : Xna.IGameComponent
	{
		IEnumerable<IGameScreenComponent> SubComponents (GameTime time);

		DisplayLayer Index { get; }
	}
}
