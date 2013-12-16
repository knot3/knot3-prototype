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

namespace Knot3.GameObjects
{
	public class GameModelInfo : GameObjectInfo
	{
		public string Modelname;
		public Angles3 Rotation;
		public Vector3 Scale;

		public GameModelInfo (string modelname)
		{
			Modelname = modelname;
			Rotation = Angles3.Zero;
			Scale = Vector3.One;
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

	/// <summary>
	/// Ein GameModel ist ein IGameObject, das ein 3D-Modell zeichnet.
	/// </summary>
	public class GameModel : IGameObject
	{
		#region Attributes and Properties

		/// <summary>
		/// Der zugewiesene GameScreen. Dieses Objekt kann nur innerhalb dieses GameScreen's verwendet werden.
		/// </summary>
		protected GameScreen screen;

		GameObjectInfo IGameObject.Info { get { return Info; } }

		public GameModelInfo Info { get; protected set; }

		private World _world;

		public World World {
			get { return _world; }
			set {
				_world = value;
				_world.Camera.OnViewChanged += OnViewChanged;
				OnViewChanged ();
			}
		}

		/// <summary>
		/// Das XNA-3D-Modell.
		/// </summary>
		/// <value>
		/// The model.
		/// </value>
		public virtual Model Model { get { return Models.LoadModel (screen, Info.Modelname); } }

		public Color BaseColor;
		public Color HighlightColor;
		public float HighlightIntensity;
		public float Alpha;

		#endregion

		#region Constructors

		public GameModel (GameScreen screen, GameModelInfo info)
		{
			this.screen = screen;
			Info = info;

			// colors
			BaseColor = Color.Transparent;
			HighlightColor = Color.Transparent;
			HighlightIntensity = 0f;
			Alpha = 1f;
		}
		
		#endregion

		#region Update
		
		public virtual void Update (GameTime time)
		{
		}

		#endregion

		#region Draw

		public virtual void Draw (GameTime time)
		{
			if (Info.IsVisible) {
				if (InCameraFrustum) {
					Overlay.Profiler ["# InFrustum"]++;

					screen.RenderEffects.Current.DrawModel (this, time);
				}
			}
		}

		#endregion Draw

		#region Intersection

		public virtual GameObjectDistance Intersects (Ray ray)
		{
			foreach (BoundingSphere sphere in Bounds) {
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

		public Vector3 Center ()
		{
			Vector3 center = Vector3.Zero;
			int count = Model.Meshes.Count;
			foreach (ModelMesh mesh in Model.Meshes) {
				center += mesh.BoundingSphere.Center / count;
			}
			return center / Info.Scale + Info.Position;
		}
		
		#endregion

		#region Cache

		private Vector3 _scale;
		private Angles3 _rotation;
		private Vector3 _position;
		private Matrix _worldMatrix;
		private BoundingSphere[] _bounds;
		private bool _inFrustum;

		public Matrix WorldMatrix {
			get {
				UpdatePrecomputed ();
				return _worldMatrix;
			}
		}

		private BoundingSphere[] Bounds {
			get {
				UpdatePrecomputed ();
				return _bounds;
			}
		}

		protected bool InCameraFrustum {
			get {
				return _inFrustum;
			}
		}

		private void UpdatePrecomputed ()
		{
			if (Info.Scale != _scale || Info.Rotation != _rotation || Info.Position != _position) {

				// world matrix
				_worldMatrix = Matrix.CreateScale (Info.Scale)
					* Matrix.CreateFromYawPitchRoll (Info.Rotation.Y, Info.Rotation.X, Info.Rotation.Z)
					* Matrix.CreateTranslation (Info.Position);

				// bounding spheres
				_bounds = Model.Bounds ().ToArray ();
				for (int i = 0; i < _bounds.Length; ++i) {
					_bounds [i] = _bounds [i].Scale (Info.Scale).Rotate(Info.Rotation).Translate ((Vector3)Info.Position);
				}

				// attrs
				_scale = Info.Scale;
				_rotation = Info.Rotation;
				_position = Info.Position;
			}
		}

		private void OnViewChanged ()
		{
			// camera frustum
			_inFrustum = false;
			foreach (BoundingSphere _sphere in Bounds) {
				var sphere = _sphere;
				if (World.Camera.ViewFrustum.FastIntersects (ref sphere)) {
					_inFrustum = true;
					break;
				}
			}
		}

		#endregion
	}
}

