using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Knot3.Utilities
{
	public static class MonoHelper
	{
		public static bool IsRunningOnMono ()
		{
			return Type.GetType ("Mono.Runtime") != null;
		}
	}
}

