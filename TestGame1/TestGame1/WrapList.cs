using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TestGame1
{
	public class WrapList<T> : IEnumerable<T>
	{
		private List<T> list = new List<T> ();
		private Dictionary<T, int> indexOf = new Dictionary<T, int> ();

		public WrapList ()
		{
		}

		public WrapList (IEnumerable<T> elems)
		{
			foreach (T elem in elems) {
				Add (elem);
			}
		}

		private int WrapIndex (int i)
		{
			return (i + list.Count) % list.Count;
		}

		public T this [int i] {
			set {
				i = WrapIndex (i);
				list [i] = value;
				indexOf [value] = i;
			}
			get {
				i = WrapIndex (i);
				return list [i];
			}
		}

		public int this [T t] {
			get {
				return indexOf [t];
			}
		}

		public int Count { get { return list.Count; } }

		public void Clear ()
		{
			list.Clear ();
			indexOf.Clear ();
		}

		public void Add (T elem)
		{
			list.Remove (elem);
			list.Add (elem);
			indexOf [elem] = list.Count - 1;
		}

		public void Remove (T[] elems)
		{
			foreach (T elem in elems) {
				indexOf.Remove (elem);
				list.Remove (elem);
			}
			for (int i = 0; i < list.Count; ++i) {
				indexOf [list [i]] = i;
			}
		}

		public void InsertAt (int i, T elem)
		{
			i = WrapIndex (i);
			list.Insert (i, elem);
			for (; i < list.Count; ++i) {
				indexOf [list [i]] = i;
			}
		}

		public void Replace (T find, T[] elem)
		{
			int i = indexOf [find];
			indexOf.Remove (find);
			list.Remove (find);
			list.InsertRange (i, elem);
			for (; i < list.Count; ++i) {
				indexOf [list [i]] = i;
			}
		}

		public void AddRange (IEnumerable<T> elems)
		{
			foreach (T elem in elems) {
				Add (elem);
			}
		}

		public bool Contains (T elem)
		{
			return indexOf.ContainsKey (elem);
		}

		public IEnumerator<T> GetEnumerator ()
		{
			return list.GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		public override string ToString ()
		{
			string str = "";
			foreach (T elem in list) {
				if (str.Length > 0)
					str += ", ";
				str += elem;
			}
			return str;
		}

	}
}

