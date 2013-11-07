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

		public DrawPipes (Game game)
			: base(game)
		{
			pipes = new Pipes (game);
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
		// pipes
		private List<Pipe> pipes;

		protected override Vector3 Position { get; set; }

		public Pipes (Game game)
			: base(game)
		{
			pipes = new List<Pipe> ();
			Position = Vector3.Zero; //new Vector3 (10, 10, 10);
		}

		public override void Update (GameTime gameTime)
		{
			for (int i = 0; i < pipes.Count; ++i) {
				Pipe pipe = pipes [i];
				pipe.Update (gameTime);
			}
		}

		public void UpdatePipes (LineList lines)
		{
			pipes.Clear ();

			for (int n = 0; n < lines.Count; n++) {
				Vector3 p1 = lines [n].From.Vector () + Position;
				Vector3 p2 = lines [n].To.Vector () + Position;

				Pipe pipe = new Pipe (game, lines, n, p1, p2, 10);
				pipes.Add (pipe);
			}
			if (world.SelectedObject is Pipe)
				world.SelectedObject = pipes [lines.SelectedLine];
		}
		
		public override void DrawObject (GameTime gameTime)
		{
			foreach (Pipe pipe in pipes) {
				pipe.Draw (gameTime);
			}
		}

		public override GameObjectDistance Intersects (Ray ray)
		{
			GameObjectDistance nearest = null;
			if (!input.GrabMouseMovement) {
				foreach (Pipe pipe in pipes) {
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
	
	public class Pipe : GameModel
	{
		private LineList Lines;
		private int LineNumber;
		private Vector3 PosFrom;
		private Vector3 PosTo;
		private Vector3 Direction;

		public Pipe (Game game, LineList lines, int lineNumber, Vector3 posFrom, Vector3 posTo, float scale)
			: base(game, "pipe1", posFrom + (posTo-posFrom)/2, scale)
		{
			Lines = lines;
			LineNumber = lineNumber;
			PosFrom = posFrom;
			PosTo = posTo;

			Direction = posTo - posFrom;
			Direction.Normalize ();

			if (Direction.Y == 1) {
				Rotation.X = MathHelper.ToRadians (90);
			} else if (Direction.Y == -1) {
				Rotation.X = MathHelper.ToRadians (270);
			}
			if (Direction.X == 1) {
				Rotation.Y = MathHelper.ToRadians (90);
			} else if (Direction.X == -1) {
				Rotation.Y = MathHelper.ToRadians (270);
			}
		}

		public virtual void OnSelected ()
		{
			Lines.SelectedLine = LineNumber;
		}

		public override void UpdateEffect (BasicEffect effect)
		{
			if (world.SelectedObject == this) {
				effect.FogEnabled = true;
				effect.FogColor = Color.Chartreuse.ToVector3 (); // For best results, ake this color whatever your background is.
				effect.FogStart = world.SelectedObjectDistance - 100;
				effect.FogEnd = world.SelectedObjectDistance + 200;
			} else {
				effect.FogEnabled = false;
			}
		}

		public override void DrawObject (GameTime gameTime)
		{
			base.DrawObject (gameTime);
		}

		public override void Update (GameTime gameTime)
		{
			// check whether is object is movable and whether it is selected
			if (IsSelected() == true) {
				// is SelectedObjectMove the current input action?
				if (input.CurrentInputAction == InputAction.SelectedObjectMove) {
					Plane groundPlane = CurrentGroundPlane ();
					Ray ray = CurrentMouseRay ();
					Vector3? mousePosition = CurrentMousePosition (ray, groundPlane);
					if (mousePosition.HasValue) {
						Vector3 move = mousePosition.Value - Position;
						Console.WriteLine ("length=" + move.Length ().Abs ()
							+ ", move=" + move
						);
						if (move.Length ().Abs () > 20) {
							Vector3 direction = move.PrimaryDirectionExcept (Direction);
							Lines.InsertAt (LineNumber, direction);
						}
					}
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

}

