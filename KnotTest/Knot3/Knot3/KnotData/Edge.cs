using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Knot3.Utilities;

namespace Knot3.KnotData
{
	public class Edge
	{
		#region Properties

		public Vector3 Direction { get; private set; }

		public Color Color { get; set; }

		public int ID { get; private set; }

		private static int LastID = 0; 

		#endregion

		#region Constructors

		public Edge (int x, int y, int z)
		{
			Direction = new Vector3 (x, y, z);
			Color = DefaultColor;
			ID = LastID++;
		}

		public Edge (Vector3 v)
		{
			Direction = v.PrimaryDirection ();
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
			return Direction.Print () + "/" + ID;
		}

		#endregion

		#region Helper Methods

		private static Random r = new Random ();

		public static Color RandomColor ()
		{
			return Colors [r.Next () % Colors.Count];
		}

		public static Color RandomColor (GameTime gameTime)
		{
			return Colors [(int)gameTime.TotalGameTime.TotalSeconds % Colors.Count];
		}

		public static Edge RandomEdge ()
		{
			int i = r.Next () % 6;
			return i == 0 ? Left : i == 1 ? Right : i == 2 ? Up : i == 3 ? Down : i == 4 ? Forward : Backward;
		}

		#endregion
		
		#region Static Properties
		
		public static List<Color> Colors = new List<Color> (){
			Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Orange
		};
		public static Color DefaultColor = RandomColor ();

		public static Edge Zero { get { return new Edge (0, 0, 0); } }

		public static Edge UnitX { get { return new Edge (1, 0, 0); } }

		public static Edge UnitY { get { return new Edge (0, 1, 0); } }

		public static Edge UnitZ { get { return new Edge (0, 0, 1); } }

		public static Edge Up { get { return new Edge (0, 1, 0); } }

		public static Edge Down { get { return new Edge (0, -1, 0); } }

		public static Edge Right { get { return new Edge (1, 0, 0); } }

		public static Edge Left { get { return new Edge (-1, 0, 0); } }

		public static Edge Forward { get { return new Edge (0, 0, -1); } }

		public static Edge Backward { get { return new Edge (0, 0, 1); } }

		#endregion
	}
}

