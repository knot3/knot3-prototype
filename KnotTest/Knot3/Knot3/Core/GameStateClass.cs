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

namespace Knot3.Core
{
	/// <summary>
	/// Standard-Implementierung von IGameStateClass. Hält eine Referenz auf einen bestimmten GameState und stellt
	/// Properties und Methoden für den direkten Zugriff auf häufig benötigte XNA-Klassen bereit.
	/// </summary>
	public abstract class GameStateClass : IGameStateClass
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Knot3.GameClass"/> class.
		/// </summary>
		/// <param name='state'>
		/// The game state.
		/// </param>
		public GameStateClass (GameState state)
		{
			this.state = state;
		}

		/// <summary>
		/// Gets the GameState associated with this object.
		/// </summary>
		/// <value>
		/// The Game state.
		/// </value>
		public GameState state { get; private set; }

		/// <summary>
		/// Gets the graphics device manager.
		/// </summary>
		/// <value>
		/// The graphics device manager.
		/// </value>
		public GraphicsDeviceManager graphics { get { return state.graphics; } }

		/// <summary>
		/// Gets the graphics device.
		/// </summary>
		/// <value>
		/// The graphics device.
		/// </value>
		public GraphicsDevice device { get { return state.device; } }

		/// <summary>
		/// Gets the viewport.
		/// </summary>
		/// <value>
		/// The viewport.
		/// </value>
		public Viewport viewport { get { return state.device.Viewport; } }

		/// <summary>
		/// Gets the content manager.
		/// </summary>
		/// <value>
		/// The content manager.
		/// </value>
		public ContentManager content { get { return state.content; } }

		/// <summary>
		/// Gets or sets the camera. Returns null of the game state is not in 3D mode!
		/// </summary>
		/// <value>
		/// The camera.
		/// </value>
		public Camera camera { get { return state.camera; } }

		/// <summary>
		/// Gets or sets the input handler.
		/// </summary>
		/// <value>
		/// The input handler.
		/// </value>
		public Input input { get { return state.input; } }

		/// <summary>
		/// Gets or sets the game world.
		/// </summary>
		/// <value>
		/// The game world.
		/// </value>
		public World world { get { return state.world; } }
	}
}

