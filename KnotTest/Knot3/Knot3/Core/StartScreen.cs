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

using Knot3.UserInterface;
using Knot3.Utilities;

namespace Knot3.Core
{
	/// <summary>
	/// Der Screen, der beim Starten des Spiels geladen wird.
	/// </summary>
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
			menu = new Menu (this, new WidgetInfo (), DisplayLayer.Menu);
		}
		
		public override void Initialize ()
		{
			base.Initialize ();

			// logo
			logo = Textures.LoadTexture (content, "logo");

			// create a new SpriteBatch, which can be used to draw textures
			spriteBatch = new SpriteBatch (device);

			// menu
			menu.ItemForegroundColor = ForegroundColor;
			menu.ItemBackgroundColor = BackgroundColor;
			menu.ItemAlignX = HAlign.Center;
			menu.ItemAlignY = VAlign.Center;

			menu.AddButton (new MenuItemInfo (
				text: "Creative", left: 0.700f, top: 0.250f, right: 0.960f, bottom: 0.380f,
			    onClick: () => NextState = GameStates.LoadSavegameScreen).AddKey (Keys.Space)
			);
			menu.AddButton (new MenuItemInfo (
				text: "Challenge", left: 0.000f, top: 0.050f, right: 0.380f, bottom: 0.190f,
			    onClick: () => NextState = GameStates.CreativeMode).AddKey (Keys.RightWindows)
			);
			menu.AddButton (new MenuItemInfo (
				text: "Options", left: 0.260f, top: 0.840f, right: 0.480f, bottom: 0.950f,
			    onClick: () => NextState = GameStates.OptionScreen).AddKey (Keys.O)
			);
			menu.AddButton (new MenuItemInfo (
				text: "Exit", left: 0.800f, top: 0.535f, right: 0.980f, bottom: 0.790f,
			    onClick: () => game.Exit ()).AddKey (Keys.Escape)
			);

			// lines
			HfGDesign.AddLinePoints (ref LinePoints, 0, 50, 
				380, 250, 960, 380, 700, 160, 1000
			);
			HfGDesign.AddLinePoints (ref LinePoints, 0, 190,
				620, 855, 800, 535, 980, 790,
				480, 950, 260, 840, 520, 1000
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
			// logo
			spriteBatch.Draw (logo, new Rectangle (50, 380, 500, 300).Scale (viewport), Color.White);
			spriteBatch.End ();
		}

		public override void Activate (GameTime gameTime)
		{
			base.Activate (gameTime);
			AddGameComponents (gameTime, menu);
		}
	}
}

