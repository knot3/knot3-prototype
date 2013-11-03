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
	public class DrawLines
	{
		// graphics-related classes
		private GraphicsDeviceManager graphics;

		// custom classes
		private Game game;
		private Camera camera;

		/// <summary>
		/// Initializes a new Overlay-
		/// </summary>
		public DrawLines (Camera camera, GraphicsDeviceManager graphics, Game game)
		{
			this.camera = camera;
			this.graphics = graphics;
			this.game = game;
		}

		/// <summary>
		/// Draw the lines.
		/// </summary>
		/// <param name='gameTime'>
		/// Game time.
		/// </param>
		public void Draw (LineList lines, GameTime gameTime)
		{
			if (lines.Count > 0) {
				DrawRoundedLines (lines);
			}
		}

		private void DrawRoundedLines (LineList lines)
		{
			Vector3 offset = new Vector3 (10, 10, 10);

			var vertices = new VertexPositionColor[lines.Count * 4];

			Vector3 last = new Vector3 (0, 0, 0);
			for (int n = 0; n < lines.Count; n++) {
				Vector3 p1 = lines [n].From.Vector () + offset;
				Vector3 p2 = lines [n].To.Vector () + offset;

				var diff = p1 - p2;
				diff.Normalize ();
				p1 = p1 - 10 * diff;
				p2 = p2 + 10 * diff;

				vertices [4 * n + 0].Position = n == 0 ? p1 : last;
				vertices [4 * n + 1].Position = p1;
				vertices [4 * n + 2].Position = p1;
				vertices [4 * n + 3].Position = p2;

				//Console.WriteLine (vertices [4 * n + 2]);
				last = p2;
			}
			for (int n = 0; n < lines.Count*4; n++) {
				if (n % 4 >= 2) {
					vertices [n].Color = Color.White;
				} else {
					vertices [n].Color = Color.Wheat;
				}
			}
			for (int n = 0; n < lines.Count; n++) {
				vertices [4 * n + 2].Color = lines.Color (n);
				vertices [4 * n + 3].Color = lines.Color (n);
			}
			graphics.GraphicsDevice.DrawUserPrimitives (PrimitiveType.LineList, vertices, 0, lines.Count * 2); 
		}
	}
}

