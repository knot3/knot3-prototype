using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Knot3.Core;
using Knot3.UserInterface;
using Knot3.KnotData;
using Knot3.RenderEffects;
using Knot3.GameObjects;
using Knot3.Settings;
using Knot3.Utilities;

namespace Knot3.UserInterface
{
	public class Widget : DrawableGameStateComponent
	{
		// size and position
		public virtual LazyPosition RelativePosition { get; protected set; }

		public virtual LazySize RelativeSize { get; protected set; }

		public virtual LazySize RelativePadding { get; protected set; }

		protected Vector2 ScaledPosition { get { return RelativePosition ().Scale (viewport); } }

		protected Vector2 ScaledSize { get { return RelativeSize ().Scale (viewport); } }

		protected Vector2 ScaledPadding { get { return RelativePadding ().Scale (viewport); } }

		// alignment
		protected HAlign AlignX;
		protected VAlign AlignY;

		// colors
		private LazyColor foregroundColorFunc;
		private LazyColor backgroundColorFunc;

		protected virtual Color ForegroundColor { get { return foregroundColorFunc (); } }

		protected virtual Color BackgroundColor { get { return backgroundColorFunc (); } }

		// visibility
		public virtual bool IsVisible { get; set; }

		public Widget (GameState state, DisplayLayer drawOrder, LazyColor foregroundColor, LazyColor backgroundColor,
		               HAlign alignX, VAlign alignY)
			: base(state, drawOrder)
		{
			RelativePosition = () => Vector2.Zero;
			RelativeSize = () => Vector2.Zero;
			RelativePadding = () => Vector2.Zero;
			AlignX = alignX;
			AlignY = alignY;
			foregroundColorFunc = foregroundColor != null ? foregroundColor : () => Color.Transparent;
			backgroundColorFunc = backgroundColor != null ? backgroundColor : () => Color.Transparent;
			IsVisible = true;
		}
	}

	public class ItemWidget : Widget
	{
		protected int ItemNum;
		public ItemState ItemState;
		private LazyItemColor foregroundItemColorFunc;
		private LazyItemColor backgroundItemColorFunc;

		protected override Color ForegroundColor { get { return foregroundItemColorFunc (ItemState); } }

		protected override Color BackgroundColor { get { return backgroundItemColorFunc (ItemState); } }

		public ItemWidget (GameState state, DisplayLayer drawOrder, int itemNum, LazyItemColor foregroundColor, LazyItemColor backgroundColor,
		                   HAlign alignX, VAlign alignY)
			: base(state, drawOrder, null, null, alignX, alignY)
		{
			ItemNum = itemNum;
			foregroundItemColorFunc = foregroundColor != null ? foregroundColor : (s) => Color.Transparent;
			backgroundItemColorFunc = backgroundColor != null ? backgroundColor : (s) => Color.Transparent;
		}
	}

	public enum ItemState
	{
		Selected,
		Unselected
	}

	public enum HAlign
	{
		Left,
		Center,
		Right
	}

	public enum VAlign
	{
		Top,
		Center,
		Bottom
	}

	// delegates
	public delegate Vector2 LazyItemSize (int n);

	public delegate Vector2 LazyItemPosition (int n);

	public delegate Vector2 LazySize ();

	public delegate Vector2 LazyPosition ();

	public delegate Color LazyColor ();

	public delegate Color LazyItemColor (ItemState itemState);
}

