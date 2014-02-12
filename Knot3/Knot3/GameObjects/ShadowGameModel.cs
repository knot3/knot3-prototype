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

using Knot3.Utilities;
using Knot3.KnotData;
using Knot3.RenderEffects;
using Knot3.Core;

using System.Collections;

namespace Knot3.GameObjects
{
	public class ShadowGameModel : ShadowGameObject
	{
		private GameModel Model;

		public Color ShadowColor { get; set; }

		public float ShadowAlpha { get; set; }

		public ShadowGameModel (GameScreen screen, GameModel model)
		: base(screen, model)
		{
			Model = model;
		}

		public override void Draw (GameTime time)
		{
			// swap position, colors, alpha
			Vector3 originalPositon = Model.Info.Position;
			Model.Info.Position = ShadowPosition;
			float originalHighlightIntensity = Model.HighlightIntensity;
			Model.HighlightIntensity = 0f;
			float originalAlpha = Model.Alpha;
			Model.Alpha = ShadowAlpha;

			// draw
			screen.CurrentRenderEffects.Current.DrawModel (Model, time);

			// swap everything back
			Model.Info.Position = originalPositon;
			Model.HighlightIntensity = originalHighlightIntensity;
			Model.Alpha = originalAlpha;
		}
	}
}
