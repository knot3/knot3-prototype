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

using Knot3.Core;
using Knot3.Utilities;

namespace Knot3.UserInterface
{
	public static class HfGDesign
	{
		public static Color LineColor = new Color (0xb4, 0xff, 0x00);
		public static Color OutlineColor = new Color (0x3b, 0x54, 0x00);
		private static Texture2D texture;
		private static SpriteFont menuFont;

		public static SpriteFont MenuFont (GameScreen screen)
		{
			if (menuFont != null) {
				return menuFont;
			}
			else {
				// load fonts
				try {
					menuFont = screen.content.Load<SpriteFont> ("MenuFont");
				}
				catch (ContentLoadException ex) {
					menuFont = null;
					Console.WriteLine (ex.Message);
				}
				return menuFont;
			}
		}

		public static void DrawLines (ref List<Vector2> linePoints, int lineWidth, SpriteBatch spriteBatch, GameScreen screen, GameTime time)
		{
			lineWidth = (int)new Vector2 (lineWidth, lineWidth).Scale (screen.viewport).X;
			if (texture == null) {
				texture = TextureHelper.Create (screen.device, Color.White);
			}

			if (linePoints.Count >= 2) {
				Rectangle[] rects = new Rectangle[linePoints.Count - 1];
				for (int i = 1; i < linePoints.Count; ++i) {
					Vector2 nodeA = linePoints [i - 1];
					Vector2 nodeB = linePoints [i];
					if (nodeA.X == nodeB.X || nodeA.Y == nodeB.Y) {
						Vector2 direction = (nodeB - nodeA).PrimaryDirection ();
						Vector2 position = nodeA.Scale (screen.viewport);
						int length = (int)(nodeB - nodeA).Scale (screen.viewport).Length ();
						if (direction.X == 0 && direction.Y > 0) {
							rects [i - 1] = CreateRectangle (lineWidth, position.X, position.Y, 0, length);
						}
						else if (direction.X == 0 && direction.Y < 0) {
							rects [i - 1] = CreateRectangle (lineWidth, position.X, position.Y - length, 0, length);
						}
						else if (direction.Y == 0 && direction.X > 0) {
							rects [i - 1] = CreateRectangle (lineWidth, position.X, position.Y, length, 0);
						}
						else if (direction.Y == 0 && direction.X < 0) {
							rects [i - 1] = CreateRectangle (lineWidth, position.X - length, position.Y, length, 0);
						}
					}
				}
				foreach (Rectangle inner in rects) {
					Rectangle outer = new Rectangle (inner.X - 1, inner.Y - 1, inner.Width + 2, inner.Height + 2);
					spriteBatch.Draw (texture, outer, OutlineColor);
				}
				foreach (Rectangle rect in rects) {
					spriteBatch.Draw (texture, rect, LineColor);
				}
			}
		}

		public static Rectangle CreateRectangle (int lineWidth, float x, float y, float w, float h)
		{
			if (w == 0) {
				return new Rectangle ((int)x - lineWidth / 2, (int)y - lineWidth / 2, lineWidth, (int)h + lineWidth);
			}
			else if (h == 0) {
				return new Rectangle ((int)x - lineWidth / 2, (int)y - lineWidth / 2, (int)w + lineWidth, lineWidth);
			}
			else {
				return new Rectangle ((int)x, (int)y, (int)w, (int)h);
			}
		}

		public static Rectangle CreateRectangle (int lineWidth, Vector2 topLeft, Vector2 size)
		{
			return CreateRectangle (lineWidth, topLeft.X, topLeft.Y, size.X, size.Y);
		}

		public static Rectangle CreateRectangle (Vector2 topLeft, Vector2 size)
		{
			return CreateRectangle (0, topLeft.X, topLeft.Y, size.X, size.Y);
		}

		public static void AddLinePoints (ref List<Vector2> linePoints, float startX, float startY, params float[] xyxy)
		{
			Vector2 start = new Vector2 (startX, startY);
			if (start.X > 1 || start.Y > 1) {
				start /= 1000f;
			}
			linePoints.Add (start);
			Vector2 current = start;
			for (int i = 0; i < xyxy.Count(); ++i) {
				// this is a new X value
				if (i % 2 == 0) {
					current.X = xyxy [i] > 1 ? xyxy [i] / 1000f : xyxy [i];
				}
				// this is a new Y value
				else {
					current.Y = xyxy [i] > 1 ? xyxy [i] / 1000f : xyxy [i];
				}

				linePoints.Add (current);
			}
		}
	}
}
