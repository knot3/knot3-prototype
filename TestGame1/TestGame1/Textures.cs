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

namespace TestGame1
{
	public static class Textures
	{
		public static Texture2D CreateColorTexture (GraphicsDevice graphicsDevice)
		{
			return Create (graphicsDevice, 1, 1, new Color ());
		}
 
		public static Texture2D Create (GraphicsDevice graphicsDevice, Color color)
		{
			return Create (graphicsDevice, 1, 1, color);
		}
 
		public static Texture2D Create (GraphicsDevice graphicsDevice, int width, int height, Color color)
		{
			// create a texture with the specified size
			Texture2D texture = new Texture2D (graphicsDevice, width, height);

			// fill it with the specified colors
			Color[] colors = new Color[width * height];
			for (int i = 0; i < colors.Length; i++) {
				colors [i] = new Color (color.ToVector3 ());
			}
			texture.SetData (colors);
			return texture;
		}
	}
}

