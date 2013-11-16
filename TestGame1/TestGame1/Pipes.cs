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

namespace TestGame1
{
	public class DrawPipes : GameClass
	{
		// pipes
		private Pipes pipes;

		public DrawPipes (GameState state)
			: base(state)
		{
			pipes = new Pipes (state);
			world.Add (pipes);
		}

		/// <summary>
		/// Draw the pipes.
		/// </summary>
		/// <param name='gameTime'>
		/// Game time.
		/// </param>
		public void Update (EdgeList lines)
		{
			if (lines.Count > 0) {
				pipes.UpdatePipes (lines);
			}
		}
		
		public void Draw (GameTime gameTime)
		{
			pipes.Draw (gameTime);
		}
	}

	public class Pipes : GameObject
	{
		// pipes and knots
		private List<PipeModel> pipes;
		private List<KnotModel> knots;
		private PipeModelCache pipeCache;
		private KnotModelCache knotCache;

		protected override Vector3 Position { get; set; }

		public Pipes (GameState state)
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

		public void UpdatePipes (EdgeList edges)
		{
			pipes.Clear ();
			for (int n = 0; n < edges.Count; n++) {
				PipeModel pipe = pipeCache [edges, edges [n], Position];
				pipe.OnDataChange = () => UpdatePipes (edges);
				pipes.Add (pipe);
			}

			knots.Clear ();
			for (int n = 0; n < edges.Count; n++) {
				KnotModel knot = knotCache [edges, edges [n], edges [n + 1], Position];
				// knot.OnDataChange = () => UpdatePipes(lines);
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
		}

		public override void DrawObject (GameTime gameTime)
		{
			Vector3 mix = (EdgeA.Color.ToVector3 () + EdgeB.Color.ToVector3 ()) / 2;
			foreach (ModelMesh mesh in ModelMeshes) {
				foreach (BasicEffect effect in mesh.Effects) {
					effect.DiffuseColor = mix;
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
		}

		public override void OnSelected (GameTime gameTime)
		{
		}

		public override void UpdateEffect (BasicEffect effect, GameTime gameTime)
		{
			float distance = (Center () - camera.Position).Length ();
			if (world.SelectedObject == this) {
				effect.FogEnabled = true;
				float fogIntensity = (gameTime.TotalGameTime.Milliseconds % 1000 - 500) / 5;
				if (fogIntensity < 0)
					fogIntensity = 0 - fogIntensity;
				effect.FogColor = Color.White.ToVector3 (); // For best results, ake this color whatever your background is.
				effect.FogStart = distance - 100 - fogIntensity;
				effect.FogEnd = distance + 200 - fogIntensity;
			} else if (Edges.SelectedEdges.Contains (Edge)) {
				effect.FogEnabled = false;
				float fogIntensity = (gameTime.TotalGameTime.Milliseconds % 1000 - 500) / 5;
				if (fogIntensity < 0)
					fogIntensity = 0 - fogIntensity;
				effect.FogColor = Color.White.ToVector3 (); // For best results, ake this color whatever your background is.
				effect.FogStart = distance - 100 - 100;
				effect.FogEnd = distance + 200 - 100;
			} else {
				effect.FogEnabled = false;
			}
		}

		public override void DrawObject (GameTime gameTime)
		{
			foreach (ModelMesh mesh in ModelMeshes) {
				foreach (BasicEffect effect in mesh.Effects) {
					if (Edges.SelectedEdges.Contains (Edge)) {
						effect.DiffuseColor = Color.White.ToVector3();
					} else {
						effect.DiffuseColor = Edge.Color.ToVector3 ();
					}
				}
			}
			base.DrawObject (gameTime);
		}

		private Vector3 previousMousePosition = Vector3.Zero;

		public override void Update (GameTime gameTime)
		{
			// check whether this object is selected
			if (IsSelected () == true) {

				// if the left mouse button is pressed, select the edge
				if (Input.MouseState.IsLeftClick (gameTime)) {
					try {
						if (Keys.LeftControl.IsHeldDown ()) {
							List<Edge> selection = Edges.SelectedEdges;
							selection.Add (Edge);
							Edges.SelectedEdges = selection;
						} else if (Keys.LeftShift.IsHeldDown ()) {
							List<Edge> selection = Edges.SelectedEdges;
							selection.Add (Edge);
							Edges.SelectedEdges = selection;
						} else {
							Edges.SelectedEdges = new List<Edge> (){Edge};
						}
					} catch (ArgumentOutOfRangeException exp) {
						Console.WriteLine (exp.ToString ());
					}
				}

				// change color?
				if (Keys.C.IsDown ()) {
					Edge.Color = Node.RandomColor ();
				}
				if (Input.MouseState.IsLeftDoubleClick (gameTime)) {
				}

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
		}

		private void Move ()
		{
			Vector3 currentMousePosition = device.Viewport.Unproject (new Vector3 (Input.MouseState.ToVector2 (), 1f),
								camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);

			Vector3 mouseMove = currentMousePosition - previousMousePosition;
			Console.WriteLine ("mouseMove=" + mouseMove);
			Vector3 direction3D = mouseMove.PrimaryDirection ();
			if (direction3D != Vector3.Zero && mouseMove.Length ().Abs () > 30) {
				try {
					Edges.InsertAt (Edges.SelectedEdges, direction3D);
				} catch (ArgumentOutOfRangeException exp) {
					Console.WriteLine (exp.ToString ());
				}
			}
		}

		/*public override GameObjectDistance Intersects (Ray ray)
		{
			Vector3 min = PosFrom;// - 10*Vector3.Cross (Direction, camera.TargetVector);
			Vector3 max = PosTo;// + 10*Vector3.Cross (Direction, camera.TargetVector);
			BoundingBox bounds = new BoundingBox (min, max);
			Console.WriteLine("bounds: "+bounds);
			Nullable<float> distance = ray.Intersects (bounds);
			if (distance != null) {
				GameObjectDistance intersection = new GameObjectDistance () {
						Object=this, Distance=distance.Value
					};
				return intersection;
			}
			return null;
		}*/
	}

	public class PipeModelCache : GameClass
	{
		// cache
		private Dictionary<Edge, PipeModel> pipeCache = new Dictionary<Edge, PipeModel> ();

		public PipeModelCache (GameState state)
			: base(state)
		{
		}

		public PipeModel this [EdgeList edges, Edge edge, Vector3 offset] {
			get {
				if (pipeCache.ContainsKey (edge)) {
					return pipeCache [edge];
				} else {
					Vector3 p1 = edge.FromNode.Vector () + offset;
					Vector3 p2 = edge.ToNode.Vector () + offset;

					PipeModel pipe = new PipeModel (state, edges, edge, p1, p2, 10f);
					pipeCache [edge] = pipe;
					return pipe;
				}
			}
		}
	}

	public class KnotModelCache : GameClass
	{
		// cache
		private Dictionary<Edge, KnotModel> knotCache = new Dictionary<Edge, KnotModel> ();

		public KnotModelCache (GameState state)
			: base(state)
		{
		}

		public KnotModel this [EdgeList edges, Edge edgeA, Edge edgeB, Vector3 offset] {
			get {
				if (knotCache.ContainsKey (edgeA)) {
					return knotCache [edgeA];
				} else {
					KnotModel knot = new KnotModel (state, edges, edgeA, edgeB, edgeA.ToNode.Vector (), 5f);
					knotCache [edgeA] = knot;
					return knot;
				}
			}
		}
	}

}

