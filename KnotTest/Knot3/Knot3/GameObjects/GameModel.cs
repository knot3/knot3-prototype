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
	public class GameModelInfo : GameObjectInfo
	{
		public string Modelname;
		public Angles3 Rotation;
		public float Scale;

		public GameModelInfo (string modelname)
		{
			Modelname = modelname;
			Rotation = Angles3.Zero;
			Scale = 1f;
		}

		public override bool Equals (GameObjectInfo other)
		{
			if (other == null) 
				return false;

			if (other is GameModelInfo) {
				if (this.Modelname == (other as GameModelInfo).Modelname && base.Equals (other))
					return true;
				else
					return false;
			} else {
				return base.Equals (other);
			}
		}
	}

	public class GameModel : GameObject
	{
		#region Attributes and Properties

		public new GameModelInfo Info { get; private set; }

		public virtual Model Model { get { return Models.LoadModel (state, Info.Modelname); } }

		public Color BaseColor;
		public Color HighlightColor;
		public float HighlightIntensity;

		#endregion

		#region Constructors

		public GameModel (GameState state, GameModelInfo info)
			: base(state, info)
		{
			Info = info;

			// colors
			BaseColor = Color.Transparent;
			HighlightColor = Color.Transparent;
			HighlightIntensity = 0f;
		}
		
		#endregion

		#region Draw

		public override void Draw (GameTime gameTime)
		{
			if (Info.IsVisible) {
				state.RenderEffects.Current.DrawModel (this, gameTime);
			}
		}

		#endregion Draw

		#region Intersection

		public override GameObjectDistance Intersects (Ray ray)
		{
			foreach (BoundingSphere _sphere in Model.Bounds()) {
				BoundingSphere sphere = _sphere.Scale (Info.Scale).Translate (Info.Position);
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
			return center / Info.Scale + Info.Position;
		}
		
		#endregion

		#region Matrix Cache

		private float _scale;
		private Angles3 _rotation;
		private Vector3 _position;
		private Matrix _worldMatrix;

		public Matrix WorldMatrix {
			get {
				UpdateWorldMatrix ();
				return _worldMatrix;
			}
		}

		private void UpdateWorldMatrix ()
		{
			if (Info.Scale != _scale || Info.Rotation != _rotation || Info.Position != _position) {
				_worldMatrix = Matrix.CreateScale (Info.Scale)
					* Matrix.CreateFromYawPitchRoll (Info.Rotation.Y, Info.Rotation.X, Info.Rotation.Z)
					* Matrix.CreateTranslation (Info.Position);
				_scale = Info.Scale;
				_rotation = Info.Rotation;
				_position = Info.Position;
			}
		}

		#endregion
	}
}

