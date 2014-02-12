using System;
using System.IO;
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
using Knot3.KnotData;

namespace Knot3.CreativeMode
{
	public class CreativeLoadScreen : MenuScreen
	{
		// menu
		private VerticalMenu menu;

		// textures
		private SpriteBatch spriteBatch;

		// files
		private FileIndex fileIndex;
		private IKnotIO fileFormat;

		public CreativeLoadScreen (Core.Knot3Game game)
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
			menu.ItemAlignX = HorizontalAlignment.Left;
			menu.ItemAlignY = VerticalAlignment.Center;

			// lines
			HfGDesign.AddLinePoints (ref LinePoints, 0, 50, new float[] {
				30, 970, 970, 50, 1000
			}
			                        );
		}

		private void UpdateFiles ()
		{
			fileFormat = new KnotFileIO ();
			fileIndex = new FileIndex (Files.SavegameDirectory + Files.Separator + "index.txt");

			string[] searchDirectories = new string[] {
				Files.BaseDirectory,
				Files.SavegameDirectory
			};
			Console.WriteLine ("Search for Savegames: " + string.Join (", ", searchDirectories));

			menu.Clear ();
			AddDefaultKnots ();
			Files.SearchFiles (searchDirectories, KnotFileIO.FileExtensions, AddFileToList);
		}

		private void AddFileToList (string filename)
		{
			string hashcode = FileUtility.GetHash(filename);
			bool isValid = fileIndex.Contains (hashcode);
			if (!isValid) {
				try {
					// load the knot and look for exceptions
					fileFormat.Load (filename);
					isValid = true;
					fileIndex.Add (hashcode);
				}
				catch (Exception ex) {
					Console.WriteLine (ex);
					isValid = false;
				}
			}
			if (isValid) {
				KnotMetaData meta = fileFormat.LoadMetaData(filename);
				Action LoadFile = () => {
					// delegate to load the file

					//if (knotInfo.IsValid) {
					Console.WriteLine ("File is valid: " + meta);
					GameScreens.CreativeMode.Knot = fileFormat.Load(filename);
					NextState = GameScreens.CreativeMode;
					//} else {
					//	Console.WriteLine ("File is invalid: " + knotInfo);
					//}
				};
				string name = meta.Name.Length > 0 ? meta.Name : filename;

				MenuItemInfo info = new MenuItemInfo (text: name, onClick: LoadFile);
				menu.AddButton (info);
			}
		}

		private void AddDefaultKnots ()
		{
			/*
			Action RandomKnot = () => {
				__Knot knot = __Knot.RandomKnot (20, format);
				Console.WriteLine ("Random Knot: " + knot.Info);
				GameScreens.CreativeMode.Knot = knot;
				NextState = GameScreens.CreativeMode;
			};
			*/
			Action DefaultKnot = () => {
				Knot knot = new Knot ();
				Console.WriteLine ("Default Knot: " + knot);
				GameScreens.CreativeMode.Knot = knot;
				NextState = GameScreens.CreativeMode;
			};
			MenuItemInfo info = new MenuItemInfo (text: "New Knot", onClick: DefaultKnot);
			menu.AddButton (info);
			/*
			info = new MenuItemInfo (text: "New Random Knot", onClick: RandomKnot);
			menu.AddButton (info);
			*/
		}

		public override void UpdateMenu (GameTime time)
		{
			// menu
			menu.Update (time);

			// when is escape is pressed, go to start screen
			if (Keys.Escape.IsDown ()) {
				NextState = GameScreens.StartScreen;
			}
		}

		public override void DrawMenu (GameTime time)
		{
			spriteBatch.Begin ();

			// text
			spriteBatch.DrawString (
			    HfGDesign.MenuFont (this), "Load Savegames", new Vector2 (0.050f, 0.050f).Scale (viewport), Color.White,
			    0, Vector2.Zero, 0.25f * viewport.ScaleFactor ().Length (), SpriteEffects.None, 0
			);

			// menu
			menu.Align (viewport, 1f, 100, 180, 0, 40);

			spriteBatch.End ();
		}

		public override void Entered (GameTime time)
		{
			UpdateFiles ();
			base.Entered (time);
			AddGameComponents (time, menu);
		}
	}
}
