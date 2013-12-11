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

using Knot3.Core;

namespace Knot3.CreativeMode
{
	/// <summary>
	/// Der Creative-Modus, in dem ein Knoten erstellt und bearbeitet werden kann.
	/// </summary>
	public class CreativeModeScreen : GameState
	{
		// the knot to draw
		private Knot knot;
		private bool knotModified;

		// the game components
		private World world;
		private MousePointer pointer;
		private Overlay overlay;
		private MousePicker picker;
		private PipeMovement movement;
		private PipeColoring coloring;
		private LineRenderer lineRenderer;
		private PipeRenderer pipeRenderer;
		private Dialog dialog;

		/// <summary>
		/// Initializes a new instance of the <see cref="TestGame1.ConstructionMode"/> class.
		/// </summary>
		/// <param name='game'>
		/// Game.
		/// </param>
		public CreativeModeScreen (Core.Game game)
			: base(game)
		{
		}

		/// <summary>
		/// Initialize the Game.
		/// </summary>
		public override void Initialize ()
		{
			// world
			world = new World (this);
			// input
			input = new KnotModeInput (this, world);
			// overlay
			overlay = new Overlay (this, world);
			// pointer
			pointer = new MousePointer (this);
			// picker
			picker = new MousePicker (this, world);

			// pipe renderer
			var knotRenderInfo = new GameObjectInfo ();
			knotRenderInfo.Position = Vector3.Zero;
			pipeRenderer = new PipeRenderer (this, knotRenderInfo);
			world.Add (pipeRenderer as IGameObject);
			
			// pipe movements
			movement = new PipeMovement (this, world, knotRenderInfo);
			world.Add (movement as IGameObject);

			// pipe colors
			coloring = new PipeColoring (this);

			// line renderer
			lineRenderer = new LineRenderer (this, knotRenderInfo);
			world.Add (lineRenderer as IGameObject);
			
			// load nodes
			Node.Scale = 100;
			Knot = Knot.DefaultKnot (new EdgeListFormat ());
		}

		public Knot Knot {
			get { return knot; }
			set {
				knot = value;
				knot.EdgesChanged += pipeRenderer.OnEdgesChanged;
				knot.EdgesChanged += lineRenderer.OnEdgesChanged;
				knot.EdgesChanged += (e) => knotModified = true;
				knot.EdgesChanged (knot.Edges);
				movement.Knot = knot;
				coloring.Knot = knot;
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
						dialog = new KnotSaveConfirmDialog (
							state: this, 
							info: new WidgetInfo (),
							drawOrder: DisplayLayer.Dialog,
							knot: knot
						);
						AddGameComponents (gameTime, dialog);
						dialog.Done += () => {
							RemoveGameComponents (gameTime, dialog);
							dialog = null;
						};
					}
				} else {
					NextState = GameStates.StartScreen;
				}
				return;
			}

			// move edges
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
				world.Redraw = true;
			}
		}

		public override void Draw (GameTime gameTime)
		{
			// all drawing is done in game components
		}

		public override void Activate (GameTime gameTime)
		{
			base.Activate (gameTime);
			AddGameComponents (gameTime, input, overlay, pointer, world, picker, coloring);
		}

		public override void Unload ()
		{
		}
	}

	public class KnotSaveConfirmDialog : TextInputDialog
	{
		public KnotSaveConfirmDialog (GameState state, WidgetInfo info, DisplayLayer drawOrder, Knot knot)
			: base(state, info, drawOrder)
		{
			Info.RelativeSize = () => new Vector2 (0.500f, 0.250f);
			Info.RelativePadding = () => new Vector2 (0.016f, 0.016f);
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

