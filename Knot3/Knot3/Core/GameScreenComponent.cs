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
	/// Eine Implementierung von IGameScreenComponent. Erbt von GameComponent aus XNA
	/// und hat nur eine Update()-Methode.
	/// </summary>
	public class GameScreenComponent : Xna.GameComponent, IGameScreenComponent
	{
		public GameScreenComponent (GameScreen screen, DisplayLayer index)
		: base(screen.game)
		{
			this.screen = screen;
			this.Index = index;
		}

		/// <summary>
		/// Gets the GameScreen associated with this object.
		/// </summary>
		/// <value>
		/// The Game screen.
		/// </value>
		public GameScreen screen { get; private set; }

		public virtual IEnumerable<IGameScreenComponent> SubComponents (GameTime time)
		{
			yield break;
		}

		public DisplayLayer Index { get; set; }
	}
}
