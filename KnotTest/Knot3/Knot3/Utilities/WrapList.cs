using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Knot3.Utilities
{
	public class WrapList<T> : IEnumerable<T>
	{
		private List<T> list = new List<T> ();
		private Dictionary<T, int> indexOf = new Dictionary<T, int> ();
		public Action<WrapList<T>> SelectionChanged = (l) => {};

		public WrapList ()
		{
		}

		public WrapList (IEnumerable<T> elems)
		{
			foreach (T elem in elems) {
				Add (elem);
			}
		}

		private void RebuildIndex ()
		{
			indexOf.Clear ();
			for (int i = 0; i < list.Count; ++i) {
				indexOf [list [i]] = i;
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
			SelectionChanged (this);
		}

		public void Add (T elem)
		{
			list.Remove (elem);
			list.Add (elem);
			RebuildIndex ();
			SelectionChanged (this);
		}

		public void Remove (params T[] elems)
		{
			foreach (T elem in elems) {
				indexOf.Remove (elem);
				list.Remove (elem);
			}
			RebuildIndex ();
			SelectionChanged (this);
		}

		public void InsertAt (int i, T elem)
		{
			i = WrapIndex (i);
			list.Insert (i, elem);
			RebuildIndex ();
			SelectionChanged (this);
		}

		public void Replace (T find, params T[] elem)
		{
			if (Contains (find)) {
				int i = indexOf [find];
				indexOf.Remove (find);
				list.Remove (find);
				list.InsertRange (i, elem);
				RebuildIndex ();
				SelectionChanged (this);
			}
		}

		public void AddRange (params T[] elems)
		{
			foreach (T elem in elems) {
				Add (elem);
			}
		}

		public void AddRange (IEnumerable<T> elems)
		{
			AddRange (elems.ToArray ());
		}

		public void Set (params T[] elems)
		{
			Clear ();
			AddRange (elems);
		}

		public void Set (IEnumerable<T> elems)
		{
			Set (elems.ToArray ());
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

	public static class Lists
	{
		public static void Times (this int count, Action action)
		{
			for (int i=0; i < count; i++) {
				action ();
			}
		}

		public static void Times (this int count, Action<int> action)
		{
			for (int i=0; i < count; i++) {
				action (i);
			}
		}
	}
}

