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
		public void Update (LineList lines, GameTime gameTime)
		{
			if (lines.Count > 0) {
				pipes.UpdatePipes (lines, gameTime);
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

		public void UpdatePipes (LineList lines, GameTime gameTime)
		{
			pipes.Clear ();

			for (int n = 0; n < lines.Count; n++) {
				Vector3 p1 = lines [n].From.Vector () + Position;
				Vector3 p2 = lines [n].To.Vector () + Position;

				Pipe pipe = new Pipe (game, p1, p2, 10);
				pipes.Add (pipe);
			}
		}
		
		public override void DrawObject (GameTime gameTime)
		{
			foreach (Pipe pipe in pipes) {
				pipe.Draw (gameTime);
			}
		}

		public override Nullable<float> Intersects (Ray ray)
		{
			return null;
		}

		public override Vector3 Center ()
		{
			return Position;
		}
	}
	
	public class Pipe : GameModel
	{
		public Pipe (Game game, Vector3 posFrom, Vector3 posTo, float scale)
			: base(game, "pipe1", posFrom + (posTo-posFrom)/2, scale)
		{
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

		public override void UpdateEffect (BasicEffect effect)
		{
			if (game.World.SelectedObject == this) {
				effect.FogEnabled = true;
				effect.FogColor = Color.CornflowerBlue.ToVector3 (); // For best results, ake this color whatever your background is.
				effect.FogStart = 9.75f;
				effect.FogEnd = 10.25f;
			}
		}

		public override void DrawObject (GameTime gameTime)
		{
			base.DrawObject (gameTime);
		}
	}
}

