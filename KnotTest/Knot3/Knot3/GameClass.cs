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

namespace Knot3
{
	/// <summary>
	/// The abstract class GameClass holds a reference to the associated GameState and provides
	/// convenience methods for subclasses.
	/// </summary>
	public abstract class GameClass
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Knot3.GameClass"/> class.
		/// </summary>
		/// <param name='state'>
		/// The game state.
		/// </param>
		public GameClass (GameState state)
		{
			this.state = state;
		}

		/// <summary>
		/// Gets the GameState associated with this object.
		/// </summary>
		/// <value>
		/// The Game state.
		/// </value>
		protected GameState state { get; private set; }

		/// <summary>
		/// Gets the graphics device manager.
		/// </summary>
		/// <value>
		/// The graphics device manager.
		/// </value>
		protected GraphicsDeviceManager graphics { get { return state.graphics; } }

		/// <summary>
		/// Gets the graphics device.
		/// </summary>
		/// <value>
		/// The graphics device.
		/// </value>
		protected GraphicsDevice device { get { return state.device; } }

		/// <summary>
		/// Gets the viewport.
		/// </summary>
		/// <value>
		/// The viewport.
		/// </value>
		protected Viewport viewport { get { return state.device.Viewport; } }

		/// <summary>
		/// Gets the content manager.
		/// </summary>
		/// <value>
		/// The content manager.
		/// </value>
		protected ContentManager content { get { return state.content; } }

		/// <summary>
		/// Gets or sets the camera. Returns null of the game state is not in 3D mode!
		/// </summary>
		/// <value>
		/// The camera.
		/// </value>
		protected virtual Camera camera {
			get { return state.camera; }
			set {}
		}

		/// <summary>
		/// Gets or sets the input handler.
		/// </summary>
		/// <value>
		/// The input handler.
		/// </value>
		protected virtual Input input {
			get { return state.input; }
			set {}
		}

		/// <summary>
		/// Gets or sets the game world.
		/// </summary>
		/// <value>
		/// The game world.
		/// </value>
		protected virtual World world {
			get { return state.world; }
			set {}
		}
	}
}

