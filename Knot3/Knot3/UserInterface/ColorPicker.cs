using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Knot3.Core;
using Knot3.Utilities;

namespace Knot3.UserInterface
{
	public class ColorPicker : Widget, IMouseEventListener
	{
		// colors
		private List<Color> colors;
		private List<Vector2> tiles;
		private static readonly Vector2 tileSize = new Vector2 (0.032f, 0.032f);

		public Color SelectedColor { get; private set; }

		public Action<Color> OnSelectColor { get; set; }

		// textures
		protected SpriteBatch spriteBatch;

		public ColorPicker (GameScreen screen, WidgetInfo info, DisplayLayer drawOrder)
		: base(screen, info, drawOrder)
		{
			info.BackgroundColor = () => Color.Black;
			info.ForegroundColor = () => Color.White;
			info.AlignX = HorizontalAlignment.Left;
			info.AlignY = VerticalAlignment.Top;
			// colors
			colors = new List<Color> (CreateColors (64));
			colors.Sort (Utilities.ColorHelper.SortColorsByLuminance);
			tiles = new List<Vector2> (CreateTiles (colors));

			// create a new SpriteBatch, which can be used to draw textures
			spriteBatch = new SpriteBatch (screen.device);

			info.RelativePosition = () => (Vector2.One - info.RelativeSize ()) / 2;
			info.RelativeSize = () => {
				float sqrt = (float)Math.Ceiling (Math.Sqrt (colors.Count));
				return tileSize * sqrt;
			};
		}

		public override void Draw (GameTime time)
		{
			if (IsVisible) {
				spriteBatch.Begin ();

				// background
				Rectangle rect = Info.ScaledRectangle (screen.viewport);
				spriteBatch.Draw (
				    TextureHelper.Create (screen.device, Color.Black), rect.Grow (2), Color.White
				);

				// color tiles
				int i = 0;
				foreach (Vector2 tile in tiles) {
					rect = HfGDesign.CreateRectangle (
					           Info.ScaledPosition (screen.viewport) + tile.Scale (screen.viewport),
					           tileSize.Scale (screen.viewport)
					       );
					spriteBatch.Draw (
					    TextureHelper.Create (screen.device, colors [i]), rect.Shrink (1), Color.White
					);

					++i;
				}

				spriteBatch.End ();
			}
		}

		private static IEnumerable<Color> CreateColors (int num)
		{
			float steps = (float)Math.Pow (num, 1.0 / 3.0);
			int n = 0;
			for (int r = 0; r < steps; ++r) {
				for (int g = 0; g < steps; ++g) {
					for (int b = 0; b < steps; ++b) {
						yield return new Color (new Vector3 (r, g, b) / steps);
						++n;
					}
				}
			}
		}

		private static IEnumerable<Vector2> CreateTiles (IEnumerable<Color> _colors)
		{
			Color[] colors = _colors.ToArray ();
			float sqrt = (float)Math.Sqrt (colors.Count ());
			int row = 0;
			int column = 0;
			foreach (Color color in colors) {
				yield return new Vector2 (tileSize.X * column, tileSize.Y * row);

				++column;
				if (column >= sqrt) {
					column = 0;
					++row;
				}
			}
		}

		private void SelectColor (Color color)
		{
			SelectedColor = color;
			OnSelectColor (color);
			IsVisible = false;
		}

		public void OnLeftClick (Vector2 position, ClickState click, GameTime time)
		{
			position = position.RelativeTo (screen.viewport);
			Console.WriteLine ("ColorPicker.OnLeftClick: positon=" + position);
			int i = 0;
			foreach (Vector2 tile in tiles) {
				Console.WriteLine ("ColorPicker: tile=" + tile + "  "
				                   + (tile.X <= position.X) + " " + (tile.X + tileSize.X > position.X) + " " + (
				                       tile.Y <= position.Y) + " " + (tile.Y + tileSize.Y > position.Y)
				                  );
				if (tile.X <= position.X && tile.X + tileSize.X > position.X
				        && tile.Y <= position.Y && tile.Y + tileSize.Y > position.Y) {
					Console.WriteLine ("ColorPicker: color=" + colors [i]);

					SelectColor (colors [i]);
				}
				++i;
			}
		}

		public void OnRightClick (Vector2 position, ClickState click, GameTime time)
		{
		}

		public void SetHovered (bool hovered)
		{
		}

		public bool IsMouseEventEnabled { get { return IsVisible; } }
	}
}
