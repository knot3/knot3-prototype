using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Knot3.Utilities;

namespace Knot3.KnotData
{
	/// <summary>
	/// Eine Kante, die aus einer Richung und einer Farbe besteht.
	/// </summary>
	public class Edge
	{
		#region Properties

		/// <summary>
		/// Der Richtungsvektor der Kante. Ist immer (1,0,0), (-1,0,0), (0,1,0), (0,-1,0), (0,0,1) oder (0,0,-1).
		/// </summary>
		/// <value>
		/// The direction.
		/// </value>
		public Direction Direction { get; private set; }

		/// <summary>
		/// Die Farbe der Kante.
		/// </summary>
		/// <value>
		/// The color.
		/// </value>
		public Color Color { get; set; }

		/// <summary>
		/// Die eindeutige Nummer der Kante.
		/// </summary>
		/// <value>
		/// The ID.
		/// </value>
		public int ID { get; private set; }

		private static int LastID = 0;

		#endregion

		#region Constructors

		public Edge (Direction direction)
		{
			Direction = direction;
			Color = DefaultColor;
			ID = LastID++;
		}

		#endregion

		#region Operators

		public static bool operator == (Edge a, Edge b)
		{
			// If both are null, or both are same instance, return true.
			if (System.Object.ReferenceEquals (a, b)) {
				return true;
			}

			// If one is null, but not both, return false.
			if (((object)a == null) || ((object)b == null)) {
				return false;
			}

			// Return true if the fields match:
			return a.ID == b.ID;
		}

		public static bool operator != (Edge a, Edge b)
		{
			return !(a == b);
		}

		public override bool Equals (object obj)
		{
			Edge other = obj as Edge;
			return this.ID == other.ID;
		}

		public override int GetHashCode ()
		{
			return ID;
		}

		public override string ToString ()
		{
			return Direction.ToString () + "/" + ID;
		}

		#endregion

		#region Helper Methods

		private static Random r = new Random ();

		public static Color RandomColor ()
		{
			return Colors [r.Next () % Colors.Count];
		}

		public static Color RandomColor (GameTime time)
		{
			return Colors [(int)time.TotalGameTime.TotalSeconds % Colors.Count];
		}

		public static Edge RandomEdge ()
		{
			int i = r.Next () % 6;
			return i == 0 ? Left : i == 1 ? Right : i == 2 ? Up : i == 3 ? Down : i == 4 ? Forward : Backward;
		}

		#endregion

		#region Static Properties

		public static List<Color> Colors = new List<Color> ()
		{
			Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Orange
		};
		public static Color DefaultColor = RandomColor ();

		public static Edge Zero { get { return new Edge (Direction.Zero); } }

		public static Edge UnitX { get { return new Edge (Direction.Right); } }

		public static Edge UnitY { get { return new Edge (Direction.Up); } }

		public static Edge UnitZ { get { return new Edge (Direction.Backward); } }

		public static Edge Up { get { return new Edge (Direction.Up); } }

		public static Edge Down { get { return new Edge (Direction.Down); } }

		public static Edge Right { get { return new Edge (Direction.Right); } }

		public static Edge Left { get { return new Edge (Direction.Left); } }

		public static Edge Forward { get { return new Edge (Direction.Forward); } }

		public static Edge Backward { get { return new Edge (Direction.Backward); } }

		#endregion
	}
}
