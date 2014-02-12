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
	public class EdgeMovement : IGameObject, IEnumerable<IGameObject>
	{
		// game screen
		private GameScreen screen;

		// game world
		public World World { get; set; }

		// info
		public GameObjectInfo Info { get; private set; }

		// the knot
		public Knot Knot { get; set; }

		// ...
		private Vector3 previousMousePosition = Vector3.Zero;
		private List<ShadowGameObject> shadowObjects;

		public EdgeMovement (GameScreen screen, World world, GameObjectInfo info)
		{
			this.screen = screen;
			this.World = world;
			Info = info;
			shadowObjects = new List<ShadowGameObject> ();
		}

		public void Update (GameTime time)
		{
			TrySelectObject (time);
			TryMovePipes (time);
		}

		private void TrySelectObject (GameTime time)
		{
			// check whether the hovered object is a pipe
			if (World.SelectedObject is PipeModel) {
				PipeModel pipe = World.SelectedObject as PipeModel;

				// pipe selection
				if (InputManager.LeftMouseButton == ClickState.SingleClick) {
					World.Redraw = true;
					try {
						Edge selectedEdge = pipe.Info.Edge;
						Console.WriteLine ("knot=" + Knot.Count());

						// CTRL
						if (Keys.LeftControl.IsHeldDown ()) {
							Knot.AddToSelection(selectedEdge);
						}
						// Shift
						else if (Keys.LeftShift.IsHeldDown ()) {
							Knot.AddRangeToSelection(selectedEdge);
						}
						// mouse click only
						else {
							Knot.ClearSelection();
							Knot.AddToSelection(selectedEdge);
						}
					}
					catch (ArgumentOutOfRangeException exp) {
						Console.WriteLine (exp.ToString ());
					}
				}
			}
		}

		private void TryMovePipes (GameTime time)
		{
			// check whether the hovered object is a pipe
			if (World.SelectedObject is PipeModel || World.SelectedObject is ArrowModel) {
				GameModel selectedModel = World.SelectedObject as GameModel;

				// find out the current mouse position in 3D
				Vector3 screenLocation = screen.viewport.Project (
				                             selectedModel.Center (), World.Camera.ProjectionMatrix,
				                             World.Camera.ViewMatrix, World.Camera.WorldMatrix
				                         );
				Vector3 currentMousePosition = screen.viewport.Unproject (
				                                   new Vector3 (InputManager.CurrentMouseState.ToVector2 (), screenLocation.Z),
				                                   World.Camera.ProjectionMatrix, World.Camera.ViewMatrix, Matrix.Identity
				                               );

				// show a shadow
				if (screen.input.CurrentInputAction == InputAction.SelectedObjectShadowMove) {
					if (previousMousePosition == Vector3.Zero) {
						previousMousePosition = currentMousePosition;
						CreateShadowPipes ();
					}
					if (selectedModel is ArrowModel) {
						MoveShadowPipes (currentMousePosition, (selectedModel as ArrowModel).Info.Direction);
					}
					else {
						MoveShadowPipes (currentMousePosition);
					}
					World.Redraw = true;
				}
				// perform the move
				else if (screen.input.CurrentInputAction == InputAction.SelectedObjectMove) {
					MovePipes (currentMousePosition);
					shadowObjects.Clear ();
					World.Redraw = true;
				}
				// do nothing
				else {
					previousMousePosition = Vector3.Zero;
					if (shadowObjects.Count > 0) {
						shadowObjects.Clear ();
						World.Redraw = true;
					}
				}
			}
			// selected object is not a PipeModel
			else {
				// left click clears the selection
				if (InputManager.LeftMouseButton == ClickState.SingleClick) {
					Knot.ClearSelection();
				}
			}
		}

		private void CreateShadowPipes ()
		{
			shadowObjects.Clear ();
			foreach (IEnumerable<IGameObject> container in World.Objects.OfType<IEnumerable<IGameObject>>()) {
				foreach (PipeModel pipe in container.OfType<PipeModel>()) {
					if (Knot.SelectedEdges.Contains (pipe.Info.Edge)) {
						shadowObjects.Add (new ShadowGameModel (screen, pipe as GameModel));
					}
				}
				foreach (ArrowModel arrow in container.OfType<ArrowModel>()) {
					shadowObjects.Add (new ShadowGameModel (screen, arrow as GameModel));
				}
			}
		}

		private void ComputeDirection (Vector3 currentMousePosition, out Direction direction, out float count)
		{
			Vector3 mouseMove = currentMousePosition - previousMousePosition;
			direction = mouseMove.PrimaryDirection ().ToDirection ();
			count = mouseMove.Length () / Node.Scale;
		}

		private void ComputeDirection (Vector3 currentMousePosition, out Direction direction, out int countInt)
		{
			float countFloat;
			ComputeDirection (currentMousePosition, out direction, out countFloat);
			countInt = (int)Math.Round (countFloat);
		}

		private void MoveShadowPipes (Vector3 currentMousePosition, Vector3 direction3D)
		{
			Direction dummy;
			float count;
			ComputeDirection (currentMousePosition, out dummy, out count);
			foreach (ShadowGameModel shadowObj in shadowObjects) {
				shadowObj.ShadowPosition = shadowObj.OriginalPosition + direction3D * count * Node.Scale;
				shadowObj.ShadowAlpha = 0.3f;
				shadowObj.ShadowColor = Color.White;
				// Console.WriteLine ("MoveShadowPipes: " + shadowObj + ", direction=" + direction3D * count);
			}
		}

		private void MoveShadowPipes (Vector3 currentMousePosition)
		{
			Direction direction;
			float count;
			ComputeDirection (currentMousePosition, out direction, out count);
			foreach (ShadowGameModel shadowObj in shadowObjects) {
				shadowObj.ShadowPosition = shadowObj.OriginalPosition + direction.ToVector3 () * count * Node.Scale;
				shadowObj.ShadowAlpha = 0.3f;
				shadowObj.ShadowColor = Color.White;
				// Console.WriteLine ("MoveShadowPipes: " + shadowObj + ", direction=" + direction3D * count);
			}
		}

		private void MovePipes (Vector3 currentMousePosition)
		{
			Direction direction;
			int count;
			ComputeDirection (currentMousePosition, out direction, out count);
			if (count > 0) {
				try {
					//Knot.Edges.SelectEdge (Info.Edge, true);
					Knot.Move (direction, count);
					//Knot.Edges.SelectEdge ();
					previousMousePosition = currentMousePosition;
				}
				catch (ArgumentOutOfRangeException exp) {
					Console.WriteLine (exp.ToString ());
				}
			}
		}

		public void Draw (GameTime time)
		{
			foreach (IGameObject shadowObj in shadowObjects) {
				shadowObj.Draw (time);
			}
		}

		public IEnumerator<IGameObject> GetEnumerator ()
		{
			foreach (IGameObject shadowObj in shadowObjects) {
				yield return shadowObj;
			}
		}

		// Explicit interface implementation for nongeneric interface
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator (); // Just return the generic version
		}

		#region Shadow Objects Intersection

		public GameObjectDistance Intersects (Ray ray)
		{
			return null;
		}

		public Vector3 Center ()
		{
			return Info.Position;
		}

		#endregion
	}
}
