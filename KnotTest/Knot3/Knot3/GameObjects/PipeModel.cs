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
	public class PipeModel : CachedGameModel
	{
		private EdgeList Edges;
		private Edge Edge;
		private Vector3 Direction;
		private BoundingSphere[] Bounds;
		public Action OnDataChange = () => {};

		public PipeModel (GameState state, EdgeList edges, Edge edge, Vector3 posFrom, Vector3 posTo, float scale)
			: base(state, "pipe1", posFrom + (posTo-posFrom)/2, scale)
		{
			Edges = edges;
			Edge = edge;

			Direction = posTo - posFrom;
			Direction.Normalize ();

			if (Direction.Y == 1) {
				Rotation += Angles3.FromDegrees (90, 0, 0);
			} else if (Direction.Y == -1) {
				Rotation += Angles3.FromDegrees (270, 0, 0);
			}
			if (Direction.X == 1) {
				Rotation += Angles3.FromDegrees (0, 90, 0);
			} else if (Direction.X == -1) {
				Rotation += Angles3.FromDegrees (0, 270, 0);
			}

			Rotation = Rotation;

			float length = (posTo - posFrom).Length ();
			Bounds = new BoundingSphere[(int)(length / (Scale / 4))];
			for (int offset = 0; offset < (int)(length / (Scale / 4)); ++offset) {
				Bounds [offset] = new BoundingSphere (posFrom + Direction * offset * (Scale / 4), Scale);
				//Console.WriteLine ("sphere[" + offset + "]=" + Bounds [offset]);
			}
		}

		public override void OnSelected (GameTime gameTime)
		{
		}

		public override void DrawObject (GameTime gameTime)
		{
			BaseColor = Edge.Color;
			if (Edges.SelectedEdges.Contains (Edge)) {
				float intensity = (float)((int)gameTime.TotalGameTime.TotalMilliseconds % 2000) / 2000f;
				intensity = intensity * 2 - 1;
				if (intensity < 0) {
					intensity = 0 - intensity;
				}
				HighlightIntensity = intensity;
				HighlightColor = Color.Black;
			} else if (world.SelectedObject == this) {
				HighlightIntensity = 0.5f;
				HighlightColor = Color.White;
			} else {
				HighlightIntensity = 0f;
			}

			base.DrawObject (gameTime);
		}

		private Vector3 previousMousePosition = Vector3.Zero;

		public override void Update (GameTime gameTime)
		{
			// check whether this object is hovered
			if (IsSelected () == true) {

				// if the left mouse button is pressed, select the edge
				if (Input.MouseState.IsLeftClick (gameTime)) {
					try {
						// CTRL
						if (Keys.LeftControl.IsHeldDown ()) {
							Edges.SelectEdge (Edge, true);
						}
						// Shift
						else if (Keys.LeftShift.IsHeldDown ()) {
							WrapList<Edge> selection = Edges.SelectedEdges;
							if (selection.Count != 0) {
								Edge last = selection [selection.Count - 1];
								Edges.SelectEdges (Edges.Interval (last, Edge).ToArray (), true);
							}
							Edges.SelectEdge (Edge, true);
						}
						// mouse click only
						else {
							Edges.SelectEdge (Edge, false);
						}
					} catch (ArgumentOutOfRangeException exp) {
						Console.WriteLine (exp.ToString ());
					}
				}
			}

			// check whether this edge is one of the selected edges
			if (IsSelected () == true) {
				// is SelectedObjectMove the current input action?
				if (input.CurrentInputAction == InputAction.SelectedObjectMove) {
					if (previousMousePosition == Vector3.Zero) {
						previousMousePosition = device.Viewport.Unproject (new Vector3 (Input.MouseState.ToVector2 (), 1f),
								camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);
					}
					Move ();
				} else {
					previousMousePosition = Vector3.Zero;
				}
			}

			// check whether this edge is one of the selected edges
			if (Edges.SelectedEdges.Contains (Edge)) {
				
				// change color?
				if (Keys.C.IsDown ()) {
					Edge.Color = Edge.RandomColor (gameTime);
				}

				if (Input.MouseState.IsDoubleClick (gameTime)) {
				}
			}
		}

		private void Move ()
		{
			Vector3 currentMousePosition = device.Viewport.Unproject (
					new Vector3 (Input.MouseState.ToVector2 (), 1f),
					camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity
			);
			Vector3 mouseMove = currentMousePosition - previousMousePosition;

			if (mouseMove != Vector3.Zero) {
				Console.WriteLine ("mouseMove=" + mouseMove);
				Vector3 direction3D = mouseMove.PrimaryDirection ();
				if (mouseMove.PrimaryVector ().Length ().Abs () > 50) {
					try {
						Edges.SelectEdge (Edge, true);
						Edges.Move (Edges.SelectedEdges, direction3D);
						Edges.SelectEdge ();
						previousMousePosition = currentMousePosition;
					} catch (ArgumentOutOfRangeException exp) {
						Console.WriteLine (exp.ToString ());
					}
				}
			}
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

