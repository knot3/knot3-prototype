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

		public override void Update (GameTime gameTime)
		{
			IMouseEventReceiver hoveredComponent = null;
			int hoveredLayer = 0;
			foreach (IGameStateComponent _component in state.game.Components) {
				if (_component is IMouseEventReceiver) {
					IMouseEventReceiver component = _component as IMouseEventReceiver;
					// mouse input
					bool hovered = component.bounds ().Contains (Input.MouseState.ToPoint ());
					component.SetHovered (hovered);
					if (hovered && component.Index > hoveredLayer && component.IsMouseEventEnabled) {
						hoveredComponent = component;
						hoveredLayer = component.Index;
					}
				}
			}
			if (hoveredComponent != null) {
				if (Input.MouseState.IsLeftClick (gameTime)) {
					hoveredComponent.Activate (gameTime);
				}
			}
		}
	}
}

