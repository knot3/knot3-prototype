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
	public class WidgetInfo : IEquatable<WidgetInfo>
	{
		// size and position
		public Func<Vector2> RelativePosition;
		public Func<Vector2> RelativeSize = () => Vector2.Zero;
		public Func<Vector2> RelativePadding = () => Vector2.Zero;

		// alignment
		public HAlign AlignX = HAlign.Left;
		public VAlign AlignY = VAlign.Center;

		// colors
		public Func<Color> ForegroundColor = () => Color.Transparent;
		public Func<Color> BackgroundColor = () => Color.Transparent;

		// scaled to viewport size

		public Vector2 ScaledPosition (Viewport viewport)
		{
			return RelativePosition ().Scale (viewport);
		}

		public Vector2 ScaledSize (Viewport viewport)
		{
			return RelativeSize ().Scale (viewport);
		}

		public Vector2 ScaledPadding (Viewport viewport)
		{
			return RelativePadding ().Scale (viewport);
		}

		public Rectangle RelativeRectangle ()
		{
			return HfGDesign.CreateRectangle (RelativePosition (), RelativeSize ());
		}

		public Rectangle ScaledRectangle (Viewport viewport)
		{
			return HfGDesign.CreateRectangle (ScaledPosition (viewport), ScaledSize (viewport));
		}

		public WidgetInfo ()
		{
			RelativePosition = () => (Vector2.One - RelativeSize ()) / 2;
		}

		public virtual bool Equals (WidgetInfo other)
		{
			if (other == null) 
				return false;

			if (this.RelativePosition () == other.RelativePosition () && this.RelativeSize () == other.RelativeSize ())
				return true;
			else
				return false;
		}

		public override bool Equals (Object obj)
		{
			if (obj == null) 
				return false;

			WidgetInfo widgetObj = obj as WidgetInfo;
			if (widgetObj == null)
				return false;
			else   
				return Equals (widgetObj);   
		}

		public override int GetHashCode ()
		{
			return this.RelativePosition ().GetHashCode ()
				+ this.RelativeSize ().GetHashCode ();
		}

		public static bool operator == (WidgetInfo o1, WidgetInfo o2)
		{
			if ((object)o1 == null || ((object)o2) == null)
				return Object.Equals (o1, o2);

			return o2.Equals (o2);
		}

		public static bool operator != (WidgetInfo o1, WidgetInfo o2)
		{
			return ! (o1 == o2);
		}
	}

	/// <summary>
	/// Alle GUI-Elemente erben von der Klasse Widget, die immer vorhandene Attribute und häufig verwendete
	/// Methoden zur Verfügung stellt.
	/// </summary>
	public abstract class Widget : DrawableGameStateComponent
	{
		public WidgetInfo Info { get; set; }

		// visibility
		public virtual bool IsVisible { get; set; }

		public Widget (GameState state, WidgetInfo info, DisplayLayer drawOrder)
			: base(state, drawOrder)
		{
			Info = info;
			IsVisible = true;
		}

		public Rectangle bounds ()
		{
			Point topLeft = Info.ScaledPosition (state.viewport).ToPoint ();
			Point size = Info.ScaledSize (state.viewport).ToPoint ();
			return new Rectangle (topLeft.X, topLeft.Y, size.X, size.Y);
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

		public ItemWidget (GameState state, WidgetInfo info, DisplayLayer drawOrder, int itemNum)
			: base(state, info, drawOrder)
		{
			ItemNum = itemNum;
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
}

