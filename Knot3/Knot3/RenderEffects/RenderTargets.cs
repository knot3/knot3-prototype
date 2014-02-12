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
	public static class RenderTargets
	{
		#region RenderTarget Stack

		private static Stack<RenderTarget2D> RenderTargetStack = new Stack<RenderTarget2D> ();

		public static void PushRenderTarget (this GraphicsDevice device, RenderTarget2D current)
		{
			RenderTargetStack.Push (current);
			device.SetRenderTarget (current);
		}

		public static RenderTarget2D PopRenderTarget (this GraphicsDevice device)
		{
			RenderTarget2D removed = RenderTargetStack.Pop ();
			if (RenderTargetStack.Count () > 0) {
				device.SetRenderTarget (RenderTargetStack.Peek ());
			}
			else {
				device.SetRenderTarget (null);
			}
			return removed;
		}

		#endregion
	}

	public sealed class RenderTargetCache
	{
		#region RenderTarget Cache

		private GraphicsDevice device;
		private Dictionary<Point, RenderTarget2D> renderTargets;

		public RenderTargetCache (GraphicsDevice device)
		{
			this.device = device;
			renderTargets = new Dictionary<Point, RenderTarget2D> ();
		}

		public RenderTarget2D CurrentRenderTarget
		{
			get {
				PresentationParameters pp = device.PresentationParameters;
				Point resolution = new Point (pp.BackBufferWidth, pp.BackBufferHeight);
				if (!renderTargets.ContainsKey (resolution)) {
					renderTargets [resolution] = new RenderTarget2D (device, resolution.X, resolution.Y,
					        false, SurfaceFormat.Color, DepthFormat.Depth24, 1, RenderTargetUsage.PreserveContents);
				}
				return renderTargets [resolution];
			}
		}

		#endregion
	}
}
