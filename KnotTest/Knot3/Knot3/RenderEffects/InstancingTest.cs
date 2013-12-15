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
using Knot3.GameObjects;
using System.Collections;

namespace Knot3.RenderEffects
{
	public class InstancingTest : RenderEffect
	{
		public InstancingTest (GameScreen state)
			: base(state)
		{
		}

		public override void Begin (Color background, GameTime gameTime)
		{
			base.Begin (background, gameTime);

			Overlay.Profiler ["DrawModel1"] = 0;
		}

		public override void End (GameTime gameTime)
		{
			base.End (gameTime);
		}

		protected override void DrawRenderTarget (SpriteBatch spriteBatch, GameTime gameTime)
		{
			foreach (string key in instanceHash.Keys) {
				ModelInstances instances = instanceHash [key] as ModelInstances;
				GameModel model = instances.Model;

				if (instances.Count == 0)
					continue;

				// set the stencil state
				state.device.DepthStencilState = DepthStencilState.Default;
				// Setting the other states isn't really necessary but good form
				state.device.BlendState = BlendState.Opaque;
				state.device.RasterizerState = RasterizerState.CullCounterClockwise;
				state.device.SamplerStates [0] = SamplerState.LinearWrap;

				foreach (ModelMesh mesh in model.Model.Meshes) {
					foreach (ModelMeshPart part in mesh.MeshParts) {
						// set the vertex and index buffers only once, for all objects
						state.device.SetVertexBuffer (part.VertexBuffer);
						state.device.Indices = part.IndexBuffer;

						BasicEffect effect = part.Effect as BasicEffect;
						ModifyBasicEffect (effect, model);

						effect.View = model.World.Camera.ViewMatrix;
						effect.Projection = model.World.Camera.ProjectionMatrix;

						for (int w = 0; w < instances.Count; ++w) {
							effect.World = instances.WorldMatrices [w] * model.World.Camera.WorldMatrix;
						
							// Draw all the instance copies in a single call.
							foreach (EffectPass pass in part.Effect.CurrentTechnique.Passes) {
								pass.Apply ();

								state.device.DrawIndexedPrimitives (
									PrimitiveType.TriangleList, part.VertexOffset, 0, part.NumVertices,
									part.StartIndex, part.PrimitiveCount //, instances.Length
								);
							}
						}
					}
				}

				instances.Count = 0;
			}

			spriteBatch.Draw (RenderTarget, Vector2.Zero, Color.White);
		}

		private Hashtable instanceHash = new Hashtable ();

		private class ModelInstances
		{
			public GameModel Model;
			public Matrix[] WorldMatrices;
			public int Count;
		};

		public override void DrawModel (GameModel model, GameTime gameTime)
		{
			//Overlay.Profiler ["DrawModel1"] += Knot3.Core.Game.Time (() => {

			string key = string.Join (";", (string)model.Info.Modelname, model.BaseColor.R, model.BaseColor.B,
			                         model.BaseColor.G, model.Alpha, model.HighlightColor.R, model.HighlightColor.B,
			                         model.HighlightColor.G, model.HighlightIntensity);
			if (!instanceHash.ContainsKey (key)) {
				instanceHash [key] = new ModelInstances {
						Model = model,
						WorldMatrices = new Matrix[200],
						Count = 0
					};
				Console.WriteLine ("new ModelInstances(" + key + ")");
			}
			ModelInstances instances = instanceHash [key] as ModelInstances;
			if (instances.Count + 1 >= instances.WorldMatrices.Length) {
				Array.Resize (ref instances.WorldMatrices, instances.WorldMatrices.Length * 2);
				Console.WriteLine ("Resize: " + instances.WorldMatrices.Length);
			}
			instances.Model = model;
			instances.WorldMatrices [instances.Count++] = model.WorldMatrix;

			//}).TotalMilliseconds;
		}
	}
}

