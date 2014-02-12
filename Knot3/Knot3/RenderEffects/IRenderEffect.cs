using System;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Knot3.GameObjects;

namespace Knot3.RenderEffects
{
	/// <summary>
	/// Enthält alle Methoden, die durch einen auf ein RenderTarget renderenden RenderEffect implementiert werden
	/// müssen, sodass er universell einsetzbar ist und von jedem GameComponent und GameModel genutzt werden kann.
	/// </summary>
	public interface IRenderEffect
	{
		RenderTarget2D RenderTarget { get; }

		void Begin (GameTime time);

		void End (GameTime time);

		void RemapModel (Model model);

		void DrawModel (GameModel model, GameTime time);

		void DrawLastFrame (GameTime time);
	}
}
