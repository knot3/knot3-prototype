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

namespace Knot3.Utilities
{
	public static class ShaderHelper
	{
		public static Effect LoadEffect (this GameScreen screen, string name)
		{
			if (MonoHelper.IsRunningOnMono ()) {
				return LoadEffectMono (screen, name);
			}
			else {
				return LoadEffectDotnet (screen, name);
			}
		}

		private static Effect LoadEffectMono (GameScreen screen, string name)
		{
			return new Effect (screen.device, System.IO.File.ReadAllBytes ("Content/" + name + ".mgfx"));
		}

		private static Effect LoadEffectDotnet (GameScreen screen, string name)
		{
			return screen.content.Load<Effect> (name);
		}
	}
}
