using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace TestGame1
{
	public class IniFile : IDisposable
	{
		private readonly List<string> lines_ = new List<string> ();

		protected string this [int index] {
			get { return lines_ [index]; }
			set { lines_ [index] = value; }
		}

		protected int Count {
			get { return lines_.Count; }
		}

		private readonly string fileName_;
		public virtual string FileName {
			get { return fileName_; }
		}

		protected void Add (string line)
		{
			lines_.Add (line);
		}

		protected void Insert (int index, string line)
		{
			lines_.Insert (index, line);
		}

		protected void RemoveLine (int index)
		{
			lines_.RemoveAt (index);
		}

		public IniFile (string fileName)
		{
			fileName_ = fileName;
			if (File.Exists (fileName)) {
				using (StreamReader reader = new StreamReader(fileName)) {
					while (reader.Peek() != -1)
						Add (reader.ReadLine ().Trim ());
				}
			}
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (disposing)
				UpdateFile ();
		}

		public virtual void UpdateFile ()
		{
			using (StreamWriter writer = new StreamWriter(FileName)) {
				for (int i = 0; i < Count; i++)
					writer.WriteLine (this [i]);
			}
		}

		protected static string StripComments (string line)
		{
			if (line != null) {
				if (line.IndexOf (';') != -1)
					return line.Remove (line.IndexOf (';')).Trim ();
				return line.Trim ();
			}
			return string.Empty;
		}

		private int SkipToSection (string name)
		{
			if (name != null) {
				string needle = "[" + name + "]";
				for (int i = 0; i < Count; i++) {
					if (StripComments (this [i]) == needle)
						return i;
				}
			}
			return -1;
		}

		public virtual bool SectionExists (string name)
		{
			return SkipToSection (name) != -1;
		}

		public virtual void DeleteKey (string section, string name)
		{
			int i = SkipToSection (section);
			if (i != -1) {
				for (; i < Count; i++) {
					string line = this [i];
					if (line.StartsWith (name + '=', StringComparison.Ordinal)
						|| line.StartsWith (name + " =",
                                           StringComparison.Ordinal)) {
						RemoveLine (i);
						return;
					}
				}
			}
		}

		public virtual void EraseSection (string section)
		{
			int i = SkipToSection (section);
			if (i != -1) {
				RemoveLine (i);

				for (; i < Count; i++) {
					string line = StripComments (this [i]);
					if (line.Length != 0 && line [0] == '['
						&& line [line.Length - 1] == ']')
						return;

					RemoveLine (i);
				}
			}
		}

		public virtual string ReadString (string section, string key)
		{
			return ReadString (section, key, String.Empty);
		}

		private int FindKey (string key, int i)
		{
			if (key != null) {
				for (; i < Count; i++) {
					string line = StripComments (this [i]);
					if (line.StartsWith (key + '=', StringComparison.Ordinal)
						|| line.StartsWith (key + " =", StringComparison.Ordinal))
						return i;
				}
			}
			return -1;
		}

		public virtual string ReadString (string section, string key,
                                         string defaultvalue)
		{
			int i = SkipToSection (section);
			if (i != -1) {
				i = FindKey (key, i);
				if (i != -1) {
					string line = StripComments (this [i]);
					char[] trimmer = new char[] { ' ', '"', '\r' };
					return line.Substring (line.IndexOf ('=') + 1).Trim (trimmer);
				}
			}
			return defaultvalue;
		}

		public virtual void WriteString (string section, string key,
                                        string value)
		{
			if (section == null || key == null || value == null)
				return;
			string newLine = key + '=' + value;
			int i = SkipToSection (section);
			if (i == -1) {
				Add ("[" + section + "]");
				Add (newLine);
			} else {
				i++;
				int j = FindKey (key, i);
				if (j != -1)
					this [i] = newLine;
				else
					Insert (i + 1, newLine);
			}
		}
	}
}

