using Sandbox;

namespace tardis.Portals;

public class PortalFunnelEntity : BaseTrigger
{
	public bool IsActive { get; set; } = true;
	public override void Spawn()
	{
		base.Spawn();
		ActivationTags.Add( "*" );
		EnableTouchPersists = true;
	}

	public override void Touch( Entity other )
	{
		/* base.Touch( other );

		if ( !IsServer || !other.IsValid() || other.IsWorld || !IsActive )
			return;

		//nudge the velocity to center on this without changing the speed into the portal
		var velocity = other.Velocity;

		var LocalPosition = Transform.PointToLocal( other.Position );
		var LocalVelocity = Transform.NormalToLocal( velocity ).x;
		var LocalDriftVelocity = Transform.NormalToLocal( velocity ).WithX( 0 );

		if ( LocalVelocity < 400 )
		{
			return;
		}
		return;
		var centerdifference = LocalPosition.WithY( 0 ).WithZ( 0 ); //this is the velocity we don't care about

		Vector3.SmoothDamp( LocalPosition, centerdifference, ref LocalDriftVelocity, 1f, Time.Delta );
		other.Velocity = Transform.NormalToWorld( LocalDriftVelocity.WithX( LocalVelocity ) );

		other.Velocity = other.Velocity.ClampLength( 1000 );

 */



	}







}
