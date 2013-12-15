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

namespace Knot3.RenderEffects
{
	/// <summary>
	/// Ein RenderEffect, der keinen besonderen Effekt anwendet.
	/// </summary>
	public sealed class NoEffect : RenderEffect
	{
		public NoEffect (GameScreen screen)
			: base(screen)
		{
		}

		protected override void DrawRenderTarget (SpriteBatch spriteBatch, GameTime gameTime)
		{
			spriteBatch.Draw (RenderTarget, Vector2.Zero, Color.White);
		}
	}
}

