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
	public class GameModel : GameObject
	{
		#region Attributes and Properties

		protected virtual Model Model { get; set; }

		protected virtual float Scale { get; set; }

		protected virtual Angles3 Rotation { get; set; }

		protected override Vector3 Position { get; set; }

		protected ModelMesh[] ModelMeshes;

		protected virtual Matrix WorldMatrix {
			get {
				return Matrix.CreateScale (Scale)
					* Matrix.CreateFromYawPitchRoll (Rotation.Y, Rotation.X, Rotation.Z)
					* Matrix.CreateTranslation (Position);
			}
		}

		#endregion

		#region Constructors

		public GameModel (GameState state, string modelname, Vector3 position, float scale)
			: base(state)
		{
			// load test model
			Model = LoadModel (content, modelname);
			Scale = scale;
			Rotation = Angles3.Zero;
			Position = position;
			ModelMeshes = Model.Meshes.ToArray ();
		}

		public GameModel (GameState state, Model model, Vector3 position, float scale)
			: base(state)
		{
			// load test model
			Model = model;
			Scale = scale;
			Rotation = Angles3.Zero;
			Position = position;
			ModelMeshes = Model.Meshes.ToArray ();
		}
		
		#endregion

		public virtual void UpdateEffect (BasicEffect effect, GameTime gameTime)
		{
		}


		#region Draw

		public override void DrawObject (GameTime gameTime)
		{
			// test:
			foreach (ModelMesh mesh in ModelMeshes) {
				foreach (BasicEffect effect in mesh.Effects) {
					if (Keys.L.IsHeldDown ()) {
						effect.LightingEnabled = false;
					} else {
						effect.EnableDefaultLighting ();  // Beleuchtung aktivieren
					}
					UpdateEffect (effect, gameTime);
					effect.World = Matrix.CreateScale (Scale)
						* Matrix.CreateFromYawPitchRoll (Rotation.Y, Rotation.X, Rotation.Z)
						* Matrix.CreateTranslation (Position);
					effect.View = camera.ViewMatrix;
					effect.Projection = camera.ProjectionMatrix;
				}

				mesh.Draw ();
			}
		}

		#endregion Draw

		#region Intersection

		public override GameObjectDistance Intersects (Ray ray)
		{
			foreach (BoundingSphere _sphere in Model.Bounds()) {
				BoundingSphere sphere = _sphere.Scale (Scale).Translate (Position);
				float? distance = ray.Intersects (sphere);
				if (distance != null) {
					GameObjectDistance intersection = new GameObjectDistance () {
						Object=this, Distance=distance.Value
					};
					return intersection;
				}
			}
			return null;
		}

		public override Vector3 Center ()
		{
			Vector3 center = Vector3.Zero;
			int count = Model.Meshes.Count;
			foreach (ModelMesh mesh in Model.Meshes) {
				center += mesh.BoundingSphere.Center / count;
			}
			return center / Scale + Position;
		}
		
		#endregion
	}

	public class CachedGameModel : GameModel
	{
		#region Constructors

		public CachedGameModel (GameState state, string modelname, Vector3 position, float scale)
			: base(state, modelname, position, scale)
		{
		}

		public CachedGameModel (GameState state, Model model, Vector3 position, float scale)
			: base(state, model, position, scale)
		{
		}

		#endregion

		#region Attributes

		private float _scale;
		private Angles3 _rotation;
		private Vector3 _position;
		private Matrix _worldMatrix;

		#endregion

		#region Properties

		protected override Matrix WorldMatrix {
			get {
				UpdateWorldMatrix ();
				return _worldMatrix;
			}
		}

		private void UpdateWorldMatrix ()
		{
			if (Scale != _scale || Rotation != _rotation || Position != _position) {
				_worldMatrix = Matrix.CreateScale (Scale)
					* Matrix.CreateFromYawPitchRoll (Rotation.Y, Rotation.X, Rotation.Z)
					* Matrix.CreateTranslation (Position);
				_scale = Scale;
				_rotation = Rotation;
				_position = Position;
			}
		}

		#endregion
	}
}

