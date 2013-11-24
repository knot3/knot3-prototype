using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Knot3.UserInterface;
using Knot3.KnotData;
using Knot3.RenderEffects;
using Knot3.GameObjects;
using Knot3.Settings;

namespace Knot3.CreativeMode
{
	public class CreativeModeScreen : GameState
	{
		// graphics-related classes
		public List<RenderEffect> RenderEffects;
		public RenderEffect KnotRenderEffect;

		// the knot to draw
		private Knot knot;
		private bool knotModified;

		// custom classes
		private MousePointer pointer;
		private Overlay overlay;
		private LineRenderer lineRenderer;
		private PipeRenderer pipeRenderer;
		private Dialog dialog;

		/// <summary>
		/// Initializes a new instance of the <see cref="TestGame1.ConstructionMode"/> class.
		/// </summary>
		/// <param name='game'>
		/// Game.
		/// </param>
		public CreativeModeScreen (Game game)
			: base(game)
		{
		}

		/// <summary>
		/// Initialize the Game.
		/// </summary>
		public override void Initialize ()
		{
			// knot render effects
			RenderEffects = new List<RenderEffect> ();
			RenderEffects.Add (new NoRenderEffect (this));
			RenderEffects.Add (new BlurEffect (this));
			RenderEffects.Add (new CelShadingEffect (this));

			if (Options.Default ["video", "cel-shading", true]) {
				KnotRenderEffect = new CelShadingEffect (this);
			} else {
				KnotRenderEffect = new NoRenderEffect (this);
			}

			// camera
			camera = new Camera (this);

			// input
			input = new KnotModeInput (this);
			input.SaveStates (null);

			// overlay
			overlay = new Overlay (this);

			// pointer
			pointer = new MousePointer (this);

			// world
			world = new World (this);

			// line renderer
			lineRenderer = new LineRenderer (this);

			// pipe renderer
			pipeRenderer = new PipeRenderer (this);
			world.Add (pipeRenderer);
			
			// load nodes
			Node.Scale = 100;
			Knot = Knot.DefaultKnot ((knt) => {});

			// load camera
			camera.LoadContent ();

			// load overlay
			overlay.LoadContent ();

			
			knotModified = true;
		}

		public Knot Knot {
			get { return knot; }
			set {
				knot = value;
				knot.EdgesChanged += pipeRenderer.OnEdgesChanged;
				knot.EdgesChanged += (e) => knotModified = true;
				knot.EdgesChanged (knot.Edges);
				knotModified = false;
			}
		}

		public void LoadFile (string file)
		{
			Console.WriteLine ("load file: " + file);
		}

		public override void Update (GameTime gameTime)
		{
			UpdateInput (gameTime);

			// camera
			camera.Update (gameTime);
			if (dialog != null) {
				// dialog
				dialog.Update (gameTime);
			} else {
				// input
				input.Update (gameTime);
				// world
				world.Update (gameTime);
			}
			input.SaveStates (gameTime);
			// overlay
			overlay.Update (gameTime);
			// pointer
			pointer.Update (gameTime);
		}

		private void UpdateInput (GameTime gameTime)
		{
			// when is escape is pressed, go to start screen
			if (Keys.Escape.IsDown ()) {
				if (knotModified) {
					if (dialog != null && dialog is KnotSaveConfirmDialog) {
						dialog.Done ();
					} else {
						dialog = new KnotSaveConfirmDialog (this, knot);
						dialog.Done += () => dialog = null;
					}
				} else {
					NextState = GameStates.StartScreen;
				}
				return;
			}

			// change background color
			//if (Keys.Space.IsDown ()) {
			//	backColor = new Color (backColor.R, backColor.G, (byte)~backColor.B);
			//}

			// move lines
			if (Keys.NumPad8.IsDown ())
				knot.Edges.Move (knot.Edges.SelectedEdges, Vector3.Up);
			if (Keys.NumPad2.IsDown ())
				knot.Edges.Move (knot.Edges.SelectedEdges, Vector3.Down);
			if (Keys.NumPad4.IsDown ())
				knot.Edges.Move (knot.Edges.SelectedEdges, Vector3.Left);
			if (Keys.NumPad6.IsDown ())
				knot.Edges.Move (knot.Edges.SelectedEdges, Vector3.Right);
			if (Keys.NumPad7.IsDown ())
				knot.Edges.Move (knot.Edges.SelectedEdges, Vector3.Forward);
			if (Keys.NumPad9.IsDown ())
				knot.Edges.Move (knot.Edges.SelectedEdges, Vector3.Backward);

			// post processing effects
			if (Keys.O.IsDown ()) {
				KnotRenderEffect = RenderEffects [(RenderEffects.IndexOf (KnotRenderEffect) + 1) % RenderEffects.Count];
			}

			if (PostProcessing is FadeEffect && (PostProcessing as FadeEffect).IsFinished) {
				PostProcessing = new NoRenderEffect(this);
			}
		}

		public override void Draw (GameTime gameTime)
		{
			Color background = KnotRenderEffect is CelShadingEffect ? Color.CornflowerBlue : Color.Black;
			PostProcessing.Begin (background, gameTime);

			KnotRenderEffect.Begin (gameTime);

			world.Draw (gameTime);
			lineRenderer.Draw (knot.Edges, gameTime);

			KnotRenderEffect.End (gameTime);

			overlay.Draw (gameTime);
			if (dialog != null) {
				dialog.Draw (gameTime);
			}
			pointer.Draw (gameTime);

			PostProcessing.End (gameTime);
		}

		public override void Activate (GameTime gameTime)
		{
		}

		public override void Deactivate (GameTime gameTime)
		{
		}

		public override void Unload ()
		{
		}
	}

	public class KnotSaveConfirmDialog : ConfirmDialog
	{

		public KnotSaveConfirmDialog (GameState state, Knot knot)
			: base(state)
		{
			Size = () => new Vector2 (0.500f, 0.250f);
			Text = new string[] {
				"Do you want to save the changes?",
				knot.Info.Name
			};
			
			OnYesClick += () => {
				Console.WriteLine ("OnYesClick");
				knot.Save (knot);
				state.NextState = GameStates.StartScreen;
			};
			OnNoClick += () => {
				Console.WriteLine ("OnNoClick");
				state.NextState = GameStates.StartScreen;
			};
		}
	}
}

