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
		public void Update (LineList lines)
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

		public void UpdatePipes (LineList lines)
		{
			pipes.Clear ();
			for (int n = 0; n < lines.Count; n++) {
				PipeModel pipe = pipeCache [lines, lines [n], Position];
				pipe.OnDataChange = () => UpdatePipes (lines);
				pipes.Add (pipe);
			}

			knots.Clear ();
			for (int n = 0; n < lines.Count; n++) {
				KnotModel knot = knotCache [lines, lines [n], lines [n + 1], Position];
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
		private LineList Lines;
		private Line LineA;
		private Line LineB;

		public KnotModel (GameState state, LineList lines, Line lineA, Line lineB, Vector3 position, float scale)
			: base(state, "knot1", position, scale)
		{
			Lines = lines;
			LineA = lineA;
			LineB = lineB;
		}

		public override void DrawObject (GameTime gameTime)
		{
			Vector3 mix = (LineA.Color.ToVector3 () + LineB.Color.ToVector3 ()) / 2;
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
		private LineList Lines;
		private Line Line;
		private Vector3 PosFrom;
		private Vector3 PosTo;
		private Vector3 Direction;
		public Action OnDataChange = () => {};

		public PipeModel (GameState state, LineList lines, Line line, Vector3 posFrom, Vector3 posTo, float scale)
			: base(state, "pipe1", posFrom + (posTo-posFrom)/2, scale)
		{
			Lines = lines;
			Line = line;
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

		public override void OnSelected ()
		{
			try {
				Lines.SelectedLine = Lines [Line];
			} catch (ArgumentOutOfRangeException exp) {
				Console.WriteLine (exp.ToString ());
			}
		}

		public override void UpdateEffect (BasicEffect effect)
		{
			if (world.SelectedObject == this) {
				effect.FogEnabled = true;
				effect.FogColor = Color.White.ToVector3 (); // For best results, ake this color whatever your background is.
				effect.FogStart = world.SelectedObjectDistance - 100;
				effect.FogEnd = world.SelectedObjectDistance + 200;
			} else {
				effect.FogEnabled = false;
			}
		}

		public override void DrawObject (GameTime gameTime)
		{
			foreach (ModelMesh mesh in ModelMeshes) {
				foreach (BasicEffect effect in mesh.Effects) {
					effect.DiffuseColor = Line.Color.ToVector3 ();
				}
			}
			base.DrawObject (gameTime);
		}

		private Vector3 previousMousePosition = Vector3.Zero;

		public override void Update (GameTime gameTime)
		{
			// check whether this object is selected
			if (IsSelected () == true) {
				// change color?
				if (Keys.C.IsDown ()) {
					System.Random r = new System.Random ();
					Line.Color = new Color ((float)r.NextDouble (), (float)r.NextDouble (), (float)r.NextDouble ());
				}
				// is SelectedObjectMove the current input action?
				if (input.CurrentInputAction == InputAction.SelectedObjectMove) {
					if (previousMousePosition == Vector3.Zero) {
						previousMousePosition = device.Viewport.Unproject (new Vector3 (input.MouseState.ToVector2 (), 1f),
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
			Vector3 currentMousePosition = device.Viewport.Unproject (new Vector3 (input.MouseState.ToVector2 (), 1f),
								camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);

			Vector3 mouseMove = currentMousePosition - previousMousePosition;
			Console.WriteLine ("mouseMove=" + mouseMove);
			Vector3 direction3D = mouseMove.PrimaryDirection ();
			if (direction3D != Vector3.Zero && mouseMove.Length ().Abs () > 10) {
				try {
					Lines.InsertAt (Lines [Line], direction3D);
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
		private Dictionary<Line, PipeModel> pipeCache = new Dictionary<Line, PipeModel> ();

		public PipeModelCache (GameState state)
			: base(state)
		{
		}

		public PipeModel this [LineList lines, Line line, Vector3 offset] {
			get {
				if (pipeCache.ContainsKey (line)) {
					return pipeCache [line];
				} else {
					Vector3 p1 = line.From.Vector () + offset;
					Vector3 p2 = line.To.Vector () + offset;

					PipeModel pipe = new PipeModel (state, lines, line, p1, p2, 10f);
					pipeCache [line] = pipe;
					return pipe;
				}
			}
		}
	}

	public class KnotModelCache : GameClass
	{
		// cache
		private Dictionary<Line, KnotModel> knotCache = new Dictionary<Line, KnotModel> ();

		public KnotModelCache (GameState state)
			: base(state)
		{
		}

		public KnotModel this [LineList lines, Line lineA, Line lineB, Vector3 offset] {
			get {
				if (knotCache.ContainsKey (lineA)) {
					return knotCache [lineA];
				} else {
					KnotModel knot = new KnotModel (state, lines, lineA, lineB, lineA.To.Vector (), 5f);
					knotCache [lineA] = knot;
					return knot;
				}
			}
		}
	}

}

