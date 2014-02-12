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

using System.IO;

namespace Knot3.CreativeMode
{
	/// <summary>
	/// Der Creative-Modus, in dem ein Knoten erstellt und bearbeitet werden kann.
	/// </summary>
	public class CreativeModeScreen : GameScreen
	{
		// the knot to draw
		private Knot knot;
		private bool knotModified;

		// the game components
		private World world;
		private KnotInputHandler knotInput;
		private MousePointer pointer;
		private Overlay overlay;
		private ModelMouseHandler picker;
		private EdgeMovement movement;
		private EdgeColoring coloring;
		private KnotRenderer renderer;
		private Dialog dialog;

		/// <summary>
		/// Initializes a new instance of the <see cref="TestGame1.ConstructionMode"/> class.
		/// </summary>
		/// <param name='game'>
		/// Game.
		/// </param>
		public CreativeModeScreen (Core.Knot3Game game)
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
			knotInput = new KnotInputHandler (this, world);
			// overlay
			overlay = new Overlay (this, world);
			// pointer
			pointer = new MousePointer (this);
			// picker
			picker = new ModelMouseHandler (this, world);

			// pipe renderer
			var knotRenderInfo = new GameObjectInfo ();
			knotRenderInfo.Position = Vector3.Zero;
			renderer = new KnotRenderer (this, knotRenderInfo);
			world.Add (renderer as IGameObject);

			// pipe movements
			movement = new EdgeMovement (this, world, knotRenderInfo);
			world.Add (movement as IGameObject);

			// pipe colors
			coloring = new EdgeColoring (this);

			// load nodes
			Node.Scale = 100;
			Knot = new Knot ();
		}

		public Knot Knot
		{
			get { return knot; }
			set {
				knot = value;
				renderer.Knot = knot;
				knot.EdgesChanged += () => knotModified = true;
				knot.EdgesChanged ();
				movement.Knot = knot;
				coloring.Knot = knot;
				knotModified = false;
			}
		}

		public void LoadFile (string file)
		{
			Console.WriteLine ("load file: " + file);
		}

		public override void Update (GameTime time)
		{
			if (dialog != null) {
				// dialog
			}
			else {
				UpdateInput (time);
			}
		}

		private void UpdateInput (GameTime time)
		{
			// when is escape is pressed, go to start screen
			if (Keys.Escape.IsDown ()) {
				if (knotModified) {
					if (dialog != null && dialog is KnotSaveConfirmDialog) {
						dialog.Done ();
					}
					else {
						dialog = new KnotSaveConfirmDialog (
						    screen: this,
						    info: new WidgetInfo (),
						    drawOrder: DisplayLayer.Dialog,
						    knot: knot
						);
						AddGameComponents (time, dialog);
						dialog.Done += () => {
							RemoveGameComponents (time, dialog);
							dialog = null;
						};
					}
				}
				else {
					NextState = GameScreens.StartScreen;
				}
				return;
			}

			// move edges
			if (Keys.NumPad8.IsDown ()) {
				knot.Move (Direction.Up);
			}
			if (Keys.NumPad2.IsDown ()) {
				knot.Move (Direction.Down);
			}
			if (Keys.NumPad4.IsDown ()) {
				knot.Move (Direction.Left);
			}
			if (Keys.NumPad6.IsDown ()) {
				knot.Move (Direction.Right);
			}
			if (Keys.NumPad7.IsDown ()) {
				knot.Move (Direction.Forward);
			}
			if (Keys.NumPad9.IsDown ()) {
				knot.Move (Direction.Backward);
			}

			if (PostProcessingEffect is FadeEffect && (PostProcessingEffect as FadeEffect).IsFinished) {
				PostProcessingEffect = new StandardEffect (this);
				world.Redraw = true;
			}
		}

		public override void Draw (GameTime time)
		{
			// all drawing is done in game components
		}

		public override void Entered (GameTime time)
		{
			base.Entered (time);
			AddGameComponents (time, knotInput, overlay, pointer, world, picker, coloring);
		}

		public override void Unload ()
		{
		}
	}

	public class KnotSaveConfirmDialog : TextInputDialog
	{
		public KnotSaveConfirmDialog (GameScreen screen, WidgetInfo info, DisplayLayer drawOrder, Knot knot)
		: base(screen, info, drawOrder)
		{
			Info.RelativeSize = () => new Vector2 (0.500f, 0.250f);
			Info.RelativePadding = () => new Vector2 (0.016f, 0.016f);
			Text = new string[] {
				"Do you want to save the changes?"
			};
			TextInput.InputText = knot.Name;
			string originalName = knot.Name;

			OnYesClick += () => {
				Console.WriteLine ("OnYesClick");

				if (TextInput.InputText.Length == 0) {
					Console.WriteLine ("Name is empty!");
					CanClose = false;
				}
				else if (originalName.Length > 0 && originalName == TextInput.InputText) {
					Console.WriteLine ("Name has not been changed: " + TextInput.InputText);
					try {
						knot.Save ();
					}
					catch (IOException ex) {
						Console.WriteLine (ex);
						knot.Save (new KnotFileIO (), knot.MetaData.Filename);
					}
					CanClose = true;
				}
				else {
					Console.WriteLine ("Name has been changed: " + TextInput.InputText);
					try {
						knot.Name = TextInput.InputText;
						knot.Save ();
					}
					catch (IOException ex) {
						Console.WriteLine (ex);
						knot.Save (new KnotFileIO (), knot.MetaData.Filename);
					}
					CanClose = true;
				}

				if (CanClose) {
					screen.NextState = GameScreens.StartScreen;
				}
			};
			OnNoClick += () => {
				Console.WriteLine ("OnNoClick");
				screen.NextState = GameScreens.StartScreen;
			};
		}
	}
}
