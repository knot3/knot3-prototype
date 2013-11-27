using System;

using Knot3.KnotData;

namespace Knot3.GameObjects
{
	public interface IEdgeChangeListener
	{
		void OnEdgesChanged (EdgeList edges);
	}
}

