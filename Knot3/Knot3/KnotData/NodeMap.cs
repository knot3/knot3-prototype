using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using Knot3.Core;
using Knot3.Utilities;
using Knot3.KnotData;
using Knot3.RenderEffects;

using System.Collections;

namespace Knot3.KnotData
{
	public class NodeMap
	{
		private Hashtable fromMap = new Hashtable ();
		private Hashtable toMap = new Hashtable ();

		public NodeMap ()
		{
		}

		public NodeMap (IEnumerable<Edge> edges)
		{
			BuildIndex (edges);
		}

		public Node FromNode (Edge edge)
		{
			return (Node)fromMap [edge];
		}

		public Node ToNode (Edge edge)
		{
			return (Node)toMap [edge];
		}

		public void OnEdgesChanged (IEnumerable<Edge> edges)
		{
			BuildIndex (edges);
		}

		private void BuildIndex (IEnumerable<Edge> edges)
		{
			fromMap.Clear ();
			toMap.Clear ();
			float x = 0, y = 0, z = 0;
			foreach (Edge edge in edges) {
				fromMap [edge] = new Node ((int)x, (int)y, (int)z);
				Vector3 v = edge.Direction.ToVector3 ();
				x += v.X;
				y += v.Y;
				z += v.Z;
				toMap [edge] = new Node ((int)x, (int)y, (int)z);
			}
		}
	}
}
