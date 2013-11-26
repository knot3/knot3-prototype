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

namespace Knot3.Settings
{
	public class VideoOptionScreen : OptionScreen
	{
		// menu
		private VerticalMenu menu;

		// textures
		private SpriteBatch spriteBatch;

		public VideoOptionScreen (Game game)
			: base(game)
		{
			menu = new VerticalMenu (this, DisplayLayer.Menu);
		}
		
		public override void Initialize ()
		{
			base.Initialize ();

			// create a new SpriteBatch, which can be used to draw textures
			spriteBatch = new SpriteBatch (device);

			// menu
			menu.Initialize (ForegroundColor, BackgroundColor, HAlign.Left);
			menu.AddDropDown (new MenuItemInfo (text: "Debug Mode"), new BooleanOptionInfo ("game", "debug", false));
			menu.AddDropDown (new MenuItemInfo (text: "Coordinate system"),
			                  new BooleanOptionInfo ("video", "debug-coordinates", false));
			menu.AddDropDown (new MenuItemInfo (text: "Camera Overlay"),
			                  new BooleanOptionInfo ("video", "camera-overlay", true));
			menu.AddDropDown (new MenuItemInfo (text: "FPS Overlay"),
			                  new BooleanOptionInfo ("video", "fps-overlay", true));

			string currentResolution = viewport.Width + "x" + viewport.Height;
			string[] resolutions = { "1280x720", "1920x1080", "1366x768", "1024x768", "1280x800", "1680x1050", "1440x900", "1600x900",
			};
			Array.Sort (resolutions);
			menu.AddDropDown (new MenuItemInfo (text: "Resolution"),
                              new DistinctOptionInfo ("video", "resolution", currentResolution, resolutions));

			Action<bool> setFullscreen = (val) => game.IsFullscreen = val;
			menu.AddDropDown (new MenuItemInfo (text: "Fullscreen"),
			                  new BooleanOptionInfo ("video", "fullscreen", false, setFullscreen));

			Action<bool> setVSync = (val) => game.VSync = val;
			menu.AddDropDown (new MenuItemInfo (text: "VSync"),
			                  new BooleanOptionInfo ("video", "vsync", true, setVSync));

			menu.AddDropDown (new MenuItemInfo (text: "Model Quality"),
			                  new DistinctOptionInfo ("video", "model-quality", Models.Quality, Models.ValidQualities));
			menu.AddDropDown (new MenuItemInfo (text: "Cel Shading"),
			                  new BooleanOptionInfo ("video", "cel-shading", true));
		}

		public void Collapse (MenuItem item)
		{
			menu.CollapseMenus (item);
		}

		public override void UpdateMenu (GameTime gameTime)
		{
		}
		
		public override void DrawMenu (GameTime gameTime)
		{
			base.DrawMenu (gameTime);

			spriteBatch.Begin (SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

			// menu
			menu.Align (viewport, 1f, 350, 180, 550, 40);

			spriteBatch.End ();
		}
	}
}

