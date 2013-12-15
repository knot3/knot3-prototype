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
using Knot3.UserInterface;
using Knot3.Utilities;

namespace Knot3.Settings
{
	public class OptionScreen : MenuScreen
	{
		// menu
		private VerticalMenu menu;

		// textures
		private SpriteBatch spriteBatch;

		public OptionScreen (Core.Game game)
			: base(game)
		{
			menu = new VerticalMenu (this, new WidgetInfo (), DisplayLayer.Menu);
		}
		
		public override void Initialize ()
		{
			base.Initialize ();

			// create a new SpriteBatch, which can be used to draw textures
			spriteBatch = new SpriteBatch (device);

			// menu
			menu.ItemForegroundColor = ForegroundColor;
			menu.ItemBackgroundColor = BackgroundColor;
			menu.ItemAlignX = HAlign.Left;
			menu.ItemAlignY = VAlign.Center;

			MenuItemInfo info;
			info = new MenuItemInfo (text: "Video", onClick: () => NextState = GameScreens.VideoOptionScreen);
			menu.AddButton (info.AddKey (Keys.V));
			info = new MenuItemInfo (text: "Audio", onClick: () => NextState = GameScreens.OptionScreen);
			menu.AddButton (info.AddKey (Keys.A));
			info = new MenuItemInfo (text: "Controls", onClick: () => NextState = GameScreens.OptionScreen);
			menu.AddButton (info.AddKey (Keys.C));
			info = new MenuItemInfo (text: "Knots", onClick: () => NextState = GameScreens.OptionScreen);
			menu.AddButton (info.AddKey (Keys.K));
			info = new MenuItemInfo (text: "Back", onClick: () => NextState = GameScreens.StartScreen);
			menu.AddButton (info.AddKey (Keys.Escape));

			// lines
			HfGDesign.AddLinePoints (ref LinePoints, 0, 50,
				30, 970, 970, 50, 1000
			);
		}
		
		public override void UpdateMenu (GameTime time)
		{
		}
		
		public override void DrawMenu (GameTime time)
		{
			spriteBatch.Begin ();

			// text
			spriteBatch.DrawString (HfGDesign.MenuFont (this), "Options", new Vector2 (0.050f, 0.050f).Scale (viewport), Color.White,
					0, Vector2.Zero, 0.25f * viewport.ScaleFactor ().Length (), SpriteEffects.None, 0);

			// menu
			menu.Align (viewport, 1f, 100, 180, 0, 60);

			spriteBatch.End ();
		}

		public override void Activate (GameTime time)
		{
			base.Activate (time);
			AddGameComponents (time, menu);
		}
	}
}

