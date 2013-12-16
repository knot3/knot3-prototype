using System;
using System.Collections.Generic;

namespace Knot3.KnotData
{
	public interface IKnotIO
	{
		IEnumerable<Edge> Edges { get; }

		int CountEdges { get; }

		string Name { get; }

		string Hash { get; }

		void Save (Knot knot);
	}
}

