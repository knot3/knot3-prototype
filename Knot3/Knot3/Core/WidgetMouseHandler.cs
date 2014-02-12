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
					bool hovered = bounds.Contains (InputManager.CurrentMouseState.ToPoint ());
					receiver.SetHovered (hovered);
					if (hovered && receiver.IsMouseEventEnabled && (best == null || receiver.Index > best.layer)) {
						best = new ClickEventComponent {
							receiver = receiver,
							layer = receiver.Index,
							relativePosition = InputManager.CurrentMouseState.ToVector2()-bounds.Location.ToVector2()
						};
					}
				}
			}
			if (best != null) {
				if (InputManager.LeftMouseButton != ClickState.None) {
					best.receiver.OnLeftClick (best.relativePosition, InputManager.LeftMouseButton, time);
				}
				if (InputManager.RightMouseButton != ClickState.None) {
					best.receiver.OnRightClick (best.relativePosition, InputManager.RightMouseButton, time);
				}
			}
		}
	}
}
