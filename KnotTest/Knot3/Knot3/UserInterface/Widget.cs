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
	/// <summary>
	/// Alle GUI-Elemente erben von der Klasse Widget, die immer vorhandene Attribute und häufig verwendete
	/// Methoden zur Verfügung stellt.
	/// </summary>
	public abstract class Widget : DrawableGameStateComponent
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

	/// <summary>
	/// Alle Widgets, die sich immer in einem Container-Widget befinden, eine bestimmte Nummer haben und
	/// deren Eigenschaften von ihrer Nummer und ihrem Container abhängigen, erben von dieser Klasse.
	/// </summary>
	public abstract class ItemWidget : Widget
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

	/// <summary>
	/// Status einem ItemWidget's.
	/// </summary>
	public enum ItemState
	{
		/// <summary>
		/// Das Widget ist nicht selektiert.
		/// </summary>
		Unselected = 0,
		/// <summary>
		/// Das Widget ist selektiert.
		/// </summary>
		Selected
	}

	/// <summary>
	/// Horizontale Ausrichtung eines Widgets.
	/// </summary>
	public enum HAlign
	{
		Left = 0,
		Center,
		Right
	}

	/// <summary>
	/// Vertikale Ausrichtung eines Widgets.
	/// </summary>
	public enum VAlign
	{
		Top,
		Center,
		Bottom
	}

	/// <summary>
	/// Ein Delegate, das ein Vector2-Objekt zurückgibt, das als Größe interpretiert wird.
	/// </summary>
	public delegate Vector2 LazySize ();

	/// <summary>
	/// Ein Delegate, das ein Vector2-Objekt zurückgibt, das als Position interpretiert wird.
	/// </summary>
	public delegate Vector2 LazyPosition ();

	/// <summary>
	/// Ein Delegate, das ein Color-Objekt zurückgibt.
	/// </summary>
	public delegate Color LazyColor ();

	/// <summary>
	/// Ein Delegate, das eine Item-Nummer (int) als Argument erwartet und ein Vector2-Objekt zurückgibt,
	/// das als Größe interpretiert wird.
	/// </summary>
	public delegate Vector2 LazyItemSize (int n);

	/// <summary>
	/// Ein Delegate, das eine Item-Nummer (int) als Argument erwartet und ein Vector2-Objekt zurückgibt,
	/// das als Position interpretiert wird.
	/// </summary>
	public delegate Vector2 LazyItemPosition (int n);

	/// <summary>
	/// Ein Delegate, das einen ItemState als Argument erwartet und ein Color-Objekt zurückgibt.
	/// </summary>
	public delegate Color LazyItemColor (ItemState itemState);
}

