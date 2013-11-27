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
	public class World : DrawableGameStateComponent
	{
		// graphics-related classes
		private List<RenderEffect> knotRenderEffects;
		private RenderEffect knotRenderEffect;

		// game objects
		public List<IGameObject> Objects { get; private set; }

		public IGameObject SelectedObject { get; private set; }

		private TexturedRectangle floor;

		/// <summary>
		/// Initializes a new Overlay
		/// </summary>
		public World (GameState state)
			: base(state, DisplayLayer.World)
		{
			Objects = new List<IGameObject> ();

			// the floor
			Vector3 size = new Vector3 (2000, 0, 2000);
			Vector3 position = new Vector3 (-1000, -100, -1000);
			var floorInfo = new TexturedRectangleInfo (
				texturename: "floor", origin: position + new Vector3 (size.X, 0, size.Z) / 2,
				left: Vector3.Left, width: size.X, up: Vector3.Forward, height: size.Z
			);
			floor = new TexturedRectangle (state, floorInfo);
			Objects.Add (floor);
		}

		public void SelectObject (IGameObject obj, GameTime gameTime)
		{
			if (SelectedObject != obj) {
				if (SelectedObject != null) {
					SelectedObject.OnUnselected (gameTime);
				}
				SelectedObject = obj; 
				if (SelectedObject != null) {
					SelectedObject.OnSelected (gameTime);
				}
			}
		}

		public float SelectedObjectDistance ()
		{
			if (SelectedObject != null) {
				Vector3 toTarget = SelectedObject.Center () - camera.Position;
				return toTarget.Length ();
			} else {
				return 0;
			}
		}
		
		public void Add (IGameObject obj)
		{
			Objects.Add (obj);
		}

		public override void Initialize ()
		{
			// knot render effects
			knotRenderEffects = new List<RenderEffect> ();
			knotRenderEffects.Add (new NoEffect (state));
			knotRenderEffects.Add (new BlurEffect (state));
			knotRenderEffects.Add (new CelShadingEffect (state));

			if (Options.Default ["video", "cel-shading", true]) {
				knotRenderEffect = new CelShadingEffect (state);
			} else {
				knotRenderEffect = new NoEffect (state);
			}
		}
		
		public override void Draw (GameTime gameTime)
		{
			Color background = knotRenderEffect is CelShadingEffect ? Color.CornflowerBlue : Color.Black;
			// begin the knot render effect
			knotRenderEffect.Begin (background, gameTime);

			foreach (IGameObject obj in Objects) {
				obj.Draw (gameTime);
			}

			// end of the knot render effect
			knotRenderEffect.End (gameTime);
		}

		public override void Update (GameTime gameTime)
		{
			// run the update method on all game objects
			foreach (IGameObject obj in Objects) {
				obj.Update (gameTime);
			}

			// post processing effects
			if (Keys.O.IsDown ()) {
				knotRenderEffect = knotRenderEffects [
				    (knotRenderEffects.IndexOf (knotRenderEffect) + 1) % knotRenderEffects.Count
				];
			}

			// spawn a game object
			if (Keys.Z.IsDown ()) {
				//objects.Add (new GameModel (state, "Test3D", new Vector3 (-200, 200, 200), 0.1f));
				var info = new GameModelInfo ("Test3D");
				info.Position = new Vector3 (200, 200, 200);
				info.Scale = 0.1f;
				info.IsMovable = true;
				var obj = new MovableGameObject (state, new TestModel (state, info));
				Objects.Add (obj);
			}
			if (Keys.P.IsDown ()) {
				//objects.Add (new GameModel (state, "Test3D", new Vector3 (-200, 200, 200), 0.1f));
				var info = new GameModelInfo ("pipe1");
				info.Position = new Vector3 (-200, 200, -200);
				info.Scale = 30f;
				info.IsMovable = true;
				var obj = new MovableGameObject (state, new TestModel (state, info));
				Objects.Add (obj);
			}

			// is the floor visible?
			floor.Info.IsVisible = Options.Default ["video", "debug-floor", false];
		}
	}
	
	public class TestModel : GameModel
	{
		public TestModel (GameState state, GameModelInfo info)
			: base(state, info)
		{
		}

		public override void Update (GameTime gameTime)
		{
			if (Keys.U.IsHeldDown ()) {
				Info.Position = Info.Position.RotateY (MathHelper.PiOver4 / 100f);
			}
			base.Update (gameTime);
		}
	}
}

