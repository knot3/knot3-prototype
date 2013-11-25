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

using Knot3.Utilities;
using Knot3.KnotData;
using Knot3.RenderEffects;

namespace Knot3.GameObjects
{
	public abstract class ModelCache : GameClass
	{
		public ModelCache (GameState state)
			: base(state)
		{
		}
	}

	public class PipeModelCache : ModelCache
	{
		// cache
		private Dictionary<string, PipeModel> pipeCache = new Dictionary<string, PipeModel> ();

		public PipeModelCache (GameState state)
			: base(state)
		{
		}

		public PipeModel this [EdgeList edges, Edge edge, Vector3 offset] {
			get {
				Node node1 = edges.FromNode (edge);
				Node node2 = edges.ToNode (edge);
				string key = edge.ID + "#" + node1 + "-" + node2;
				if (pipeCache.ContainsKey (key)) {
					return pipeCache [key];
				} else {
					Vector3 p1 = node1.Vector () + offset;
					Vector3 p2 = node2.Vector () + offset;

					PipeModel pipe = new PipeModel (state, edges, edge, p1, p2, 10f);
					pipeCache [key] = pipe;
					return pipe;
				}
			}
		}
	}

	public class NodeModelCache : ModelCache
	{
		// cache
		private Dictionary<Node, NodeModel> knotCache = new Dictionary<Node, NodeModel> ();

		public NodeModelCache (GameState state)
			: base(state)
		{
		}

		public NodeModel this [EdgeList edges, Edge edgeA, Edge edgeB, Vector3 offset] {
			get {
				Node node = edges.ToNode (edgeA);
				if (knotCache.ContainsKey (node)) {
					return knotCache [node];
				} else {
					NodeModel knot = new NodeModel (state, edges, edgeA, edgeB, edges.ToNode (edgeA).Vector (), 5f);
					knotCache [node] = knot;
					return knot;
				}
			}
		}
	}
}

