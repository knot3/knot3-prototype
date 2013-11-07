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
			game.World.Add (pipes);
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
			Position = new Vector3 (10, 10, 10);
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
			if (!game.Input.GrabMouseMovement) {
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

		public Pipe (Game game, LineList lines, int lineNumber, Vector3 posFrom, Vector3 posTo, float scale)
			: base(game, "pipe1", posFrom + (posTo-posFrom)/2, scale)
		{
			Lines = lines;
			LineNumber = lineNumber;

			Vector3 direction = posTo - posFrom;
			direction.Normalize ();

			if (direction.Y == 1) {
				Rotation.X = MathHelper.ToRadians (90);
			} else if (direction.Y == -1) {
				Rotation.X = MathHelper.ToRadians (270);
			}
			if (direction.X == 1) {
				Rotation.Y = MathHelper.ToRadians (90);
			} else if (direction.X == -1) {
				Rotation.Y = MathHelper.ToRadians (270);
			}
		}

		public override bool Selected {
			get {
				return base.Selected;
			}
			set {
				base.Selected = value;
				if (value == true) {
					Lines.SelectedLine = LineNumber;
				}
			}
		}

		public override void UpdateEffect (BasicEffect effect)
		{
			if (game.World.SelectedObject == this) {
				effect.FogEnabled = true;
				effect.FogColor = Color.Chartreuse.ToVector3 (); // For best results, ake this color whatever your background is.
				effect.FogStart = game.World.SelectedObjectDistance - 100;
				effect.FogEnd = game.World.SelectedObjectDistance + 100;
			} else {
				effect.FogEnabled = false;
			}
		}

		public override void DrawObject (GameTime gameTime)
		{
			base.DrawObject (gameTime);
		}
	}
}

