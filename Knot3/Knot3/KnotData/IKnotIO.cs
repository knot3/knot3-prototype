using System;
using System.Collections.Generic;

namespace Knot3.KnotData
{
	public interface IKnotIO
	{
		Knot Load(string filename);

		KnotMetaData LoadMetaData(string filename);

		void Save (Knot knot);
	}
}
