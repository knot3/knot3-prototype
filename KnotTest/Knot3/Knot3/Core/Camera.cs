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

namespace Knot3.Core
{
	/// <summary>
	/// Ein GameScreenComponent, der bei jedem Update die View-, World- und Projectionmatrizen auf Basis der 
	/// aktuellen Kameraposition neu berechnet.
	/// </summary>
	public class Camera : GameScreenComponent
	{
		World World { get; set; }

		/// <summary>
		/// Gets the world matrix.
		/// </summary>
		/// <value>
		/// The world matrix.
		/// </value>
		public Matrix WorldMatrix { get; private set; }

		/// <summary>
		/// Gets the view matrix.
		/// </summary>
		/// <value>
		/// The view matrix.
		/// </value>
		public Matrix ViewMatrix { get; private set; }

		/// <summary>
		/// Gets the projection matrix.
		/// </summary>
		/// <value>
		/// The projection matrix.
		/// </value>
		public Matrix ProjectionMatrix { get; private set; }

		public BoundingFrustum ViewFrustum { get; private set; }

		/// <summary>
		/// Gets the default position.
		/// </summary>
		/// <value>
		/// The default position.
		/// </value>
		public Vector3 DefaultPosition { get; private set; }

		private Vector3 _position;

		/// <summary>
		/// Gets or sets the current camera position.
		/// </summary>
		/// <value>
		/// The current camera position.
		/// </value>
		public Vector3 Position {
			get { return _position; }
			set {
				OnViewChanged ();
				_position = value;
			}
		}

		private Vector3 _target;

		/// <summary>
		/// Gets or sets the current camera target.
		/// </summary>
		/// <value>
		/// The current camera target.
		/// </value>
		public Vector3 Target {
			get { return _target; }
			set {
				OnViewChanged ();
				_target = value;
			}
		}

		public Vector3 UpVector { get; private set; }

		private float foV;

		/// <summary>
		/// Gets or sets the field of view.
		/// </summary>
		/// <value>
		/// The field of view.
		/// </value>
		public float FoV {
			get { return foV; }
			set { foV = MathHelper.Clamp (value, 40, 100); }
		}

		public Action OnViewChanged = () => {};
		public Angles3 RotationAngle = Angles3.Zero;
		private Angles3 AutoRotation = Angles3.Zero;
		private float aspectRatio;
		private float nearPlane;
		private float farPlane;

		/// <summary>
		/// Initializes a new instance of the <see cref="Knot3.Core.Camera"/> class.
		/// </summary>
		/// <param name='screen'>
		/// Game State.
		/// </param>
		public Camera (GameScreen screen, World world)
			: base(screen, DisplayLayer.None)
		{
			World = world;
			DefaultPosition = new Vector3 (400, 400, 700);
			Position = DefaultPosition;
			Target = new Vector3 (0, 0, 0);
			UpVector = Vector3.Up;
 
			FoV = MathHelper.ToDegrees (MathHelper.PiOver4);
			aspectRatio = screen.viewport.AspectRatio;
			nearPlane = 0.5f;
			farPlane = 10000.0f;
			
			UpdateMatrices (null);
		}

		public override void Update (GameTime time)
		{
			UpdateRotation (time);
			UpdateMatrices (time);
		}

		private void UpdateRotation (GameTime time)
		{
			// auto rotation
			RotationAngle += AutoRotation;
		}

		private void UpdateMatrices (GameTime time)
		{ 
			// setting up rotation
			ViewMatrix = Matrix.CreateLookAt (Position, Target, UpVector);
			WorldMatrix = Matrix.CreateFromYawPitchRoll (RotationAngle.Y, RotationAngle.X, RotationAngle.Z);
			ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView (MathHelper.ToRadians (FoV), aspectRatio, nearPlane, farPlane);
			ViewFrustum = new BoundingFrustum (ViewMatrix * ProjectionMatrix);
		}

		/// <summary>
		/// Gets or sets the distance between camera position and camera target.
		/// </summary>
		/// <value>
		/// The target distance.
		/// </value>
		public float TargetDistance {
			get {
				Vector3 toTarget = Target - Position;
				return toTarget.Length ();
			}
			set {
				Vector3 toPosition = Position - Target;
				if (Math.Abs (value) > 300) {
					Position = Target + toPosition * value / toPosition.Length ();
				} else {
					Position = Target + toPosition * 300 / toPosition.Length ();
				}
			}
		}

		/// <summary>
		/// Gets the normalized direction from the camera position to the camera target.
		/// </summary>
		/// <value>
		/// The target direction.
		/// </value>
		public Vector3 TargetDirection {
			get {
				Vector3 toTarget = Target - Position;
				toTarget.Normalize ();
				return toTarget;
			}
		}

		/// <summary>
		/// Gets the mouse ray from the specified 2D mouse position.
		/// </summary>
		/// <returns>
		/// The mouse ray.
		/// </returns>
		/// <param name='mouse'>
		/// 2D Mouse position.
		/// </param>
		public Ray GetMouseRay (Vector2 mouse)
		{
			Viewport viewport = screen.viewport;

			Vector3 nearPoint = new Vector3 (mouse, 0);
			Vector3 farPoint = new Vector3 (mouse, 1);

			nearPoint = viewport.Unproject (nearPoint, ProjectionMatrix, ViewMatrix, Matrix.Identity);
			farPoint = viewport.Unproject (farPoint, ProjectionMatrix, ViewMatrix, Matrix.Identity);

			Vector3 direction = farPoint - nearPoint;
			direction.Normalize ();

			return new Ray (nearPoint, direction);
		}

		/// <summary>
		/// Gets the current arcball target's position.
		/// </summary>
		/// <value>
		/// The current arcball target's position.
		/// </value>
		public Vector3 ArcballTarget {
			get {
				if (World.SelectedObject != null)
					return World.SelectedObject.Center ();
				else
					return Vector3.Zero;
			}
		}
	}
}

