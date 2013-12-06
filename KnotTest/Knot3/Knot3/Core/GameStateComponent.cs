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
	/// Eine Implementierung von IGameStateComponent. Erbt von GameComponent aus XNA
	/// und hat nur eine Update()-Methode.
	/// </summary>
	public class GameStateComponent : Xna.GameComponent, IGameStateComponent
	{
		protected DisplayLayer InputOrder;

		public GameStateComponent (GameState state, DisplayLayer inputOrder)
			: base(state.game)
		{
			this.state = state;
			this.InputOrder = inputOrder;
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

		public int Index { get { return (int)InputOrder; } }
	}
}

