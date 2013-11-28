using System;

using Knot3.KnotData;

namespace Knot3.GameObjects
{
	/// <summary>
	/// Klassen, die diese  Schnittstelle implementieren, werden benachrichtigt, falls sich die Kanten eines
	/// Knoten geändert haben.
	/// </summary>
	public interface IEdgeChangeListener
	{
		void OnEdgesChanged (EdgeList edges);
	}
}

