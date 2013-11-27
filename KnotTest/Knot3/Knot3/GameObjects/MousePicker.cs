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

using Knot3.Core;
using Knot3.Utilities;
using Knot3.RenderEffects;
using Knot3.Settings;

namespace Knot3.GameObjects
{
	public class MousePicker : GameStateComponent
	{
		// ray check
		private double lastRayCheck = 0;

		/// <summary>
		/// Initializes a new MousePicking component.
		/// </summary>
		public MousePicker (GameState state)
			: base(state, DisplayLayer.None)
		{
		}

		public override void Update (GameTime gameTime)
		{
			// mouse ray selection
			UpdateMouseRay (gameTime);
		}

		public void UpdateMouseRay (GameTime gameTime)
		{
			double millis = gameTime.TotalGameTime.TotalMilliseconds;
			if (millis > lastRayCheck + 10 && (input.CurrentInputAction == InputAction.TargetMove
				|| input.CurrentInputAction == InputAction.FreeMouse)) {
				lastRayCheck = millis;

				Ray ray = camera.GetMouseRay (Core.Input.MouseState.ToVector2 ());

				GameObjectDistance nearest = null;
				foreach (IGameObject obj in world.Objects) {
					if (obj.Info.IsVisible) {
						GameObjectDistance intersection = obj.Intersects (ray);
						if (intersection != null) {
							//Console.WriteLine ("time=" + (int)gameTime.TotalGameTime.TotalMilliseconds +
							//	", obj = " + obj + ", distance = " + MathHelper.Clamp ((float)distance, 0, 100000)
							//);
							if (intersection.Distance > 0 && (nearest == null || intersection.Distance < nearest.Distance)) {
								nearest = intersection;
							}
						}
					}
				}
				if (nearest != null) {
					world.SelectObject (nearest.Object, gameTime);
				}
			}
		}
	}
}

