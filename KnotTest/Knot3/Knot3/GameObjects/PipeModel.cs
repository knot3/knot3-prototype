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
		public EdgeList Edges;
		public Edge Edge;
		public Vector3 Direction;
		public Vector3 PositionFrom;
		public Vector3 PositionTo;

		public PipeModelInfo (EdgeList edges, Edge edge, Vector3 offset)
			: base("pipe1")
		{
			Edges = edges;
			Edge = edge;
			Node node1 = edges.FromNode (edge);
			Node node2 = edges.ToNode (edge);
			PositionFrom = node1.Vector () + offset;
			PositionTo = node2.Vector () + offset;
			Position = PositionFrom + (PositionTo - PositionFrom) / 2;
			Direction = PositionTo - PositionFrom;
			Direction.Normalize ();
			Scale = 10f;
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

		public new PipeModelInfo Info { get; private set; }

		private BoundingSphere[] Bounds;
		public Action OnDataChange = () => {};

		#endregion

		public PipeModel (GameState state, PipeModelInfo info)
			: base(state, info)
		{
			Info = info;

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
			Bounds = new BoundingSphere[(int)(length / (Info.Scale / 4))];
			for (int offset = 0; offset < (int)(length / (Info.Scale / 4)); ++offset) {
				Bounds [offset] = new BoundingSphere (info.PositionFrom + Info.Direction * offset * (Info.Scale / 4), Info.Scale);
				//Console.WriteLine ("sphere[" + offset + "]=" + Bounds [offset]);
			}
		}

		public override void Draw (GameTime gameTime)
		{
			BaseColor = Info.Edge.Color;
			if (Info.Edges.SelectedEdges.Contains (Info.Edge)) {
				float intensity = (float)((int)gameTime.TotalGameTime.TotalMilliseconds % 2000) / 2000f;
				intensity = intensity * 2 - 1;
				if (intensity < 0) {
					intensity = 0 - intensity;
				}
				HighlightIntensity = intensity * 0.9f;
				HighlightColor = Color.White;
			} else if (World.SelectedObject == this) {
				HighlightIntensity = 0.5f;
				HighlightColor = Color.White;
			} else {
				HighlightIntensity = 0f;
			}

			base.Draw (gameTime);
		}

		public override void Update (GameTime gameTime)
		{
			// check whether this object is hovered
			if (World.SelectedObject == this) {

				// if the left mouse button is pressed, select the edge
				if (Core.Input.LeftButton == ClickState.SingleClick) {
					try {
						// CTRL
						if (Keys.LeftControl.IsHeldDown ()) {
							Info.Edges.SelectEdge (Info.Edge, true);
						}
						// Shift
						else if (Keys.LeftShift.IsHeldDown ()) {
							WrapList<Edge> selection = Info.Edges.SelectedEdges;
							if (selection.Count != 0) {
								Edge last = selection [selection.Count - 1];
								Info.Edges.SelectEdges (Info.Edges.Interval (last, Info.Edge).ToArray (), true);
							}
							Info.Edges.SelectEdge (Info.Edge, true);
						}
						// mouse click only
						else {
							Info.Edges.SelectEdge (Info.Edge, false);
						}
					} catch (ArgumentOutOfRangeException exp) {
						Console.WriteLine (exp.ToString ());
					}
				}
			}

			base.Update (gameTime);
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

	public class PipeModelFactory : ModelFactory
	{
		protected override GameModel CreateModel (GameState state, GameModelInfo info)
		{
			return new PipeModel (state, info as PipeModelInfo);
		}
	}
}

