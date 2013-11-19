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
	public abstract class MenuScreen : GameState
	{
		// graphics-related attributes
		private SpriteBatch spriteBatch;

		// colors
		private Color backColor = Color.Black;

		// ...
		private MousePointer pointer;
		protected GameState NextGameState;

		// lines
		protected List<Vector2> LinePoints;
		private int LineWidth;
		private Texture2D texture;

		public MenuScreen (Game game)
			: base(game)
		{
			LinePoints = new List<Vector2> ();
			LineWidth = 12;
			texture = Textures.Create (device, Color.White);
		}

		public override void Initialize ()
		{
			// create a new SpriteBatch, which can be used to draw textures
			spriteBatch = new SpriteBatch (graphics.GraphicsDevice);

			// input
			input = new StartScreenInput (this);
			input.SaveStates (null);

			// pointer
			pointer = new MousePointer (this);
		}

		public override GameState Update (GameTime gameTime)
		{
			NextGameState = this;

			// subclass...
			UpdateMenu (gameTime);

			// input
			input.Update (gameTime);
			input.SaveStates (gameTime);

			// pointer
			pointer.Update (gameTime);

			return NextGameState;
		}

		public abstract void UpdateMenu (GameTime gameTime);
		
		public override void Draw (GameTime gameTime)
		{
			PostProcessing.Begin (gameTime);

			graphics.GraphicsDevice.Clear (backColor);

			// subclass...
			DrawMenu (gameTime);

			DrawLines (gameTime);

			// pointer
			pointer.Draw (gameTime);

			PostProcessing.End (gameTime);
		}

		public abstract void DrawMenu (GameTime gameTime);

		public void DrawLines (GameTime gameTime)
		{
			if (LinePoints.Count >= 2) {
				Rectangle[] rects = new Rectangle[LinePoints.Count - 1];
				for (int i = 1; i < LinePoints.Count; ++i) {
					Vector2 nodeA = LinePoints [i - 1];
					Vector2 nodeB = LinePoints [i];
					if (nodeA.X == nodeB.X || nodeA.Y == nodeB.Y) {
						Vector2 direction = (nodeB - nodeA).PrimaryDirection ();
						Vector2 position = nodeA.Scale (viewport);
						int length = (int)(nodeB - nodeA).Scale (viewport).Length ();
						if (direction.X == 0 && direction.Y > 0) {
							rects [i - 1] = CreateRectangle (position.X, position.Y, 0, length);
						} else if (direction.X == 0 && direction.Y < 0) {
							rects [i - 1] = CreateRectangle (position.X, position.Y - length, 0, length);
						} else if (direction.Y == 0 && direction.X > 0) {
							rects [i - 1] = CreateRectangle (position.X, position.Y, length, 0);
						} else if (direction.Y == 0 && direction.X < 0) {
							rects [i - 1] = CreateRectangle (position.X - length, position.Y, length, 0);
						}
					}
				}
				spriteBatch.Begin ();
				foreach (Rectangle inner in rects) {
					Rectangle outer = new Rectangle (inner.X - 1, inner.Y - 1, inner.Width + 2, inner.Height + 2);
					spriteBatch.Draw (texture, outer, new Color (0x3b, 0x54, 0x00));
				}
				foreach (Rectangle rect in rects) {
					spriteBatch.Draw (texture, rect, new Color (0xb4, 0xff, 0x00));
				}
				spriteBatch.End ();
			}
		}

		private Rectangle CreateRectangle (float x, float y, float w, float h)
		{
			return new Rectangle ((int)x - LineWidth / 2, (int)y - LineWidth / 2, (int)w + LineWidth / 2, (int)h + LineWidth / 2);
		}

		protected void AddLinePoints (float startX, float startY, float[] xyxy)
		{
			Vector2 start = new Vector2 (startX, startY) / 1000f;
			LinePoints.Add (start);
			Vector2 current = start;
			for (int i = 0; i < xyxy.Count(); ++i) {
				// this is a new X value
				if (i % 2 == 0) 
					current.X = xyxy [i] / 1000f;
				// this is a new Y value
				else
					current.Y = xyxy [i] / 1000f;

				LinePoints.Add (current);
			}
		}

		public override void Unload ()
		{
		}

		protected Color BackgroundColor (MenuItemState itemState)
		{
			switch (itemState) {
			case MenuItemState.Selected:
				return Color.Black*0f;
			case MenuItemState.Normal:
			default:
				return Color.Black*0f;
			}
		}

		protected Color ForegroundColor (MenuItemState itemState)
		{
			switch (itemState) {
			case MenuItemState.Selected:
				return Color.White;
			case MenuItemState.Normal:
			default:
				return Color.White * 0.7f;
			}
		}
	}
}

