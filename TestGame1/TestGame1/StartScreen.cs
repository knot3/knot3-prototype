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
	public class StartScreen : GameState
	{
		// colors
		private Color backColor = Color.CornflowerBlue;

		// menu
		private Menu menu;

		// ...
		private GameState NextGameState;

		public StartScreen (Game game)
			: base(game)
		{
			menu = new Menu (this);
		}
		
		public override void Initialize ()
		{
			// input
			input = new StartScreenInput (this);
			input.SaveStates ();

			// menu
			menu.Initialize ();
			menu.Add ("New Game", Keys.Space, () => NextGameState = GameStates.KnotMode);
			menu.Add ("Options", Keys.O, () => game.Exit ());
			menu.Add ("Exit", Keys.Escape, () => game.Exit ());
		}

		public override GameState Update (GameTime gameTime)
		{
			NextGameState = this;

			// menu
			menu.Update (gameTime);

			// input
			input.Update (gameTime);
			input.SaveStates ();

			return NextGameState;
		}

		public override void Draw (GameTime gameTime)
		{
			menu.Align (device.Viewport);
			graphics.GraphicsDevice.Clear (backColor);
			menu.Draw (gameTime);
		}

		public override void Unload ()
		{
		}
	}

	public class Menu : GameClass
	{
		// graphics-related classes
		private SpriteBatch spriteBatch;

		// fonts
		private SpriteFont font;

		// menu-related attributes
		private List<MenuItem> Items;
		private Vector2 Position;
		private Vector2 ItemSize;
		private Vector2 Padding;

		public Menu (GameState state)
			: base(state)
		{
			Items = new List<MenuItem> ();
		}

		public void Add (string text, Keys key, Action onClick)
		{
			MenuItem item = new MenuItem (state, text, Items.Count, ItemPosition, () => ItemSize);
			item.OnClick += onClick;
			item.Keys.Add (key);
			Items.Add (item);
		}

		public void Align (Viewport viewport)
		{
			ItemSize = Vector2.Zero;
			foreach (MenuItem item in Items) {
				Vector2 minSize = item.MinimumSize (font);
				if (minSize.X < ItemSize.X || ItemSize == Vector2.Zero) {
					ItemSize = minSize;
				}
			}
			ItemSize += new Vector2 (200, font.LineSpacing * 0.5f);
			Position = (viewport.ToVector2 () - size ()) / 2;
			Console.WriteLine ("viewport=" + viewport.ToVector2 () + ", size=" + size () + " => position=" + Position);
		}

		public Vector2 size ()
		{
			return new Vector2 (ItemSize.X, ItemSize.Y * Items.Count + Padding.Y * (Items.Count - 1));
		}

		public Vector2 ItemPosition (int n)
		{
			return Position + new Vector2 (0, (ItemSize.Y + Padding.Y) * n);
		}

		public MenuItem this [int i] {
			get {
				while (i < 0) {
					i += Items.Count;
				}
				return Items [i % Items.Count];
			}
			set {
				while (i < 0) {
					i += Items.Count;
				}
				Items [i % Items.Count] = value;
			}
		}
		
		public void Initialize ()
		{
			// create a new SpriteBatch, which can be used to draw textures
			spriteBatch = new SpriteBatch (graphics.GraphicsDevice);

			// load fonts
			try {
				font = content.Load<SpriteFont> ("MenuFont");
			} catch (ContentLoadException ex) {
				font = null;
				Console.WriteLine (ex.Message);
			}

			Position = Vector2.Zero;
			ItemSize = new Vector2 (300, 0);
			Padding = new Vector2 (font.LineSpacing * 0.5f, font.LineSpacing * 0.5f);
		}

		public void Update (GameTime gameTime)
		{
			updateMouseSelection (gameTime);

			if (Mouse.GetState ().LeftButton == ButtonState.Pressed || Keys.Enter.IsDown ()) {
				onMouseClick (gameTime);
			}
			
			foreach (MenuItem item in Items) {
				item.Update (gameTime);
			}
		}
		
		private Point previousMousePosition;

		private void updateMouseSelection (GameTime gameTime)
		{
			Point mousePosition = Mouse.GetState ().ToPoint ();
			if (mousePosition != previousMousePosition) {
				foreach (MenuItem item in Items) {
					bool selected = item.bounds ().Contains (mousePosition);
					item.ItemState = selected ? MenuItemState.Selected : MenuItemState.Normal;
				}
			}
			previousMousePosition = mousePosition;
		}

		private void onMouseClick (GameTime gameTime)
		{
			foreach (MenuItem item in Items) {
				if (item.ItemState == MenuItemState.Selected) {
					item.Activate ();
				}
			}
		}

		public void Draw (GameTime gameTime)
		{
			spriteBatch.Begin ();
			foreach (MenuItem item in Items) {
				item.Draw (spriteBatch, font, gameTime);
			}
			spriteBatch.End ();
		}
	}

	public enum MenuItemState
	{
		Selected,
		Normal
	}

	public class MenuItem : GameClass
	{
		// item data
		private int ItemNum;
		private string Text;

		// state, position and sizes
		public MenuItemState ItemState;
		private Func<int, Vector2> PositionFunc;
		private Func<Vector2> SizeFunc;

		private Vector2 Position {
			get { return PositionFunc (ItemNum); }
		}

		private Vector2 Size {
			get { return SizeFunc (); }
		}

		// keys to listen on
		public List<Keys> Keys = new List<Keys> ();

		// click action
		public Action OnClick = () => {};

		/// <summary>
		/// Initializes a new instance of the <see cref="TestGame1.MenuItem"/> class.
		/// </summary>
		public MenuItem (GameState state, string text, int itemNum, Func<int, Vector2> positionFunc, Func<Vector2> sizeFunc)
			: base(state)
		{
			Text = text;
			PositionFunc = positionFunc;
			SizeFunc = sizeFunc;
			ItemNum = itemNum;
		}

		public void Update (GameTime gameTime)
		{
			foreach (Keys key in Keys) {
				if (key.IsDown ()) {
					Activate ();
				}
			}
		}

		public void Draw (SpriteBatch spriteBatch, SpriteFont font, GameTime gameTime)
		{
			Texture2D paneTexture = Textures.Create (device, Color.White);
			spriteBatch.Draw (paneTexture, bounds (), BackgroundColor ());

			try {
				spriteBatch.DrawString (font, Text, TextPosition (font), Color.Black);

			} catch (ArgumentException exp) {
				Console.WriteLine (exp.ToString ());
			} catch (InvalidOperationException exp) {
				Console.WriteLine (exp.ToString ());
			}
		}

		public Color BackgroundColor ()
		{
			switch (ItemState) {
			case MenuItemState.Selected:
				return Color.White * 0.6f;
			case MenuItemState.Normal:
			default:
				return Color.White * 0.4f;
			}
		}

		public Vector2 TextPosition (SpriteFont font)
		{
			Vector2 textPosition = Position;
			textPosition.Y += (Size.Y - font.LineSpacing) / 2;
			textPosition.X += (Size.Y - font.LineSpacing) / 2;
			return textPosition;
		}

		public Vector2 MinimumSize (SpriteFont font)
		{
            return font.MeasureString(Text) + new Vector2(font.LineSpacing, font.LineSpacing) * 0.2f;
		}

		public Rectangle bounds ()
		{
			Point topLeft = Position.ToPoint ();
			Point size = Size.ToPoint ();
			return new Rectangle (topLeft.X, topLeft.Y, size.X, size.Y);
		}

		public void Activate ()
		{
			OnClick ();
		}
	}
}

