using System;

using Knot3.KnotData;

namespace Knot3.GameObjects
{
	public interface IEdgeChangeReceiver
	{
		void OnEdgesChanged (EdgeList edges);
	}
}

