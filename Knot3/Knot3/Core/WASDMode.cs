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

using Knot3.UserInterface;
using Knot3.Utilities;

namespace Knot3.Core
{
	/// <summary>
	/// Die aktuelle Belegung für die Tasten W,A,S,D und angrenzende Tasten.
	/// </summary>
	public enum WASDMode {
		/// <summary>
		/// W,A,S,D bewegen die Kamera "wie auf einer Kugel-Oberfläche", das heißt in einem festen Radius, um ein Objekt.
		/// </summary>
		ArcballMode,
		/// <summary>
		/// W,A,S,D bewegen die Kamera wie in einem First Person Shooter.
		/// </summary>
		FirstPersonMode
	}
}
