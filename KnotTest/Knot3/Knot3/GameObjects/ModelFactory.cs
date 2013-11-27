using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using Knot3.Core;
using Knot3.Utilities;
using Knot3.KnotData;
using Knot3.RenderEffects;

namespace Knot3.GameObjects
{
	public abstract class ModelFactory
	{
		// cache
		private Dictionary<GameModelInfo, GameModel> cache = new Dictionary<GameModelInfo, GameModel> ();

		public GameModel this [GameState state, GameModelInfo info] {
			get {
				if (cache.ContainsKey (info)) {
					return cache [info];
				} else {
					return cache [info] = CreateModel(state, info);
				}
			}
		}

		protected abstract GameModel CreateModel(GameState state, GameModelInfo info);
	}
}

