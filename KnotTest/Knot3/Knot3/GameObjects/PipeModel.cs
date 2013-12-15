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
using Knot3.Core;
using Knot3.UserInterface;

namespace Knot3.GameObjects
{
	public class PipeModelInfo : GameModelInfo
	{
		public EdgeList EdgeList;
		public NodeMap NodeMap;
		public Edge Edge;
		public Vector3 Direction;
		public Vector3 PositionFrom;
		public Vector3 PositionTo;

		public PipeModelInfo (EdgeList edgeList, NodeMap nodeMap, Edge edge, Vector3 offset)
			: base("pipe1")
		{
			EdgeList = edgeList;
			NodeMap = nodeMap;
			Edge = edge;
			Node node1 = nodeMap.FromNode (edge);
			Node node2 = nodeMap.ToNode (edge);
			PositionFrom = node1.Vector () + offset;
			PositionTo = node2.Vector () + offset;
			Position = node1.CenterBetween (node2) + offset;
			Direction = PositionTo - PositionFrom;
			Direction.Normalize ();
			Scale = Vector3.One * 10f;
			// a pipe is movable
			IsMovable = true;
		}

		public override bool Equals (GameObjectInfo other)
		{
			if (other == null) 
				return false;

			if (other is PipeModelInfo) {
				if (this.Edge == (other as PipeModelInfo).Edge && base.Equals (other))
					return true;
				else
					return false;
			} else {
				return base.Equals (other);
			}
		}
	}
	
	/// <summary>
	/// Ein GameModel, das eine 3D-Röhre zeichnet.
	/// </summary>
	public class PipeModel : GameModel
	{
		#region Attributes and Properties

		public new PipeModelInfo Info { get { return base.Info as PipeModelInfo; } set { base.Info = value; } }

		private BoundingSphere[] Bounds;
		public Action OnDataChange = () => {};

		#endregion

		public PipeModel (GameScreen screen, PipeModelInfo info)
			: base(screen, info)
		{
			if (Info.Direction.Y == 1) {
				Info.Rotation += Angles3.FromDegrees (90, 0, 0);
			} else if (Info.Direction.Y == -1) {
				Info.Rotation += Angles3.FromDegrees (270, 0, 0);
			}
			if (Info.Direction.X == 1) {
				Info.Rotation += Angles3.FromDegrees (0, 90, 0);
			} else if (Info.Direction.X == -1) {
				Info.Rotation += Angles3.FromDegrees (0, 270, 0);
			}

			float length = (info.PositionTo - info.PositionFrom).Length ();
			float radius = Info.Scale.PrimaryVector ().Length ();
			Bounds = Vectors.CylinderBounds (length, radius, Info.Direction, info.PositionFrom);

			/*float distance = radius / 4;
			Bounds = new BoundingSphere[(int)(length / distance)];
			for (int offset = 0; offset < (int)(length / distance); ++offset) {
				Bounds [offset] = new BoundingSphere (info.PositionFrom + Info.Direction * offset * distance, radius);
				//Console.WriteLine ("sphere[" + offset + "]=" + Bounds [offset]);
			}*/
		}

		public override void Draw (GameTime gameTime)
		{
			BaseColor = Info.Edge.Color;
			if (World.SelectedObject == this) {
				HighlightIntensity = 0.40f;
				HighlightColor = Color.White;
			} else if (Info.EdgeList.SelectedEdges.Contains (Info.Edge)) {
				HighlightIntensity = 0.80f;
				HighlightColor = Color.White;
			} else {
				HighlightIntensity = 0f;
			}

			base.Draw (gameTime);
		}

		public override GameObjectDistance Intersects (Ray ray)
		{
			foreach (BoundingSphere sphere in Bounds) {
				float? distance = ray.Intersects (sphere);
				if (distance != null) {
					GameObjectDistance intersection = new GameObjectDistance () {
						Object=this, Distance=distance.Value
					};
					return intersection;
				}
			}
			return null;
		}
	}
}

