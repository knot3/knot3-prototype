using System;

using Microsoft.Xna.Framework;

using Knot3.Utilities;

namespace Knot3.Core
{
	public class WidgetMouseHandler : GameScreenComponent
	{
		public WidgetMouseHandler (GameScreen screen)
			: base(screen, DisplayLayer.None)
		{
		}

		private class ClickEventComponent
		{
			public IMouseEventListener receiver;
			public DisplayLayer layer = 0;
			public Vector2 relativePosition;
		}

		public override void Update (GameTime time)
		{
			ClickEventComponent best = null;
			foreach (IGameScreenComponent _component in screen.game.Components) {
				if (_component is IMouseEventListener) {
					IMouseEventListener receiver = _component as IMouseEventListener;
					// mouse input
					Rectangle bounds = receiver.bounds ();
					bool hovered = bounds.Contains (InputManager.MouseState.ToPoint ());
					receiver.SetHovered (hovered);
					if (hovered && receiver.IsMouseEventEnabled && (best == null || receiver.Index > best.layer)) {
						best = new ClickEventComponent {
							receiver = receiver,
							layer = receiver.Index,
							relativePosition = InputManager.MouseState.ToVector2()-bounds.Location.ToVector2()
						};
					}
				}
			}
			if (best != null) {
				if (InputManager.LeftButton != ClickState.None) {
					best.receiver.OnLeftClick (best.relativePosition, InputManager.LeftButton, time);
				}
				if (InputManager.RightButton != ClickState.None) {
					best.receiver.OnRightClick (best.relativePosition, InputManager.LeftButton, time);
				}
			}
		}
	}
}

