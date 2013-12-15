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
	/// <summary>
	/// Ein GameScreenComponent, der das IGameObject aus der World selektiert, das 
	/// sich unter der aktuellen Mausposition befindet.
	/// </summary>
	public class ModelMousePicker : GameScreenComponent
	{
		// game world
		private World World { get; set; }

		// ray check
		private double lastRayCheck = 0;
		private Vector2 lastMousePosition = Vector2.Zero;

		/// <summary>
		/// Initializes a new MousePicking component.
		/// </summary>
		public ModelMousePicker (GameScreen screen, World world)
			: base(screen, DisplayLayer.None)
		{
			World = world;
		}

		public override void Update (GameTime time)
		{
			// mouse ray selection
			CheckMouseRay (time);
		}

		private void CheckMouseRay (GameTime time)
		{
			double millis = time.TotalGameTime.TotalMilliseconds;
			if (millis > lastRayCheck + 10
				&& (screen.input.CurrentInputAction == InputAction.TargetMove
				|| screen.input.CurrentInputAction == InputAction.FreeMouse)
				&& InputManager.MouseState.ToVector2 () != lastMousePosition) {

				lastRayCheck = millis;
				lastMousePosition = InputManager.MouseState.ToVector2 ();

				Overlay.Profiler ["Ray"] = Knot3.Core.Game.Time (() => {

					UpdateMouseRay (time);

				}
				).TotalMilliseconds;
			}
		}

		private void UpdateMouseRay (GameTime time)
		{
			Ray ray = World.Camera.GetMouseRay (InputManager.MouseState.ToVector2 ());

			GameObjectDistance nearest = null;
			foreach (IGameObject obj in World.Objects) {
				if (obj.Info.IsVisible) {
					GameObjectDistance intersection = obj.Intersects (ray);
					if (intersection != null) {
						if (intersection.Distance > 0 && (nearest == null || intersection.Distance < nearest.Distance)) {
							nearest = intersection;
						}
					}
				}
			}
			if (nearest != null) {
				World.SelectObject (nearest.Object, time);
			} else {
				World.SelectObject (null, time);
			}
		}
	}
}

