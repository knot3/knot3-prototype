using System;

namespace Knot3.KnotData
{
	public class FileIO
	{
		protected string Filename;

		public virtual string ConvertToFilename (string humanReadableName)
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

