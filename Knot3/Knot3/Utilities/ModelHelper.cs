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
using Knot3.Settings;
using Knot3.RenderEffects;

namespace Knot3.Utilities
{
	public static class ModelHelper
	{
		public static string[] ValidQualities = new string[] {
			"low",
			"medium",
			"high"
		};

		public static string Quality
		{
			get { return Options.Default ["video", "model-quality", "medium"]; }
		}

		private static Dictionary<string, ContentManager> contentManagers = new Dictionary<string, ContentManager> ();
		private static HashSet<string> invalidModels = new HashSet<string> ();

		public static Model LoadModel (GameScreen screen, string name)
		{
			ContentManager content;
			if (contentManagers.ContainsKey (screen.CurrentRenderEffects.Current.ToString ())) {
				content = contentManagers [screen.CurrentRenderEffects.Current.ToString ()];
			}
			else {
				contentManagers [screen.CurrentRenderEffects.Current.ToString ()] = content = new ContentManager (screen.content.ServiceProvider, screen.content.RootDirectory);
			}

			Model model = LoadModel (content, screen.CurrentRenderEffects.Current, name + "-" + Quality);
			if (model == null) {
				model = LoadModel (content, screen.CurrentRenderEffects.Current, name);
			}
			return model;
		}

		private static Model LoadModel (ContentManager content, IRenderEffect pp, string name)
		{
			if (invalidModels.Contains (name)) {
				return null;
			}
			else {
				try {
					Model model = content.Load<Model> (name);
					pp.RemapModel (model);
					return model;
				}
				catch (ContentLoadException) {
					Console.WriteLine ("Warning: Model " + name + " does not exist!");
					invalidModels.Add (name);
					return null;
				}
			}
		}
	}
}
