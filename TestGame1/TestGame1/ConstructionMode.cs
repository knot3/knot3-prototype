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

namespace TestGame1
{
	public class ConstructionMode : GameState
	{
		// graphics-related classes
		public BasicEffect basicEffect;

		// nodes
		private NodeList nodes;
		private LineList lines;

		// colors, sizes, ...
		private Color backColor = Color.CornflowerBlue;

		// custom classes

		protected Overlay overlay { get; set; }

		private DrawLines drawLines;
		private DrawPipes drawPipes;

		/// <summary>
		/// Initializes a new instance of the <see cref="TestGame1.ConstructionMode"/> class.
		/// </summary>
		/// <param name='game'>
		/// Game.
		/// </param>
		public ConstructionMode (Game game)
			: base(game)
		{
		}

		/// <summary>
		/// Initialize the Game.
		/// </summary>
		public override void Initialize ()
		{
			// basic effect
			basicEffect = new BasicEffect (game.GraphicsDevice);
			basicEffect.VertexColorEnabled = true;

			// camera
			camera = new Camera (this);

			// input
			input = new Input (this);
			input.SaveStates ();

			// overlay
			overlay = new Overlay (this);

			// overlay
			world = new World (this);

			// line drawing
			drawLines = new DrawLines (this);

			// pipe drawing
			drawPipes = new DrawPipes (this);
			
			// load nodes
			Node.Scale = 100;
			nodes = new NodeList ();
			lines = new LineList (nodes);
			lines.LinesChanged += () => {
				drawPipes.Update (lines);
			};

			// load camera
			camera.LoadContent ();

			// load overlay
			overlay.LoadContent ();

			// add some default nodes
			nodes.Add (new Node (0, 0, 0));
			nodes.Add (new Node (0, 1, 0));
			nodes.Add (new Node (1, 1, 0));
			nodes.Add (new Node (1, 0, 0));

			nodes.Add (new Node (1, 0, 1));
			nodes.Add (new Node (1, 1, 1));
			nodes.Add (new Node (0, 1, 1));
			nodes.Add (new Node (0, 0, 1));

			drawPipes.Update (lines);
		}

		public override GameState Update (GameTime gameTime)
		{
			GameState state = UpdateInput (gameTime);

			// camera
			camera.Update (gameTime);
			// input
			input.Update (gameTime);
			// world
			world.Update (gameTime);
			input.SaveStates ();
			// overlay
			overlay.Update (gameTime);

			return state;
		}

		private GameState UpdateInput (GameTime gameTime)
		{
			// allows the game to exit
			if (Keys.Escape.IsDown ()) {
				return GameStates.StartScreen;
			}

			// change background color
			if (Keys.Space.IsDown ()) {
				backColor = new Color (backColor.R, backColor.G, (byte)~backColor.B);
			}

			// select lines
			if (Keys.Y.IsDown () || Keys.NumPad1.IsDown ()) {
				lines.SelectedLine -= 1;
			} else if (Keys.X.IsDown () || Keys.NumPad3.IsDown ()) {
				lines.SelectedLine += 1;
			}

			// move lines
			if (Keys.NumPad8.IsDown ())
				lines.InsertAt (lines.SelectedLine, Vector3.Up);
			if (Keys.NumPad2.IsDown ())
				lines.InsertAt (lines.SelectedLine, Vector3.Down);
			if (Keys.NumPad4.IsDown ())
				lines.InsertAt (lines.SelectedLine, Vector3.Left);
			if (Keys.NumPad6.IsDown ())
				lines.InsertAt (lines.SelectedLine, Vector3.Right);
			if (Keys.NumPad7.IsDown ())
				lines.InsertAt (lines.SelectedLine, Vector3.Forward);
			if (Keys.NumPad9.IsDown ())
				lines.InsertAt (lines.SelectedLine, Vector3.Backward);
			
			return this;
		}

		public override void Draw (GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear (backColor);
			basicEffect.CurrentTechnique.Passes [0].Apply ();

			game.GraphicsDevice.BlendState = BlendState.Opaque;
			game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

			drawPipes.Draw (gameTime);
			world.Draw (gameTime);
			basicEffect.CurrentTechnique.Passes [0].Apply ();
			drawLines.Draw (lines, gameTime);
			basicEffect.CurrentTechnique.Passes [0].Apply ();
			overlay.Draw (gameTime);
			basicEffect.CurrentTechnique.Passes [0].Apply ();
			camera.Draw (basicEffect, gameTime);
		}

		public override void Unload ()
		{
		}
	}
}

