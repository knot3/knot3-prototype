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
		public ModelMousePicker (GameScreen state, World world)
			: base(state, DisplayLayer.None)
		{
			World = world;
		}

		public override void Update (GameTime gameTime)
		{
			// mouse ray selection
			CheckMouseRay (gameTime);
		}

		private void CheckMouseRay (GameTime gameTime)
		{
			double millis = gameTime.TotalGameTime.TotalMilliseconds;
			if (millis > lastRayCheck + 10
				&& (state.input.CurrentInputAction == InputAction.TargetMove
				|| state.input.CurrentInputAction == InputAction.FreeMouse)
				&& InputManager.MouseState.ToVector2 () != lastMousePosition) {

				lastRayCheck = millis;
				lastMousePosition = InputManager.MouseState.ToVector2 ();

				Overlay.Profiler ["Ray"] = Knot3.Core.Game.Time (() => {

					UpdateMouseRay (gameTime);

				}
				).TotalMilliseconds;
			}
		}

		private void UpdateMouseRay (GameTime gameTime)
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
				World.SelectObject (nearest.Object, gameTime);
			} else {
				World.SelectObject (null, gameTime);
			}
		}
	}
}

