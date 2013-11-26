using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Knot3.Utilities;

namespace Knot3.UserInterface
{
	public class KeyHandler : GameComponent
	{
		public KeyHandler (GameState state)
			: base(state, DisplayLayer.None)
		{
		}

		public override void Update (GameTime gameTime)
		{
			IKeyEvent activatedComponent = null;
			int activatedLayer = 0;
			foreach (GameComponent _component in state.game.Components) {
				if (_component is IKeyEvent) {
					IKeyEvent component = _component as IKeyEvent;
					// keyboard input
					bool keyPressed = false;
					foreach (Keys key in component.Keys) {
						if (key.IsDown ()) {
							keyPressed = true;
							break;
						}
					}
					if (keyPressed && component.Index > activatedLayer && component.IsVisible) {
						activatedComponent = component;
						activatedLayer = component.Index;
					}
				}
			}
			if (activatedComponent != null) {
				activatedComponent.Activate ();
			}
		}
	}
}

