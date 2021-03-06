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

namespace Knot3.GameObjects
{
	/// <summary>
	/// Ein IGameObject ist ein Objekt, zum Beispiel ein 3D-Modell (Klasse GameModel),
	/// das in der 3D-Welt (Klasse World) gezeichnet wird.
	/// </summary>
	public interface IGameObject
	{
		GameObjectInfo Info { get; }

		World World { get; set; }

		void Draw (GameTime time);

		void Update (GameTime time);

		/// <summary>
		/// Schneiden sich der (meist aus einer Mausposition berechnete) Ray und das Spielobjekt?
		/// </summary>
		/// <param name='ray'>
		/// The specified Ray.
		/// </param>
		GameObjectDistance Intersects (Ray ray);

		/// <summary>
		/// Die Mitte des Spielobjekts.
		/// </summary>
		Vector3 Center ();
	}

	public class GameObjectInfo : IEquatable<GameObjectInfo>
	{
		public Vector3 Position;
		public bool IsVisible;
		public bool IsSelectable;
		public bool IsMovable;

		public GameObjectInfo ()
		{
			Position = Vector3.Zero;
			IsVisible = true;
			IsSelectable = true;
			IsMovable = false;
		}

		public virtual bool Equals (GameObjectInfo other)
		{
			if (other == null) {
				return false;
			}

			if (this.Position == other.Position) {
				return true;
			}
			else {
				return false;
			}
		}

		public override bool Equals (Object obj)
		{
			if (obj == null) {
				return false;
			}

			GameObjectInfo personObj = obj as GameObjectInfo;
			if (personObj == null) {
				return false;
			}
			else {
				return Equals (personObj);
			}
		}

		public override int GetHashCode ()
		{
			return this.Position.GetHashCode ();
		}

		public static bool operator == (GameObjectInfo o1, GameObjectInfo o2)
		{
			if ((object)o1 == null || ((object)o2) == null) {
				return Object.Equals (o1, o2);
			}

			return o2.Equals (o2);
		}

		public static bool operator != (GameObjectInfo o1, GameObjectInfo o2)
		{
			return ! (o1 == o2);
		}
	}
}
