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

namespace TestGame1
{
	public class LoadSavegameScreen : MenuScreen
	{
		// menu
		private VerticalMenu menu;

		// textures
		private SpriteBatch spriteBatch;

		// savegame files
		string[] FileExtensions = new string[] {".knot", ".knt"};

		public LoadSavegameScreen (Game game)
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

			// lines
			HfGDesign.AddLinePoints (ref LinePoints, 0, 50, new float[]{
				30, 970, 970, 50, 1000
			}
			);
		}

		private void UpdateFiles ()
		{
			string[] searchDirectories = new string[] {
				Files.BaseDirectory,
				Files.SavegameDirectory
			};
			Console.WriteLine ("Search for Savegames: " + string.Join(", ", searchDirectories));

			menu.Clear ();
			AddDefaultKnots ();
			Files.SearchFiles (searchDirectories, FileExtensions, AddFileToList);
		}

		private void AddFileToList (string file)
		{
			KnotFormat format = new KnotFormat (file);
			KnotInfo knotInfo = format.Info;
			Action LoadFile = () => {
				// delegate to load the file
				if (knotInfo.IsValid) {
					Console.WriteLine ("File is valid: " + knotInfo);
					GameStates.CreativeMode.Knot = format.Knot;
					NextState = GameStates.CreativeMode;
				} else {
					Console.WriteLine ("File is invalid: " + knotInfo);
				}
			};
			string name = knotInfo.IsValid ? knotInfo.Name : file;

			MenuItemInfo info = new MenuItemInfo (text: name, onClick: LoadFile);
			menu.AddButton (info);
		}

		private void AddDefaultKnots ()
		{
			Action RandomKnot = () => {
				Knot knot = Knot.RandomKnot (20, (k) => {});
				knot.Save = (k) => new KnotFormat (knot.Info.Filename).Knot = k;
				Console.WriteLine ("Random Knot: " + knot.Info);
				GameStates.CreativeMode.Knot = knot;
				NextState = GameStates.CreativeMode;
			};
			Action DefaultKnot = () => {
				Knot knot = Knot.DefaultKnot ((k) => {});
				knot.Save = (k) => new KnotFormat (knot.Info.Filename).Knot = k;
				Console.WriteLine ("Default Knot: " + knot.Info);
				GameStates.CreativeMode.Knot = knot;
				NextState = GameStates.CreativeMode;
			};
			MenuItemInfo info = new MenuItemInfo (text: "New Knot", onClick: DefaultKnot);
			menu.AddButton (info);
			info = new MenuItemInfo (text: "New Random Knot", onClick: RandomKnot);
			menu.AddButton (info);
		}
		
		public override void UpdateMenu (GameTime gameTime)
		{
			// menu
			menu.Update (gameTime);

			// when is escape is pressed, go to start screen
			if (Keys.Escape.IsDown ()) {
				NextState = GameStates.StartScreen;
			}
		}
		
		public override void DrawMenu (GameTime gameTime)
		{
			spriteBatch.Begin ();

			// text
			spriteBatch.DrawString (menu.Font, "Load Savegames", new Vector2 (0.050f, 0.050f).Scale (viewport), Color.White,
					0, Vector2.Zero, 0.25f * viewport.ScaleFactor ().Length (), SpriteEffects.None, 0);

			// menu
			menu.Align (viewport, 1f, 100, 180, 0, 40);
			menu.Draw (0f, spriteBatch, gameTime);

			spriteBatch.End ();
		}

		public override void Activate (GameTime gameTime)
		{
			UpdateFiles ();
		}
	}
}

