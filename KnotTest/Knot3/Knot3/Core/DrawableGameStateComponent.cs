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

