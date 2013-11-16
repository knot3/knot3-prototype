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
	public class DrawLines : GameClass
	{
		public DrawLines (GameState state)
			: base(state)
		{
		}

		/// <summary>
		/// Draw the lines.
		/// </summary>
		/// <param name='gameTime'>
		/// Game time.
		/// </param>
		public void Draw (EdgeList edges, GameTime gameTime)
		{
			if (edges.Count > 0) {
				DrawRoundedLines (edges);
			}
		}

		private void DrawRoundedLines (EdgeList lines)
		{
			Vector3 offset = Vector3.Zero; //new Vector3 (10, 10, 10);

			var vertices = new VertexPositionColor[lines.Count * 4];

			Vector3 last = new Vector3 (0, 0, 0);
			for (int n = 0; n < lines.Count; n++) {
				Vector3 p1 = lines [n].FromNode.Vector () + offset;
				Vector3 p2 = lines [n].ToNode.Vector () + offset;

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
			for (int n = 0; n < lines.Count; n++) {
				vertices [4 * n + 0].Color = Color.Black;
				vertices [4 * n + 1].Color = Color.Black;
				vertices [4 * n + 2].Color = Color.Black;
				vertices [4 * n + 3].Color = Color.Black;
			}
			graphics.GraphicsDevice.DrawUserPrimitives (PrimitiveType.LineList, vertices, 0, lines.Count * 2); 
		}
	}
}

