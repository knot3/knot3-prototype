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

namespace Knot3.Utilities
{
	public static class Shaders
	{
		public static Effect LoadEffect (this GameState state, string name)
		{
			if (Mono.IsRunningOnMono ())
				return LoadEffectMono (state, name);
			else
				return LoadEffectDotnet (state, name);
		}

		private static Effect LoadEffectMono (GameState state, string name)
		{
			return new Effect (state.device, System.IO.File.ReadAllBytes ("Content/" + name + ".mgfx"));
		}

		private static Effect LoadEffectDotnet (GameState state, string name)
		{
			return state.content.Load<Effect> (name);
		}
	}
}
