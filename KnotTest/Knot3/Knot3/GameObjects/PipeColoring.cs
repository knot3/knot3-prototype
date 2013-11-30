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
	public class PipeColoring : GameStateComponent
	{
		public Knot Knot { get; set; }

		public PipeColoring (GameState state)
			: base(state, DisplayLayer.None)
		{
		}

		public override void Update (GameTime gameTime)
		{
			// check whether this edge is one of the selected edges
			if (Knot.Edges.SelectedEdges.Count () > 0) {
				// change color?
				if (Keys.C.IsDown ()) {
					ColorPicker picker = new ColorPicker (state, DisplayLayer.Dialog);
					picker.OnSelectColor = (c) => state.RemoveGameComponents(gameTime, picker);
					foreach (Edge edge in Knot.Edges.SelectedEdges) {
						picker.OnSelectColor += (c) => edge.Color = c;
					}
					state.AddGameComponents (gameTime, picker);
				}
			}
		}
	}
}

