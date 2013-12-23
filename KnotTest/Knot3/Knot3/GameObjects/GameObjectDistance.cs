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

namespace Knot3.GameObjects
{
	/// <summary>
	/// Ein IGameObject ist ein Objekt, zum Beispiel ein 3D-Modell (Klasse GameModel),
	/// das in der 3D-Welt (Klasse World) gezeichnet wird.
	/// </summary>

	public sealed class GameObjectDistance
	{
		public IGameObject Object;
		public float Distance;
	}
}
