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
using System.Collections;

namespace Knot3.GameObjects
{
	/// <summary>
	/// Eine Liste von Spielobjekten (Interface IGameObject), die in einer 3D-Welt gezeichnet werden. Ruft die Update()-
	/// und Draw()-Methoden der Spielobjekte auf.
	/// </summary>
	public class World : DrawableGameStateComponent, IEnumerable<IGameObject>
	{
		// graphics-related classes
		private List<RenderEffect> knotRenderEffects;
		private RenderEffect knotRenderEffect;

		/// <summary>
		/// Die Liste von Spielobjekten.
		/// </summary>
		/// <value>
		/// The objects.
		/// </value>
		public List<IGameObject> Objects { get; private set; }

		/// <summary>
		/// Das selektierte Spielobjekt, wobei selektiert nur bedeutet, dass die Maus darüber liegt.
		/// </summary>
		/// <value>
		/// The selected object.
		/// </value>
		public IGameObject SelectedObject { get; private set; }

		/// <summary>
		/// Gets the camera for this game world.
		/// </summary>
		/// <value>
		/// The camera.
		/// </value>
		public Camera Camera { get; protected set; }

		private TexturedRectangle floor;

		/// <summary>
		/// Initializes a new Overlay
		/// </summary>
		public World (GameState state)
			: base(state, DisplayLayer.World)
		{
			// camera
			Camera = new Camera (state, this);

			// the list of game objects
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
				SelectedObject = obj;
				Redraw = true;
			}
		}

		public float SelectedObjectDistance ()
		{
			if (SelectedObject != null) {
				Vector3 toTarget = SelectedObject.Center () - Camera.Position;
				return toTarget.Length ();
			} else {
				return 0;
			}
		}

		public void Add (IGameObject obj)
		{
			Objects.Add (obj);
			obj.World = this;
		}

		public override void Initialize ()
		{
			// knot render effects
			knotRenderEffects = new List<RenderEffect> ();
			knotRenderEffects.Add (new InstancingTest (state));
			knotRenderEffects.Add (new NoEffect (state));
			knotRenderEffects.Add (new BlurEffect (state));
			knotRenderEffects.Add (new CelShadingEffect (state));

			if (Options.Default ["video", "cel-shading", true]) {
				knotRenderEffect = new CelShadingEffect (state);
			} else {
				knotRenderEffect = new NoEffect (state);
			}
		}

		private bool _redraw = true;

		public bool Redraw {
			get { return _redraw; }
			set { _redraw = value; }
		}
		
		public override void Draw (GameTime gameTime)
		{
			if (Redraw) {
				Redraw = false;

				// begin the post processing effect scope
				Color background = knotRenderEffect is CelShadingEffect ? Color.CornflowerBlue : Color.Black;
				state.PostProcessing.Begin (background, gameTime);

				// begin the knot render effect
				knotRenderEffect.Begin (gameTime);

				foreach (IGameObject obj in Objects) {
					obj.World = this;
					obj.Draw (gameTime);
				}

				// end of the knot render effect
				knotRenderEffect.End (gameTime);
			
				// end of the post processing effect
				state.PostProcessing.End (gameTime);
			} else {
				state.PostProcessing.DrawLastFrame (gameTime);
			}
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
				Redraw = true;
			}

			if (Keys.N.IsDown ()) {
				Redraw = true;
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
				Redraw = true;
			}
			if (Keys.P.IsDown ()) {
				//objects.Add (new GameModel (state, "Test3D", new Vector3 (-200, 200, 200), 0.1f));
				var info = new GameModelInfo ("pipe1");
				info.Position = new Vector3 (-200, 200, -200);
				info.Scale = 30f;
				info.IsMovable = true;
				var obj = new MovableGameObject (state, new TestModel (state, info));
				Objects.Add (obj);
				Redraw = true;
			}

			// is the floor visible?
			floor.Info.IsVisible = Options.Default ["video", "debug-floor", false];
		}
		
		public IEnumerator<IGameObject> GetEnumerator ()
		{
			foreach (IGameObject obj in Objects) {
				yield return obj;
			}
		}

		// Explicit interface implementation for nongeneric interface
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator (); // Just return the generic version
		}

		public override IEnumerable<IGameStateComponent> SubComponents (GameTime gameTime)
		{
			foreach (DrawableGameStateComponent component in base.SubComponents(gameTime)) {
				yield return component;
			}
			yield return Camera;
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

