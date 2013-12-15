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

using Knot3.GameObjects;
using Knot3.RenderEffects;
using Knot3.Settings;
using Knot3.CreativeMode;
using Knot3.UserInterface;

namespace Knot3.Core
{
	/// <summary>
	/// Die abstrakte Klasse GameScreen steht für einen bestimmten Zustand des Spiels und verwaltet
	/// eine Liste von IGameScreenComponent-Objekten, die einzelne Komponenten zeichenen und auf Eingaben reagieren.
	/// </summary>
	public abstract class GameScreen
	{
		/// <summary>
		/// The game.
		/// </summary>
		public Game game;

		/// <summary>
		/// Gets or sets the next game screen.
		/// </summary>
		/// <value>
		/// The next game screen.
		/// </value>
		public GameScreen NextState { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Knot3.GameScreen"/> class.
		/// </summary>
		/// <param name='game'>
		/// The Game.
		/// </param>
		public GameScreen (Game game)
		{
			this.game = game;
			this.NextState = this;
			this.RenderEffects = new RenderEffectStack (defaultEffect: new NoEffect (this));
			this.PostProcessing = new NoEffect (this);
			this.input = new InputManager (this);
		}

		/// <summary>
		/// Gets the basic input handler for this game screen.
		/// </summary>
		/// <value>
		/// The input handler.
		/// </value>
		public InputManager input { get; set; }

		/// <summary>
		/// Gets the graphics device manager for this game screen.
		/// </summary>
		/// <value>
		/// The graphics device manager.
		/// </value>
		public GraphicsDeviceManager graphics { get { return game.graphics; } }

		/// <summary>
		/// Gets the graphics device for this game screen.
		/// </summary>
		/// <value>
		/// The graphics device.
		/// </value>
		public GraphicsDevice device { get { return game.GraphicsDevice; } }

		/// <summary>
		/// Gets the viewport for this game screen.
		/// </summary>
		/// <value>
		/// The viewport.
		/// </value>
		public Viewport viewport { get { return device.Viewport; } }

		/// <summary>
		/// Gets the content manager for this game screen.
		/// </summary>
		/// <value>
		/// The content manager.
		/// </value>
		public ContentManager content { get { return game.Content; } }

		/// <summary>
		/// Gets the currently active render effects for this game screen.
		/// </summary>
		/// <value>
		/// The currently active render effects.
		/// </value>
		public RenderEffectStack RenderEffects { get; private set; }

		/// <summary>
		/// The post processing effect of this game screen.
		/// </summary>
		public RenderEffect PostProcessing;

		/// <summary>
		/// Initialize the game screen.
		/// </summary>
		public abstract void Initialize ();

		/// <summary>
		/// Update the game screen.
		/// </summary>
		/// <param name='gameTime'>
		/// The Game time.
		/// </param>
		public abstract void Update (GameTime gameTime);

		/// <summary>
		/// Draw the game screen.
		/// </summary>
		/// <param name='gameTime'>
		/// The Game time.
		/// </param>
		public abstract void Draw (GameTime gameTime);

		/// <summary>
		/// Unload the game screen.
		/// </summary>
		public abstract void Unload ();

		/// <summary>
		/// Adds game components.
		/// </summary>
		/// <param name='components'>
		/// Game Components.
		/// </param>
		public void AddGameComponents (GameTime gameTime, params IGameScreenComponent[] components)
		{
			foreach (IGameScreenComponent component in components) {
				//Console.WriteLine ("AddGameComponents: " + component);
				game.Components.Add (component);
				AddGameComponents (gameTime, component.SubComponents (gameTime).ToArray ());
			}
		}

		/// <summary>
		/// Removes game components.
		/// </summary>
		/// <param name='components'>
		/// Game Components.
		/// </param>
		public void RemoveGameComponents (GameTime gameTime, params IGameScreenComponent[] components)
		{
			foreach (IGameScreenComponent component in components) {
				Console.WriteLine ("RemoveGameComponents: " + component);
				RemoveGameComponents (gameTime, component.SubComponents (gameTime).ToArray ());
				game.Components.Remove (component);
			}
		}

		/// <summary>
		/// This is run when this game screen becomes the active game screen.
		/// </summary>
		/// <param name='gameTime'>
		/// The Game time.
		/// </param>
		public virtual void Activate (GameTime gameTime)
		{
			Console.WriteLine ("Activate: " + this);
			AddGameComponents (gameTime, input, new WidgetKeyHandler (this), new WidgetMouseHandler (this));
		}

		/// <summary>
		/// This is run when this game screen is about to becomes inactive.
		/// </summary>
		/// <param name='gameTime'>
		/// The Game time.
		/// </param>
		public virtual void Deactivate (GameTime gameTime)
		{
			Console.WriteLine ("Deactivate: " + this);
			game.Components.Clear ();
		}
	}

	/// <summary>
	/// Verwaltet Referenzen auf alle dauerhaft im Speicher liegenden GameScreen-Objekte des Spiels.
	/// </summary>
	public static class GameScreens
	{
		public static CreativeModeScreen CreativeMode;
		public static StartScreen StartScreen;
		public static OptionScreen OptionScreen;
		public static VideoOptionScreen VideoOptionScreen;
		public static LoadSavegameScreen LoadSavegameScreen;

		public static void Initialize (Game game)
		{
			CreativeMode = new CreativeModeScreen (game);
			StartScreen = new StartScreen (game);
			OptionScreen = new OptionScreen (game);
			VideoOptionScreen = new VideoOptionScreen (game);
			LoadSavegameScreen = new LoadSavegameScreen (game);
			CreativeMode.Initialize ();
			StartScreen.Initialize ();
			OptionScreen.Initialize ();
			VideoOptionScreen.Initialize ();
			LoadSavegameScreen.Initialize ();
		}
	}
}

