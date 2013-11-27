using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Knot3.GameObjects;

namespace Knot3.Core
{
	/// <summary>
	/// Eine von IGameStateClass abgeleitete Klasse hält eine Referenz auf einen bestimmten GameState und stellt
	/// Properties und Methoden für den direkten Zugriff auf häufig benötigte XNA-Klassen bereit.
	/// </summary>
	public interface IGameStateClass
	{
		/// <summary>
		/// Gets the GameState associated with this object.
		/// </summary>
		/// <value>
		/// The Game state.
		/// </value>
		GameState state { get; }

		/// <summary>
		/// Gets the graphics device manager.
		/// </summary>
		/// <value>
		/// The graphics device manager.
		/// </value>
		GraphicsDeviceManager graphics { get; }

		/// <summary>
		/// Gets the graphics device.
		/// </summary>
		/// <value>
		/// The graphics device.
		/// </value>
		GraphicsDevice device { get; }

		/// <summary>
		/// Gets the viewport.
		/// </summary>
		/// <value>
		/// The viewport.
		/// </value>
		Viewport viewport { get; }

		/// <summary>
		/// Gets the content manager.
		/// </summary>
		/// <value>
		/// The content manager.
		/// </value>
		ContentManager content { get; }

		/// <summary>
		/// Gets the camera. Returns null of the game state is not in 3D mode!
		/// </summary>
		/// <value>
		/// The camera.
		/// </value>
		Camera camera { get; }

		/// <summary>
		/// Gets the input handler.
		/// </summary>
		/// <value>
		/// The input handler.
		/// </value>
		Input input { get; }

		/// <summary>
		/// Gets the game world.
		/// </summary>
		/// <value>
		/// The game world.
		/// </value>
		World world { get; }
	}
}

