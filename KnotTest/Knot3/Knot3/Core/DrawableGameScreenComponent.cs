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
	/// Eine Implementierung von IGameScreenComponent. Erbt von DrawableGameComponent aus XNA
	/// und hat daher außer der Update()-Methode auch eine Draw()-Methode.
	/// </summary>
	public abstract class DrawableGameScreenComponent : Xna.DrawableGameComponent, IGameScreenComponent
	{
		public DrawableGameScreenComponent (GameScreen screen, DisplayLayer index)
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

		public virtual IEnumerable<IGameScreenComponent> SubComponents (GameTime gameTime)
		{
			yield break;
		}

		private DisplayLayer _index;

		public DisplayLayer Index {
			get { return _index; }
			set { _index = value; DrawOrder = (int)value; }
		}
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

