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
	public class Menu : GameClass
	{
		// graphics-related attributes
		private SpriteBatch spriteBatch;

		// fonts
		protected SpriteFont font;

		// menu-related attributes
		protected List<MenuItem> Items;

		public Menu (GameState state)
			: base(state)
		{
			Items = new List<MenuItem> ();
		}

		public void Add (string text, Keys key, Action onClick, LazyVector2 positionFunc,
		                 LazyVector2 sizeFunc, HorizontalAlignment alignX)
		{
			MenuItem item = new MenuItem (state, text, Items.Count, positionFunc, sizeFunc, alignX);
			item.OnClick += onClick;
			item.Keys.Add (key);
			Items.Add (item);
		}

		public void Add (string text, Keys key, Action onClick, Vector2 position, Vector2 size, HorizontalAlignment alignX)
		{
			Add (text, key, onClick, (i) => position.Scale(viewport), (i) => (size - position).Scale(viewport), alignX);
		}

		public void Add (string text, Keys key, Action onClick, float left, float top, float right, float bottom, HorizontalAlignment alignX)
		{
			Add (text, key, onClick, new Vector2 (left, top), new Vector2 (right, bottom), alignX);
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
		
		public virtual void Initialize ()
		{
			// create a new SpriteBatch, which can be used to draw textures
			spriteBatch = new SpriteBatch (device);

			// load fonts
			try {
				font = content.Load<SpriteFont> ("MenuFont");
			} catch (ContentLoadException ex) {
				font = null;
				Console.WriteLine (ex.Message);
			}
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

	public class LinearMenu : Menu
	{
		// menu-related attributes
		private Vector2 Position;
		private Vector2 ItemSize;
		private Vector2 Padding;

		public LinearMenu (GameState state)
			: base(state)
		{
		}
		
		public override void Initialize ()
		{
			base.Initialize ();

			Position = Vector2.Zero;
			ItemSize = new Vector2 (300, 0);
			Padding = new Vector2 (font.LineSpacing * 0.5f, font.LineSpacing * 0.5f);
		}

		public void Add (string text, Keys key, Action onClick)
		{
			Add (text, key, onClick, ItemPosition, (int n) => ItemSize, HorizontalAlignment.Left);
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
			//Console.WriteLine ("viewport=" + viewport.ToVector2 () + ", size=" + size () + " => position=" + Position);
		}

		public Vector2 size ()
		{
			return new Vector2 (ItemSize.X, ItemSize.Y * Items.Count + Padding.Y * (Items.Count - 1));
		}

		public Vector2 ItemPosition (int n)
		{
			return Position + new Vector2 (0, (ItemSize.Y + Padding.Y) * n);
		}
	}
}

