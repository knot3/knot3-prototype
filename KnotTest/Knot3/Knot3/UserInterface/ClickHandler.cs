using System;

using Microsoft.Xna.Framework;

using Knot3.Utilities;

namespace Knot3.UserInterface
{
	public class ClickHandler : GameComponent
	{
		public ClickHandler (GameState state)
			: base(state, DisplayLayer.None)
		{
		}

		public override void Update (GameTime gameTime)
		{
			IMouseEvent hoveredComponent = null;
			int hoveredLayer = 0;
			foreach (GameComponent _component in state.game.Components) {
				if (_component is IMouseEvent) {
					IMouseEvent component = _component as IMouseEvent;
					// mouse input
					bool hovered = component.bounds ().Contains (Input.MouseState.ToPoint ());
					component.SetHovered (hovered);
					if (hovered && component.Index > hoveredLayer && component.IsVisible) {
						hoveredComponent = component;
						hoveredLayer = component.Index;
					}
				}
			}
			if (hoveredComponent != null) {
				if (Input.MouseState.IsLeftClick (gameTime)) {
					hoveredComponent.Activate ();
				}
			}
		}
	}
}

