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
using Knot3.Utilities;

namespace Knot3.CreativeMode
{
	public class CreativeModeScreen : GameState
	{
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
			// camera
			camera = new Camera (this);
			AddGameComponents (camera);

			// input
			input = new KnotModeInput (this);
			AddGameComponents (input);

			// overlay
			overlay = new Overlay (this);
			AddGameComponents (overlay);

			// pointer
			pointer = new MousePointer (this);
			AddGameComponents (pointer);

			// world
			world = new World (this);
			AddGameComponents (world);

			// pipe renderer
			var knotRenderInfo = new GameObjectInfo();
			knotRenderInfo.Position = Vector3.Zero;
			pipeRenderer = new PipeRenderer (this, knotRenderInfo);
			world.Add (pipeRenderer);

			// line renderer
			lineRenderer = new LineRenderer (this, knotRenderInfo);
			world.Add (lineRenderer);
			
			// load nodes
			Node.Scale = 100;
			Knot = Knot.DefaultKnot (new KnotFormat ());
		}

		public Knot Knot {
			get { return knot; }
			set {
				knot = value;
				knot.EdgesChanged += pipeRenderer.OnEdgesChanged;
				knot.EdgesChanged += lineRenderer.OnEdgesChanged;
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
			if (dialog != null) {
				// dialog
				dialog.Update (gameTime);
			} else {
				UpdateInput (gameTime);
			}
		}

		private void UpdateInput (GameTime gameTime)
		{
			// when is escape is pressed, go to start screen
			if (Keys.Escape.IsDown ()) {
				if (knotModified) {
					if (dialog != null && dialog is KnotSaveConfirmDialog) {
						dialog.Done ();
					} else {
						dialog = new KnotSaveConfirmDialog (this, DisplayLayer.Dialog, knot);
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

			if (PostProcessing is FadeEffect && (PostProcessing as FadeEffect).IsFinished) {
				PostProcessing = new NoEffect (this);
			}
		}

		public override void Draw (GameTime gameTime)
		{
			// begin the post processing effect scope
			Color background = Color.Black;
			PostProcessing.Begin (background, gameTime);

			if (dialog != null) {
				// draw the current dialog
				dialog.Draw (gameTime);
			}

			// end of the post processing effect
			PostProcessing.End (gameTime);
		}

		public override void Unload ()
		{
		}
	}

	public class KnotSaveConfirmDialog : TextInputDialog
	{
		public KnotSaveConfirmDialog (GameState state, DisplayLayer drawOrder, Knot knot)
			: base(state, drawOrder)
		{
			RelativeSize = () => new Vector2 (0.500f, 0.250f);
			Text = new string[] {
				"Do you want to save the changes?"
			};
			TextInput.InputText = knot.Info.Name;
			
			OnYesClick += () => {
				Console.WriteLine ("OnYesClick");
				knot.Rename (TextInput.InputText);
				knot.Save ();
				state.NextState = GameStates.StartScreen;
			};
			OnNoClick += () => {
				Console.WriteLine ("OnNoClick");
				state.NextState = GameStates.StartScreen;
			};
		}
	}
}

