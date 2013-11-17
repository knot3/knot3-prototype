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
	public class CreativeMode : GameState
	{
		// graphics-related classes
		public BasicEffect basicEffect;
		public List<PostProcessing> PostProcessingEffects;

		// nodes
		private NodeList nodes;
		private EdgeList edges;

		// custom classes
		private MousePointer pointer;
		private Overlay overlay;
		private DrawLines drawLines;
		private DrawPipes drawPipes;

		/// <summary>
		/// Initializes a new instance of the <see cref="TestGame1.ConstructionMode"/> class.
		/// </summary>
		/// <param name='game'>
		/// Game.
		/// </param>
		public CreativeMode (Game game)
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

			// post processing effects
			PostProcessingEffects = new List<PostProcessing> ();
			PostProcessingEffects.Add (new NoPostProcessing (this));
			PostProcessingEffects.Add (new BlurEffect (this));
			foreach (PostProcessing	pp in PostProcessingEffects) {
				pp.LoadContent ();
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

			// line drawing
			drawLines = new DrawLines (this);

			// pipe drawing
			drawPipes = new DrawPipes (this);
			
			// load nodes
			Node.Scale = 100;
			nodes = new NodeList ();
			edges = new EdgeList (nodes);
			edges.LinesChanged += () => {
				drawPipes.Update (edges);
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

			drawPipes.Update (edges);
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
			input.SaveStates (gameTime);
			// overlay
			overlay.Update (gameTime);
			// pointer
			pointer.Update (gameTime);

			return state;
		}

		private GameState UpdateInput (GameTime gameTime)
		{
			// when is escape is pressed, go to start screen
			if (Keys.Escape.IsDown ()) {
				return GameStates.StartScreen;
			}

			// change background color
			//if (Keys.Space.IsDown ()) {
			//	backColor = new Color (backColor.R, backColor.G, (byte)~backColor.B);
			//}

			// move lines
			if (Keys.NumPad8.IsDown ())
				edges.InsertAt (edges.SelectedEdges, Vector3.Up);
			if (Keys.NumPad2.IsDown ())
				edges.InsertAt (edges.SelectedEdges, Vector3.Down);
			if (Keys.NumPad4.IsDown ())
				edges.InsertAt (edges.SelectedEdges, Vector3.Left);
			if (Keys.NumPad6.IsDown ())
				edges.InsertAt (edges.SelectedEdges, Vector3.Right);
			if (Keys.NumPad7.IsDown ())
				edges.InsertAt (edges.SelectedEdges, Vector3.Forward);
			if (Keys.NumPad9.IsDown ())
				edges.InsertAt (edges.SelectedEdges, Vector3.Backward);

			// post processing effects
			if (Keys.O.IsDown ())
				PostProcessing = PostProcessingEffects [(PostProcessingEffects.IndexOf (PostProcessing) + 1) % PostProcessingEffects.Count];

			// toggle debug mode
			if (Keys.RightControl.IsDown())
				Game.Debug = !Game.Debug;

			return this;
		}

		public override void Draw (GameTime gameTime)
		{
			PostProcessing.Begin (gameTime);

			graphics.GraphicsDevice.Clear (Game.Debug ? Color.CornflowerBlue : Color.Black);
			basicEffect.CurrentTechnique.Passes [0].Apply ();

			game.GraphicsDevice.BlendState = BlendState.Opaque;
			game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

			drawPipes.Draw (gameTime);
			world.Draw (gameTime);
			basicEffect.CurrentTechnique.Passes [0].Apply ();
			drawLines.Draw (edges, gameTime);
			basicEffect.CurrentTechnique.Passes [0].Apply ();
			overlay.Draw (gameTime);
			pointer.Draw (gameTime);
			basicEffect.CurrentTechnique.Passes [0].Apply ();
			camera.Draw (basicEffect, gameTime);

			PostProcessing.End (gameTime);
		}

		public override void Unload ()
		{
		}
	}
}

