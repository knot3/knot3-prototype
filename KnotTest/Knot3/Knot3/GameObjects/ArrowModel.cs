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
using Knot3.UserInterface;

namespace Knot3.GameObjects
{
	public class ArrowModelInfo : GameModelInfo
	{
		public Vector3 Direction;
		public float Length = 40f;
		public float Diameter = 8f;

		public ArrowModelInfo (Vector3 position, Vector3 direction, Vector3 offset)
			: base("pipe1")
		{
			Direction = direction.PrimaryDirection ();
			Position = position + Direction * Node.Scale / 3 + offset;
			Scale = new Vector3 (Diameter, Diameter, Length / 10f);
			// a pipe is movable
			IsMovable = true;
		}

		public override bool Equals (GameObjectInfo other)
		{
			if (other == null) 
				return false;

			if (other is ArrowModelInfo) {
				if (this.Position == other.Position && this.Direction == (other as ArrowModelInfo).Direction && base.Equals (other))
					return true;
				else
					return false;
			} else {
				return base.Equals (other);
			}
		}
	}
	
	public class ArrowModel : GameModel
	{
		#region Attributes and Properties

		public new ArrowModelInfo Info { get { return base.Info as ArrowModelInfo; } set { base.Info = value; } }

		private BoundingSphere[] Bounds;

		#endregion

		public ArrowModel (GameScreen screen, ArrowModelInfo info)
			: base(screen, info)
		{
			if (Info.Direction.Y == 1) {
				Info.Rotation += Angles3.FromDegrees (90, 0, 0);
			} else if (Info.Direction.Y == -1) {
				Info.Rotation += Angles3.FromDegrees (270, 0, 0);
			}
			if (Info.Direction.X == 1) {
				Info.Rotation += Angles3.FromDegrees (0, 90, 0);
			} else if (Info.Direction.X == -1) {
				Info.Rotation += Angles3.FromDegrees (0, 270, 0);
			}

			Bounds = Vectors.CylinderBounds (Info.Length, Info.Diameter / 2, Info.Direction,
			                                 info.Position - info.Direction * Info.Length / 2);
		}

		public override void Draw (GameTime time)
		{
			BaseColor = Color.Red;
			if (World.SelectedObject == this) {
				HighlightIntensity = 1f;
				HighlightColor = Color.Orange;
			} else {
				HighlightIntensity = 0f;
			}

			base.Draw (time);
		}

		public override GameObjectDistance Intersects (Ray ray)
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
	}
}

