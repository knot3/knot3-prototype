using System;
using System.Collections.Generic;

namespace Knot3.KnotData
{
	public interface IKnotIO
	{
		IEnumerable<Edge> Edges { get; }

		KnotMetaData MetaData { get; }

		string Name { get; }

		void Save (Knot knot);
	}
}

