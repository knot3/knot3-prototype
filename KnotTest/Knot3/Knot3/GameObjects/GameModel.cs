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

namespace Knot3.GameObjects
{
	public class GameModel : GameObject
	{
		#region Attributes and Properties

		protected virtual string Modelname { get; private set; }

		public virtual Model Model { get { return Models.LoadModel (state, Modelname); } }

		protected virtual float Scale { get; set; }

		protected virtual Angles3 Rotation { get; set; }

		protected override Vector3 Position { get; set; }

		public Color BaseColor;
		public Color HighlightColor;
		public float HighlightIntensity;

		#endregion

		#region Constructors

		public GameModel (GameState state, string modelname, Vector3 position, float scale)
			: base(state)
		{
			// load test model
			Modelname = modelname;
			Scale = scale;
			Rotation = Angles3.Zero;
			Position = position;

			// colors
			BaseColor = Color.Transparent;
			HighlightColor = Color.Transparent;
			HighlightIntensity = 0f;
		}
		
		#endregion

		#region Draw

		public virtual Matrix WorldMatrix {
			get {
				return Matrix.CreateScale (Scale)
					* Matrix.CreateFromYawPitchRoll (Rotation.Y, Rotation.X, Rotation.Z)
					* Matrix.CreateTranslation (Position);
			}
		}

		public override void DrawObject (GameTime gameTime)
		{
			state.RenderEffects.Current.DrawModel (this, gameTime);
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

		#endregion

		#region Attributes

		private float _scale;
		private Angles3 _rotation;
		private Vector3 _position;
		private Matrix _worldMatrix;

		#endregion

		#region Properties

		public override Matrix WorldMatrix {
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

