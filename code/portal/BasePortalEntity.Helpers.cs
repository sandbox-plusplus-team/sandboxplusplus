

using Sandbox;

public partial class BasePortalEntity
{


	public Transform GetLinkedCameraTransform( Entity pawn, bool forceCorrect = false /* ref float zNear */)
	{
		if ( Partner == null || pawn == null )
			return new Transform();

		// use camera transform so we don't break anything when we override it
		//Transform camTransform = forceCorrect ? new Transform( Camera.Position, Camera.Rotation ) : new Transform( pawn.AimRay.Position, pawn is Player player ? player.EyeRotation : pawn.Rotation );
		Transform camTransform = new Transform( Camera.Position, Camera.Rotation );
		//Transform camTransform = new Transform( CurrentView.Position, CurrentView.Rotation );

		Transform relativeTransform = Transform.ToLocal( camTransform );

		//zNear = Math.Max( zNear, relativeTransform.Position.Length - Radius );

		Transform rotatedTransform = Partner.Transform;
		rotatedTransform.Rotation = Rotation.LookAt( rotatedTransform.Rotation.Backward, rotatedTransform.Rotation.Up );

		return rotatedTransform.ToWorld( relativeTransform );
	}

	public Transform ToLinkedTransform( Transform t )
	{
		if ( !Partner.IsValid() )
		{
			return t;
		}
		Transform relativeTransform = Transform.ToLocal( t );

		Transform rotatedTransform = Partner.Transform;
		rotatedTransform.Rotation = Rotation.LookAt( rotatedTransform.Rotation.Backward, rotatedTransform.Rotation.Up );

		return rotatedTransform.ToWorld( relativeTransform );
	}

	public Vector3 ToLinkedVelocity( Vector3 velocity )
	{
		//to local vector
		Vector3 v = Rotation.Inverse * velocity;
		//Vector3 v = velocity * Rotation.Inverse;
		//
		Rotation rotated = Partner.Rotation;
		rotated = Rotation.LookAt( rotated.Backward, rotated.Up );
		var newvel = rotated * v;
		return newvel;
	}

	public float GetAngleToPortal( Vector3 pos, float offset = 0.0f )
	{
		Vector3 portalPosition = Position + Rotation.Backward * offset;
		return Vector3.Dot( pos - portalPosition, Rotation.Backward );
	}

	public bool IsOnOtherSide( PhysicsBody pos, float offset = 0.0f )
	{
		return GetAngleToPortal( pos.MassCenter, offset ) > 0.0f && InPortal( pos );
	}

	public bool InPortal( PhysicsBody collider )
	{
		if ( !Partner.IsValid() || !CanTeleport )
		{
			return false;
		}
		return Trace.Body( collider, collider.Position ).WithTag( DWTags.Portal ).Run().Hit;
	}
	public bool InPortalFace( PhysicsBody collider )
	{
		if ( !Partner.IsValid() || !CanTeleport )
		{
			return false;
		}
		if ( !PortalFaceCollider.IsValid() )
		{
			return InPortal( collider );
		}
		return Trace.Body( collider, collider.Position ).WithTag( DWTags.PortalFace ).Run().Hit;
	}
	public bool InPortalPair( PhysicsBody collider )
	{
		if ( !Partner.IsValid() || !CanTeleport )
		{
			return false;
		}
		return InPortal( collider ) || Partner.InPortal( collider );
	}

	public bool InPortalPairFace( PhysicsBody collider )
	{
		if ( !Partner.IsValid() || !CanTeleport )
		{
			return false;
		}
		return InPortalFace( collider ) || Partner.InPortalFace( collider );
	}


	public bool InPortalFace( Vector3 Position )
	{
		if ( !Partner.IsValid() || !CanTeleport )
		{
			return false;
		}
		if ( !PortalFaceCollider.IsValid() )
		{
			return InPortal( Position );
		}
		return PortalFaceCollider.PointInside( Position )/*  || (PortalFaceCollider.CollisionBounds + PortalFaceCollider.Position).Contains( Position ) */;
	}


	public bool InPortal( Vector3 pos )
	{
		if ( !Partner.IsValid() || !CanTeleport )
		{
			return false;
		}

		return (CollisionBounds).Contains( Transform.PointToLocal( pos ) );
	}


	public bool InPortalPair( Vector3 collider )
	{
		if ( !Partner.IsValid() || !CanTeleport )
		{
			return false;
		}
		return (CollisionBounds).Contains( Transform.PointToLocal( collider ) );
	}

	public bool InPortalPairFace( Vector3 eyePosition )
	{
		if ( !Partner.IsValid() || !CanTeleport )
		{
			return false;
		}
		return InPortalFace( eyePosition ) || Partner.InPortalFace( eyePosition );
	}

	public bool LinkPortal( BasePortalEntity portal )
	{
		if ( !portal.IsValid() || portal == this )
		{
			return false;
		}
		if ( Link.IsValid() )
		{
			Link.Delete();
		}
		IsEntrance = false;
		portal.IsEntrance = true;
		Link = new()
		{
			Entrance = this,
			Exit = portal,
		};
		portal.Link = Link;

		OpenPortal();
		portal.OpenPortal();

		return true;
	}


}
