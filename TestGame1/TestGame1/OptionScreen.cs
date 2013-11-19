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
	public class OptionScreen : MenuScreen
	{
		// menu
		private VerticalMenu menu;

		// textures
		private SpriteBatch spriteBatch;

		public OptionScreen (Game game)
			: base(game)
		{
			menu = new VerticalMenu (this);
		}
		
		public override void Initialize ()
		{
			base.Initialize ();

			// create a new SpriteBatch, which can be used to draw textures
			spriteBatch = new SpriteBatch (device);

			// menu
			menu.Initialize (ForegroundColor, BackgroundColor, HAlign.Left);
			MenuItemInfo info;
			info = new MenuItemInfo (text: "Video", onClick: () => NextGameState = GameStates.VideoOptionScreen);
			menu.AddButton (info.AddKey (Keys.V));
			info = new MenuItemInfo (text: "Audio", onClick: () => NextGameState = GameStates.OptionScreen);
			menu.AddButton (info.AddKey (Keys.A));
			info = new MenuItemInfo (text: "Controls", onClick: () => NextGameState = GameStates.OptionScreen);
			menu.AddButton (info.AddKey (Keys.C));
			info = new MenuItemInfo (text: "Knots", onClick: () => NextGameState = GameStates.OptionScreen);
			menu.AddButton (info.AddKey (Keys.K));
			info = new MenuItemInfo (text: "Back", onClick: () => NextGameState = GameStates.StartScreen);
			menu.AddButton (info.AddKey (Keys.Escape));

			// lines
			AddLinePoints (0, 50, new float[]{
				30, 970, 970, 50, 1000
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
			spriteBatch.Begin ();

			// text
			spriteBatch.DrawString (menu.Font, "Options", new Vector2 (0.050f, 0.050f).Scale (viewport), Color.White,
					0, Vector2.Zero, 0.25f * viewport.ScaleFactor ().Length (), SpriteEffects.None, 0);

			// menu
			menu.Align (viewport, 1f, 100, 180, 0, 60);
			menu.Draw (0f, spriteBatch, gameTime);

			spriteBatch.End ();
		}
	}
}

