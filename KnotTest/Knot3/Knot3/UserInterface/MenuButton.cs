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

namespace Knot3.UserInterface
{
	public class MenuButton : MenuItem
	{
		public MenuButton (GameState state, DisplayLayer drawOrder, int itemNum, MenuItemInfo info,
		                 LazyItemColor fgColor, LazyItemColor bgColor, HAlign alignX)
			: base(state, drawOrder, itemNum, info, fgColor, bgColor, alignX)
		{
		}
	}
}

