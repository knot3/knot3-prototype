using System;
using System.Collections.Generic;

namespace Knot3.GameObjects
{
	public interface IGameObjectContainer
	{
		IEnumerable<IGameObject> SubGameObjects ();
	}
}

