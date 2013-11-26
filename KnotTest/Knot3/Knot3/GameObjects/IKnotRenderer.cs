using System;

using Knot3.KnotData;

namespace Knot3.GameObjects
{
	public interface IKnotRenderer : IGameObject
	{
		void OnEdgesChanged (EdgeList edges);
	}
}

