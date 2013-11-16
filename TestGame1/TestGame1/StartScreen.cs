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
	public class StartScreen : MenuScreen
	{
		// menu
		protected Menu menu;

		// textures
		private Texture2D logo;
		private SpriteBatch spriteBatch;

		public StartScreen (Game game)
			: base(game)
		{
			menu = new Menu (this);
		}
		
		public override void Initialize ()
		{
			base.Initialize ();

			// logo
			logo = GameObject.LoadTexture (content, "logo");

			// create a new SpriteBatch, which can be used to draw textures
			spriteBatch = new SpriteBatch (device);

			// menu
			menu.Initialize ();
			menu.Add ("Creative", Keys.Space, () => NextGameState = GameStates.KnotMode,
			          0.700f, 0.250f, 0.960f, 0.380f, HorizontalAlignment.Center);
			menu.Add ("Challenge", Keys.RightWindows, () => NextGameState = GameStates.KnotMode,
			          0.000f, 0.050f, 0.380f, 0.190f, HorizontalAlignment.Center);
			menu.Add ("Options", Keys.O, () => game.Exit (),
			          0.260f, 0.840f, 0.480f, 0.950f, HorizontalAlignment.Center);
			menu.Add ("Exit", Keys.Escape, () => game.Exit (),
			          0.800f, 0.535f, 0.980f, 0.790f, HorizontalAlignment.Center);

			// lines
			AddLinePoints (0, 50, new float[]{
				380, 250, 960, 380, 700, 160, 1000
			}
			);
			AddLinePoints (0, 190, new float[]{
				620, 855, 800, 535, 980, 790,
				480, 950, 260, 840,
				520, 1000
			}
			);
		}
		
		public override void UpdateMenu (GameTime gameTime)
		{
			// menu
			menu.Update (gameTime);
		}
		
		public override void DrawMenu (GameTime gameTime)
		{
			// logo
			spriteBatch.Begin ();
			spriteBatch.Draw(logo, new Rectangle(50, 380, 500, 300).Scale(viewport), Color.White);
			spriteBatch.End ();

			// menu
			menu.Draw (gameTime);
		}
	}
}

