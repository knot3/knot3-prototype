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
	public class PipeMovement : IGameObject, IEnumerable<IGameObject>
	{
		// game state
		private GameState state;

		// game world
		public World World { get; set; }

		// info
		public GameObjectInfo Info { get; private set; }

		// the knot
		public Knot Knot { get; set; }

		// ...
		private Vector3 previousMousePosition = Vector3.Zero;
		private List<ShadowGameObject> shadowObjects;

		public PipeMovement (GameState state, World world, GameObjectInfo info)
		{
			this.state = state;
			this.World = world;
			Info = info;
			shadowObjects = new List<ShadowGameObject> ();
		}

		private HashSet<EdgeList> knownEdgeLists = new HashSet<EdgeList> ();

		public void Update (GameTime gameTime)
		{
			// check whether the hovered object is a pipe
			if (World.SelectedObject is PipeModel) {
				PipeModel pipe = World.SelectedObject as PipeModel;

				// pipe selection
				if (Core.Input.LeftButton == ClickState.SingleClick) {
					World.Redraw = true;
					try {
						Edge e = pipe.Info.Edge;
						EdgeList edges = pipe.Info.EdgeList;
						knownEdgeLists.Add (edges);
						Console.WriteLine ("knownEdgeLists=" + knownEdgeLists.Count);

						// CTRL
						if (Keys.LeftControl.IsHeldDown ()) {
							edges.SelectedEdges.Add (e);
						}
						// Shift
						else if (Keys.LeftShift.IsHeldDown ()) {
							if (edges.SelectedEdges.Count != 0) {
								Edge last = edges.SelectedEdges [-1];
								edges.SelectedEdges.AddRange (edges.Interval (last, e));
							}
							edges.SelectedEdges.Add (e);
						}
						// mouse click only
						else {
							edges.SelectedEdges.Set (e);
						}
					} catch (ArgumentOutOfRangeException exp) {
						Console.WriteLine (exp.ToString ());
					}
				}

				// find out the current mouse position in 3D
				Vector3 screenLocation = state.viewport.Project (
					pipe.Center (), World.Camera.ProjectionMatrix, World.Camera.ViewMatrix, World.Camera.WorldMatrix
				);
				Vector3 currentMousePosition = state.viewport.Unproject (
					new Vector3 (Core.Input.MouseState.ToVector2 (), screenLocation.Z),
					World.Camera.ProjectionMatrix, World.Camera.ViewMatrix, Matrix.Identity
				);

				// show a shadow
				if (state.input.CurrentInputAction == InputAction.SelectedObjectShadowMove) {
					if (previousMousePosition == Vector3.Zero) {
						previousMousePosition = currentMousePosition;
						CreateShadowPipes ();
					}
					MoveShadowPipes (currentMousePosition);
					World.Redraw = true;
				}
				// perform the move
				else if (state.input.CurrentInputAction == InputAction.SelectedObjectMove) {
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
				if (Core.Input.LeftButton == ClickState.SingleClick) {
					foreach (EdgeList edges in knownEdgeLists) {
						edges.SelectedEdges.Clear ();
					}
				}
			}
		}

		private void CreateShadowPipes ()
		{
			shadowObjects.Clear ();
			foreach (IEnumerable<IGameObject> container in World.Objects.OfType<IEnumerable<IGameObject>>()) {
				foreach (PipeModel pipe in container.OfType<PipeModel>()) {
					if (Knot.Edges.SelectedEdges.Contains (pipe.Info.Edge)) {
						shadowObjects.Add (new ShadowGameModel (state, pipe as GameModel));
					}
				}
			}
		}

		private void ComputeDirection (Vector3 currentMousePosition, out Vector3 direction, out float count)
		{
			Vector3 mouseMove = currentMousePosition - previousMousePosition;
			direction = mouseMove.PrimaryDirection ();
			count = mouseMove.Length () / Node.Scale;
		}

		private void ComputeDirection (Vector3 currentMousePosition, out Vector3 direction, out int countInt)
		{
			float countFloat;
			ComputeDirection (currentMousePosition, out direction, out countFloat);
			countInt = (int)Math.Round (countFloat);
		}

		private void MoveShadowPipes (Vector3 currentMousePosition)
		{
			Vector3 direction3D;
			float count;
			ComputeDirection (currentMousePosition, out direction3D, out count);
			foreach (ShadowGameModel shadowObj in shadowObjects) {
				shadowObj.ShadowPosition = shadowObj.OriginalPosition + direction3D * count * Node.Scale;
				shadowObj.ShadowAlpha = 0.3f;
				shadowObj.ShadowColor = Color.White;
				// Console.WriteLine ("MoveShadowPipes: " + shadowObj + ", direction=" + direction3D * count);
			}
		}

		private void MovePipes (Vector3 currentMousePosition)
		{
			Vector3 direction3D;
			int count;
			ComputeDirection (currentMousePosition, out direction3D, out count);
			if (count > 0) {
				try {
					//Knot.Edges.SelectEdge (Info.Edge, true);
					Knot.Edges.Move (Knot.Edges.SelectedEdges, direction3D, count);
					//Knot.Edges.SelectEdge ();
					previousMousePosition = currentMousePosition;
				} catch (ArgumentOutOfRangeException exp) {
					Console.WriteLine (exp.ToString ());
				}
			}
		}

		public void Draw (GameTime gameTime)
		{
			foreach (IGameObject shadowObj in shadowObjects) {
				shadowObj.Draw (gameTime);
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
	
	public class ShadowGameObject : IGameObject
	{
		// game state
		protected GameState state;

		// the decorated object
		private IGameObject Obj;

		// game world
		public World World {
			get { return Obj.World; }
			set {}
		}

		// info
		public GameObjectInfo Info { get; private set; }

		public ShadowGameObject (GameState state, IGameObject obj)
		{
 this.state = state;
			Info = new GameObjectInfo ();
			Obj = obj;
			Info.IsVisible = true;
			Info.IsSelectable = false;
			Info.IsMovable = false;
		}

		public Vector3 ShadowPosition {
			get {
				return Info.Position;
			}
			set {
				Info.Position = value;
			}
		}

		public Vector3 OriginalPosition {
			get {
				return Obj.Info.Position;
			}
		}

		#region Update

		public virtual void Update (GameTime gameTime)
		{
			Info.IsVisible = Math.Abs ((ShadowPosition - Obj.Info.Position).Length ()) > 50;
		}

		#endregion

		#region Draw

		public virtual void Draw (GameTime gameTime)
		{
			Vector3 originalPositon = Obj.Info.Position;
			Obj.Info.Position = ShadowPosition;
			Obj.Draw (gameTime);
			Obj.Info.Position = originalPositon;
		}

		#endregion

		#region Intersection

		public GameObjectDistance Intersects (Ray ray)
		{
			return null;
			//return Obj.Intersects (ray);
		}

		public Vector3 Center ()
		{
			return Obj.Center ();
		}

		#endregion
	}
	
	public class ShadowGameModel : ShadowGameObject
	{
		private GameModel Model;

		public Color ShadowColor { get; set; }

		public float ShadowAlpha { get; set; }

		public ShadowGameModel (GameState state, GameModel model)
			: base(state, model)
		{
			Model = model;
		}

		public override void Draw (GameTime gameTime)
		{
			// swap position, colors, alpha
			Vector3 originalPositon = Model.Info.Position;
			Model.Info.Position = ShadowPosition;
			float originalHighlightIntensity = Model.HighlightIntensity;
			Model.HighlightIntensity = 0f;
			float originalAlpha = Model.Alpha;
			Model.Alpha = ShadowAlpha;

			// draw
			state.RenderEffects.Current.DrawModel (Model, gameTime);

			// swap everything back
			Model.Info.Position = originalPositon;
			Model.HighlightIntensity = originalHighlightIntensity;
			Model.Alpha = originalAlpha;
		}
	}
}

