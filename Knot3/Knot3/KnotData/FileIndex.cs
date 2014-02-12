using System;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

using Knot3.Utilities;

using System.IO;

namespace Knot3.KnotData
{
	public class FileIndex
	{
		private HashSet<string> hashes;
		private string filename;

		public FileIndex (string filename)
		{
			this.filename = filename;
			try {
				hashes = new HashSet<string> (Files.ReadFrom (filename));
			}
			catch (IOException) {
				hashes = new HashSet<string> ();
			}
		}

		public void Add (string hash)
		{
			hashes.Add (hash);
			Save ();
		}

		public void Remove (string hash)
		{
			hashes.Remove (hash);
			Save ();
		}

		public bool Contains (string hash)
		{
			return hashes.Contains (hash);
		}

		private void Save ()
		{
			File.WriteAllText (filename, string.Join ("\n", hashes));
		}
	}

	public static class StringExtensions
	{
		public static string ToMD5Hash (this string TextToHash)
		{
			if (string.IsNullOrEmpty (TextToHash)) {
				return string.Empty;
			}

			MD5 md5 = new MD5CryptoServiceProvider ();
			byte[] textToHash = Encoding.Default.GetBytes (TextToHash);
			byte[] result = md5.ComputeHash (textToHash);

			return System.BitConverter.ToString (result);
		}
	}
}
