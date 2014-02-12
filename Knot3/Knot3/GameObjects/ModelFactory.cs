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
	public sealed class ModelFactory
	{
		// cache
		private Dictionary<GameModelInfo, GameModel> cache = new Dictionary<GameModelInfo, GameModel> ();

		private Func<GameScreen, GameModelInfo, GameModel> CreateModel;

		public ModelFactory (Func<GameScreen, GameModelInfo, GameModel> createModel)
		{
			CreateModel = createModel;
		}

		public GameModel this [GameScreen screen, GameModelInfo info]
		{
			get {
				if (cache.ContainsKey (info)) {
					return cache [info];
				}
				else {
					return cache [info] = CreateModel(screen, info);
				}
			}
		}
	}
}
