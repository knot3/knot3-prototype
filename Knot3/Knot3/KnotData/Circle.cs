using System;
using System.Collections.Generic;
using System.Collections;

namespace Knot3.KnotData
{
	public class Circle<T> : IEnumerable<T>
	{
		public T Content { get; set; }

		public Circle<T> Next { get; set; }

		public Circle<T> Previous { get; set; }

		public Circle (T content)
		{
			Content = content;
			Previous = this;
			Next = this;
		}

		public Circle (IEnumerable<T> list)
		{
			bool first = true;
			Circle<T> inserted = this;
			foreach (T obj in list) {
				if (first) {
					Content = obj;
					Previous = this;
					Next = this;
				}
				else {
					inserted = inserted.InsertAfter (obj);
				}
				first = false;
			}
		}

		public Circle<T> InsertBefore (T obj)
		{
			Circle<T> insert = new Circle<T> (obj);
			insert.Previous = this.Previous;
			insert.Next = this;
			this.Previous.Next = insert;
			this.Previous = insert;
			return insert;
		}

		public Circle<T> InsertAfter (T obj)
		{
			Console.WriteLine (this + ".InsertAfter(" + obj + ")");
			Circle<T> insert = new Circle<T> (obj);
			insert.Next = this.Next;
			insert.Previous = this;
			this.Next.Previous = insert;
			this.Next = insert;
			return insert;
		}

		public void Remove ()
		{
			Previous.Next = Next;
			Next.Previous = Previous;
		}

		public int Count
		{
			get {
				Circle<T> current = this;
				int count = 0;
				do {
					++count;
					current = current.Next;
				}
				while (current != this);
				return count;
			}
		}

		public bool Contains (T obj, out Circle<T> item)
		{
			item = Find (obj);
			return item != null;
		}

		public Circle<T> Find (T obj)
		{
			Circle<T> current = this;
			do {
				if (current.Content.Equals (obj)) {
					return current;
				}
				current = current.Next;
			}
			while (current != this);
			return null;
		}

		public IEnumerable<T> RangeTo (Circle<T> other)
		{
			Circle<T> current = this;
			do {
				yield return current.Content;
				current = current.Next;
			}
			while (current != other && current != this);
		}

		public IEnumerator<T> GetEnumerator ()
		{
			Circle<T> current = this;
			do {
				Console.WriteLine (this + " => " + current.Content);
				yield return current.Content;
				current = current.Next;
			}
			while (current != this);
		}

		// explicit interface implementation for nongeneric interface
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator (); // just return the generic version
		}
	}
}
