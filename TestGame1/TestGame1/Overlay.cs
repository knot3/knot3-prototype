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
	public class Overlay : GameClass
	{
		// graphics-related classes
		private SpriteBatch spriteBatch;

		// fonts
		private SpriteFont font;

		/// <summary>
		/// Initializes a new Overlay-
		/// </summary>
		public Overlay (GameState state)
			: base(state)
		{
			// create a new SpriteBatch, which can be used to draw textures
			spriteBatch = new SpriteBatch (graphics.GraphicsDevice);
		}

		/// <summary>
		/// Loads the content.
		/// </summary>
		public void LoadContent ()
		{
			// load fonts
			try {
				font = content.Load<SpriteFont> ("Font");
			} catch (ContentLoadException ex) {
				font = null;
				Console.WriteLine (ex.Message);
			}
		}

		/// <summary>
		/// Draw the Overlay.
		/// </summary>
		/// <param name='gameTime'>
		/// Game time.
		/// </param>
		public void Draw (GameTime gameTime)
		{
			DrawCoordinates (gameTime);
			DrawOverlay (gameTime);
			DrawFPS (gameTime);
		}
		
		public void Update (GameTime gameTime)
		{
			UpdateFPS (gameTime);
		}

		private void DrawCoordinates (GameTime gameTime)
		{
			int length = 2000;
			var vertices = new VertexPositionColor[6];
			vertices [0].Position = new Vector3 (-length, 0, 0);
			vertices [0].Color = Color.Green;
			vertices [1].Position = new Vector3 (+length, 0, 0);
			vertices [1].Color = Color.Green;
			vertices [2].Position = new Vector3 (0, -length, 0);
			vertices [2].Color = Color.Red;
			vertices [3].Position = new Vector3 (0, +length, 0);
			vertices [3].Color = Color.Red;
			vertices [4].Position = new Vector3 (0, 0, -length);
			vertices [4].Color = Color.Yellow;
			vertices [5].Position = new Vector3 (0, 0, +length);
			vertices [5].Color = Color.Yellow;
			graphics.GraphicsDevice.DrawUserPrimitives (PrimitiveType.LineList, vertices, 0, 3);
		}

		private void DrawOverlay (GameTime gameTime)
		{
			spriteBatch.Begin ();

			int height = 20;
			int width1 = 20, width2 = 150, width3 = 210, width4 = 270;
			DrawString ("Rotation: ", width1, height, Color.White);
			DrawString (camera.RotationAngle.Degrees.X, width2, height, Color.Green);
			DrawString (camera.RotationAngle.Degrees.Y, width3, height, Color.Red);
			DrawString (camera.RotationAngle.Degrees.Z, width4, height, Color.Yellow);
			height += 20;
			DrawString ("Camera Position: ", width1, height, Color.White);
			DrawVectorCoordinates (camera.Position, width2, width3, width4, height);
			height += 20;
			DrawString ("Camera Target: ", width1, height, Color.White);
			DrawVectorCoordinates (camera.Target, width2, width3, width4, height);
			height += 20;
			DrawString ("Distance: ", width1, height, Color.White);
			DrawString (camera.TargetDistance, width2, height, Color.White);
			height += 20;
			DrawString ("Selected Object: ", width1, height, Color.White);
			if (world.SelectedObject != null) {
				Vector3 selectedObjectCenter = world.SelectedObject.Center ();
				DrawVectorCoordinates (selectedObjectCenter, width2, width3, width4, height);
			}
			height += 20;
			DrawString ("Distance: ", width1, height, Color.White);
			DrawString (world.SelectedObjectDistance, width2, height, Color.White);
			height += 20;
			DrawString ("FoV: ", width1, height, Color.White);
			DrawString (camera.FoV, width2, height, Color.White);
			height += 20;
			DrawString ("WASD: ", width1, height, Color.White);
			string wasdMode =
					  input.WASDMode == WASDMode.ArcballMode ? "Arcball"
					: input.WASDMode == WASDMode.FirstPersonMode ? "FPS"
					: input.WASDMode == WASDMode.RotationMode ? "Rotation"
					: "unknown";
			DrawString (wasdMode, width2, height, Color.White);

			spriteBatch.End ();
		}

		private void DrawVectorCoordinates (Vector3 vector, int width2, int width3, int width4, int height)
		{
			DrawString ((int)vector.X, width2, height, Color.Green);
			DrawString ((int)vector.Y, width3, height, Color.Red);
			DrawString ((int)vector.Z, width4, height, Color.Yellow);
		}

		private void DrawString (string str, int width, int height, Color color)
		{
			if (font != null) {
				try {
					spriteBatch.DrawString (font, str, new Vector2 (width, height), color);

				} catch (ArgumentException exp) {
					Console.WriteLine (exp.ToString ());
				} catch (InvalidOperationException exp) {
					Console.WriteLine (exp.ToString ());
				}
			}
		}

		private void DrawString (float n, int width, int height, Color color)
		{
			DrawString ("" + n, width, height, color);
		}
		
		int _total_frames = 0;
		float _elapsed_time = 0.0f;
		int _fps = 0;

		private void UpdateFPS (GameTime gameTime)
		{
			_elapsed_time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

			if (_elapsed_time >= 1000.0f) {
				_fps = _total_frames;
				_total_frames = 0;
				_elapsed_time = 0;
			}
		}

		private void DrawFPS (GameTime gameTime)
		{
			_total_frames++;
			spriteBatch.Begin ();
			DrawString ("FPS: " + _fps, device.Viewport.Width - 150, 20, Color.White);
			spriteBatch.End ();
		}
	}
}

