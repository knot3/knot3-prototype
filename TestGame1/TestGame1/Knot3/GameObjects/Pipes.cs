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
	public class PipeRenderer : GameObject
	{
		// pipes and knots
		private List<PipeModel> pipes;
		private List<KnotModel> knots;
		private PipeModelCache pipeCache;
		private KnotModelCache knotCache;

		protected override Vector3 Position { get; set; }

		public PipeRenderer (GameState state)
			: base(state)
		{
			pipes = new List<PipeModel> ();
			knots = new List<KnotModel> ();
			pipeCache = new PipeModelCache (state);
			knotCache = new KnotModelCache (state);
			Position = Vector3.Zero; //new Vector3 (10, 10, 10);
		}

		public override void Update (GameTime gameTime)
		{
			for (int i = 0; i < pipes.Count; ++i) {
				pipes [i].Update (gameTime);
			}
			for (int i = 0; i < knots.Count; ++i) {
				knots [i].Update (gameTime);
			}
		}

		public void OnEdgesChanged (EdgeList edges)
		{
			pipes.Clear ();
			for (int n = 0; n < edges.Count; n++) {
				PipeModel pipe = pipeCache [edges, edges [n], Position];
				// pipe.OnDataChange = () => UpdatePipes (edges);
				pipes.Add (pipe);
			}

			knots.Clear ();
			for (int n = 0; n < edges.Count; n++) {
				KnotModel knot = knotCache [edges, edges [n], edges [n + 1], Position];
				knots.Add (knot);
			}
		}
		
		public override void DrawObject (GameTime gameTime)
		{
			foreach (PipeModel pipe in pipes) {
				pipe.Draw (gameTime);
			}
			foreach (KnotModel knot in knots) {
				knot.Draw (gameTime);
			}
		}

		public override GameObjectDistance Intersects (Ray ray)
		{
			GameObjectDistance nearest = null;
			if (!input.GrabMouseMovement) {
				foreach (PipeModel pipe in pipes) {
					GameObjectDistance intersection = pipe.Intersects (ray);
					if (intersection != null) {
						if (intersection.Distance > 0 && (nearest == null || intersection.Distance < nearest.Distance)) {
							nearest = intersection;
						}
					}
				}
			}
			return nearest;
		}

		public override Vector3 Center ()
		{
			return Position;
		}
	}
	
	public class KnotModel : GameModel
	{
		private EdgeList Edges;
		private Edge EdgeA;
		private Edge EdgeB;

		public KnotModel (GameState state, EdgeList edges, Edge edgeA, Edge edgeB, Vector3 position, float scale)
			: base(state, "knot1", position, scale)
		{
			Edges = edges;
			EdgeA = edgeA;
			EdgeB = edgeB;
			IsVisible = edgeA.Direction != edgeB.Direction;
		}

		public override void DrawObject (GameTime gameTime)
		{
			Color mix = EdgeA.Color.Mix (EdgeB.Color);
			if (state.PostProcessing is CelShadingEffect) {
				(state.PostProcessing as CelShadingEffect).SetColor (mix);
			} else {
				foreach (ModelMesh mesh in Model.Meshes) {
					foreach (Effect effect in mesh.Effects) {
						if (effect is BasicEffect) {
							(effect as BasicEffect).DiffuseColor = mix.ToVector3 ();
						} else {
							Console.WriteLine ("KnotModel: effect is not BasicEffect! (" + effect.CurrentTechnique.Name + ")");
						}
					}
				}
			}

			base.DrawObject (gameTime);
		}
	}
	
	public class PipeModel : GameModel
	{
		private EdgeList Edges;
		private Edge Edge;
		private Vector3 PosFrom;
		private Vector3 PosTo;
		private Vector3 Direction;
		private BoundingSphere[] Bounds;
		public Action OnDataChange = () => {};

		public PipeModel (GameState state, EdgeList edges, Edge edge, Vector3 posFrom, Vector3 posTo, float scale)
			: base(state, "pipe1", posFrom + (posTo-posFrom)/2, scale)
		{
			Edges = edges;
			Edge = edge;
			PosFrom = posFrom;
			PosTo = posTo;

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

		public override void UpdateEffect (BasicEffect effect, GameTime gameTime)
		{
			float distance = (Center () - camera.Position).Length ();
			if (Edges.SelectedEdges.Contains (Edge)) {
				effect.FogEnabled = true;
				float fogIntensity = ((int)gameTime.TotalGameTime.TotalMilliseconds % 2000 - 1000) / 10;
				if (fogIntensity < 0)
					fogIntensity = 0 - fogIntensity;
				effect.FogColor = Color.Black.ToVector3 ();
				effect.FogStart = distance - 150 - fogIntensity;
				effect.FogEnd = distance + 150 - fogIntensity;

			} else if (world.SelectedObject == this) {
				effect.FogEnabled = true;
				effect.FogColor = Color.White.ToVector3 ();
				effect.FogStart = distance - 100;
				effect.FogEnd = distance + 200;

			} else {
				effect.FogEnabled = false;
			}
		}

		public override void DrawObject (GameTime gameTime)
		{
			// if we are using a cel shading effect
			if (state.PostProcessing is CelShadingEffect) {
				if (world.SelectedObject == this)
					(state.PostProcessing as CelShadingEffect).SetColor (Color.White.Mix(Edge.Color));
				else if (Edges.SelectedEdges.Contains (Edge))
					(state.PostProcessing as CelShadingEffect).SetColor (Color.Black.Mix(Edge.Color));
				else
					(state.PostProcessing as CelShadingEffect).SetColor (Edge.Color);

			}
			// or a basic effect
			else {
				foreach (ModelMesh mesh in Model.Meshes) {
					foreach (Effect effect in mesh.Effects) {
						if (effect is BasicEffect) {
							(effect as BasicEffect).DiffuseColor = Edge.Color.ToVector3 ();
						} else {
							Console.WriteLine ("PipeModel: effect is not BasicEffect! (" + effect.CurrentTechnique.Name + ")");
						}
					}
				}
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

				if (Input.MouseState.IsLeftDoubleClick (gameTime)) {
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

	public class PipeModelCache : GameClass
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

	public class KnotModelCache : GameClass
	{
		// cache
		private Dictionary<Node, KnotModel> knotCache = new Dictionary<Node, KnotModel> ();

		public KnotModelCache (GameState state)
			: base(state)
		{
		}

		public KnotModel this [EdgeList edges, Edge edgeA, Edge edgeB, Vector3 offset] {
			get {
				Node node = edges.ToNode (edgeA);
				if (knotCache.ContainsKey (node)) {
					return knotCache [node];
				} else {
					KnotModel knot = new KnotModel (state, edges, edgeA, edgeB, edges.ToNode (edgeA).Vector (), 5f);
					knotCache [node] = knot;
					return knot;
				}
			}
		}
	}

}

