using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using Knot3.Utilities;
using Knot3.KnotData;
using Knot3.RenderEffects;
using Knot3.Core;

using System.Collections;

using Knot3.UserInterface;

namespace Knot3.GameObjects
{
	public class EdgeColoring : GameScreenComponent, IKeyEventListener
	{
		public Knot Knot { get; set; }

		public EdgeColoring (GameScreen screen)
		: base(screen, DisplayLayer.None)
		{
			ValidKeys = new List<Keys> ();
			ValidKeys.Add (Keys.C);
		}

		public override void Update (GameTime time)
		{
		}

		public void OnKeyEvent (List<Keys> key, KeyEvent keyEvent, GameTime time)
		{
			// change color?
			if (Knot.SelectedEdges.Count () > 0 && Keys.C.IsDown ()) {
				ColorPicker picker = new ColorPicker (screen, new WidgetInfo (), DisplayLayer.Dialog);
				picker.OnSelectColor = (c) => screen.RemoveGameComponents (time, picker);
				foreach (Edge edge in Knot.SelectedEdges) {
					picker.OnSelectColor += (c) => edge.Color = c;
				}
				screen.AddGameComponents (time, picker);
			}
		}

		public List<Keys> ValidKeys { get; private set; }

		public bool IsKeyEventEnabled { get { return true; } }
	}
}
