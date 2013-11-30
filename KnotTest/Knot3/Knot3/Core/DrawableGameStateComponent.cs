using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Xna = Microsoft.Xna.Framework;
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
	/// Eine Implementierung von IGameStateComponent. Erbt von DrawableGameComponent aus XNA
	/// und hat daher außer der Update()-Methode auch eine Draw()-Methode.
	/// </summary>
	public abstract class DrawableGameStateComponent : Xna.DrawableGameComponent, IGameStateComponent
	{
		public DrawableGameStateComponent (GameState state, DisplayLayer drawOrder)
			: base(state.game)
		{
			this.state = state;
			this.DrawOrder = (int)drawOrder;
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
		/// Gets the camera. Returns null of the game state is not in 3D mode!
		/// </summary>
		/// <value>
		/// The camera.
		/// </value>
		public Camera camera { get { return state.camera; } }

		/// <summary>
		/// Gets the input handler.
		/// </summary>
		/// <value>
		/// The input handler.
		/// </value>
		public Input input { get { return state.input; } }

		/// <summary>
		/// Gets the game world.
		/// </summary>
		/// <value>
		/// The game world.
		/// </value>
		public World world { get { return state.world; } }
		
		public virtual IEnumerable<IGameStateComponent> SubComponents (GameTime gameTime)
		{
			yield break;
		}

		public int Index { get { return DrawOrder; } }
	}

	public enum DisplayLayer
	{
		None,
		Background,
		World,
		Dialog,
		Menu,
		MenuItem,
		SubMenu,
		SubMenuItem,
		Overlay,
		Cursor
	}
}

