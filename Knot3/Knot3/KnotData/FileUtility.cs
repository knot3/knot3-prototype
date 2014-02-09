using System;

using Knot3.Utilities;

namespace Knot3.KnotData
{
	public static class FileUtility
	{
		public static string GetHash (string filename)
		{
			return string.Join ("\n", Files.ReadFrom (filename)).ToMD5Hash ();
		}

		public static string ConvertToFilename (string humanReadableName)
		{
			char[] arr = humanReadableName.ToCharArray ();
			arr = Array.FindAll<char> (arr, (c => (char.IsLetterOrDigit (c) 
				|| char.IsWhiteSpace (c) 
				|| c == '-'))
			);
			return new string (arr);
		}
	}
}

