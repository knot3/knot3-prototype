using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Knot3.Utilities;

namespace Knot3.Core
{
	public class KeyHandler : GameStateComponent
	{
		public KeyHandler (GameState state)
			: base(state, DisplayLayer.None)
		{
		}

		private class KeyEventComponent
		{
			public IKeyEventListener receiver;
			public int layer = 0;
			public KeyEvent keyEvent;
			public List<Keys> keys;
		}

		public override void Update (GameTime gameTime)
		{
			KeyEventComponent best = null;
			foreach (IGameStateComponent component in state.game.Components) {
				if (component is IKeyEventListener) {

					// keyboard input
					IKeyEventListener receiver = component as IKeyEventListener;
					KeyEvent keyEvent = KeyEvent.None;
					List<Keys> keysInvolved = new List<Keys> ();

					foreach (Keys key in receiver.ValidKeys) {
						if (key.IsDown ()) {
							keysInvolved.Add (key);
							keyEvent = KeyEvent.KeyDown;
						} else if (key.IsHeldDown ()) {
							keysInvolved.Add (key);
							keyEvent = KeyEvent.KeyHeldDown;
						}
					}

					if (keysInvolved.Count > 0 && receiver.IsKeyEventEnabled && (best == null || component.Index >= best.layer)) {
						best = new KeyEventComponent {
							receiver = receiver,
							layer = receiver.Index,
							keyEvent = keyEvent,
							keys = keysInvolved
						}; 
					}
				}
			}
			if (best != null) {
				best.receiver.OnKeyEvent (best.keys, best.keyEvent, gameTime);
			}
		}
	}
}

