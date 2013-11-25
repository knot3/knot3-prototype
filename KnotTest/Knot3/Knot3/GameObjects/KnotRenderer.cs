using System;

using Knot3.KnotData;

namespace Knot3.GameObjects
{
	public abstract class KnotRenderer : GameObject
	{
		public KnotRenderer (GameState state)
			: base(state)
		{
		}

		public abstract void OnEdgesChanged (EdgeList edges);
	}
}

