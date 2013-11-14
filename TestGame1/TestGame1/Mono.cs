using System;

namespace TestGame1
{
	public static class Mono
	{
		public static bool IsRunningOnMono ()
		{
			return Type.GetType ("Mono.Runtime") != null;
		}
		
	}
}

