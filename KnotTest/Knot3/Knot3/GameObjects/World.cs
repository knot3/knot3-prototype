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
	public class World : DrawableGameScreenComponent, IEnumerable<IGameObject>
	{
		// graphics-related classes
		private List<IRenderEffect> effects;
		private IRenderEffect currentEffect;

		/// <summary>
		/// Die Liste von Spielobjekten.
		/// </summary>
		/// <value>
		/// The objects.
		/// </value>
		public List<IGameObject> Objects { get; private set; }

		private IGameObject _selectedObject;

		/// <summary>
		/// Das selektierte Spielobjekt, wobei selektiert nur bedeutet, dass die Maus darüber liegt.
		/// </summary>
		/// <value>
		/// The selected object.
		/// </value>
		public IGameObject SelectedObject {
			get {
				return _selectedObject;
			}
			private set {
				_selectedObject = value;
				SelectionChanged (_selectedObject);
				Redraw = true;
			}
		}

		/// <summary>
		/// When the selection is changed.
		/// </summary>
		public Action<IGameObject> SelectionChanged = (o) => {};

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
		public World (GameScreen screen)
			: base(screen, DisplayLayer.World)
		{
			// camera
			Camera = new Camera (screen, this);

			// the list of game objects
			Objects = new List<IGameObject> ();

			// the floor
			Vector3 size = new Vector3 (2000, 0, 2000);
			Vector3 position = new Vector3 (-1000, -100, -1000);
			var floorInfo = new TexturedRectangleInfo (
				texturename: "floor", origin: position + new Vector3 (size.X, 0, size.Z) / 2,
				left: Vector3.Left, width: size.X, up: Vector3.Forward, height: size.Z
			) {
				IsVisible = false
			};
			floor = new TexturedRectangle (screen, floorInfo);
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
			effects = new List<IRenderEffect> ();
			effects.Add (new InstancingTest (screen));
			effects.Add (new NoEffect (screen));
			effects.Add (new BlurEffect (screen));
			effects.Add (new CelShadingEffect (screen));

			if (Options.Default ["video", "cel-shading", true]) {
				currentEffect = new CelShadingEffect (screen);
			} else {
				currentEffect = new NoEffect (screen);
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
				Color background = currentEffect is CelShadingEffect ? Color.CornflowerBlue : Color.Black;
				screen.PostProcessing.Begin (background, gameTime);

				// begin the knot render effect
				currentEffect.Begin (gameTime);

				foreach (IGameObject obj in Objects) {
					obj.World = this;
					obj.Draw (gameTime);
				}

				// end of the knot render effect
				currentEffect.End (gameTime);
			
				// end of the post processing effect
				screen.PostProcessing.End (gameTime);
			} else {
				screen.PostProcessing.DrawLastFrame (gameTime);
			}
		}

		public override void Update (GameTime gameTime)
		{
			if (screen.PostProcessing is FadeEffect)
				Redraw = true;

			// run the update method on all game objects
			foreach (IGameObject obj in Objects) {
				obj.Update (gameTime);
			}

			// post processing effects
			if (Keys.O.IsDown ()) {
				currentEffect = effects [(effects.IndexOf (currentEffect) + 1) % effects.Count];
				Redraw = true;
			}

			if (Keys.N.IsDown ()) {
				Redraw = true;
			}

			// spawn a game object
			if (Keys.Z.IsDown ()) {
				//objects.Add (new GameModel (screen, "Test3D", new Vector3 (-200, 200, 200), 0.1f));
				var info = new GameModelInfo ("Test3D");
				info.Position = new Vector3 (200, 200, 200);
				info.Scale = Vector3.One * 0.1f;
				info.IsMovable = true;
				var obj = new MovableGameObject (screen, new TestModel (screen, info));
				Objects.Add (obj);
				Redraw = true;
			}
			if (Keys.P.IsDown ()) {
				//objects.Add (new GameModel (screen, "Test3D", new Vector3 (-200, 200, 200), 0.1f));
				var info = new GameModelInfo ("pipe1");
				info.Position = new Vector3 (-200, 200, -200);
				info.Scale = Vector3.One * 30f;
				info.IsMovable = true;
				var obj = new MovableGameObject (screen, new TestModel (screen, info));
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

		public override IEnumerable<IGameScreenComponent> SubComponents (GameTime gameTime)
		{
			foreach (DrawableGameScreenComponent component in base.SubComponents(gameTime)) {
				yield return component;
			}
			yield return Camera;
		}
	}
	
	public class TestModel : GameModel
	{
		public TestModel (GameScreen screen, GameModelInfo info)
			: base(screen, info)
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

