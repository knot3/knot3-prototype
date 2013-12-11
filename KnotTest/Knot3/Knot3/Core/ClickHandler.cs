using System;

using Microsoft.Xna.Framework;

using Knot3.Utilities;

namespace Knot3.Core
{
	public class ClickHandler : GameStateComponent
	{
		public ClickHandler (GameState state)
			: base(state, DisplayLayer.None)
		{
		}

		private class ClickEventComponent
		{
			public IMouseEventListener receiver;
			public DisplayLayer layer = 0;
			public Vector2 relativePosition;
		}

		public override void Update (GameTime gameTime)
		{
			ClickEventComponent best = null;
			foreach (IGameStateComponent _component in state.game.Components) {
				if (_component is IMouseEventListener) {
					IMouseEventListener receiver = _component as IMouseEventListener;
					// mouse input
					Rectangle bounds = receiver.bounds ();
					bool hovered = bounds.Contains (Input.MouseState.ToPoint ());
					receiver.SetHovered (hovered);
					if (hovered && receiver.IsMouseEventEnabled && (best == null || receiver.Index > best.layer)) {
						best = new ClickEventComponent {
							receiver = receiver,
							layer = receiver.Index,
							relativePosition = Input.MouseState.ToVector2()-bounds.Location.ToVector2()
						};
					}
				}
			}
			if (best != null) {
				if (Input.LeftButton != ClickState.None) {
					best.receiver.OnLeftClick (best.relativePosition, Input.LeftButton, gameTime);
				}
				if (Input.RightButton != ClickState.None) {
					best.receiver.OnRightClick (best.relativePosition, Input.LeftButton, gameTime);
				}
			}
		}
	}
}

