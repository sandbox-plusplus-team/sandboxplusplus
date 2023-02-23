namespace tardis.Portals;

public class PortalLinkCameraModifier : CameraModifier
{
	public PortalLink Parent;
	private SandboxPlayer Player;

	public PortalLinkCameraModifier( SandboxPlayer player )
	{
		Player = player;
	}
	public override bool Update()
	{
		if ( !Parent.IsValid() )
		{
			return true;
		}

		BasePortalEntity teleportedPortal = Parent.JustTeleportedPortal;

		if ( Player.Client.IsInAPortalFace() )
		{
			Camera.ZNear = 0.05f;
		}

		if ( teleportedPortal != null )
		{
			Log.Info( "camera modifier smoothing" );
			// if we've just teleported, help smooth things over until viewangles can be updated
			Transform linkedTransform = teleportedPortal.ToLinkedTransform( teleportedPortal.LastViewTransform );

			Camera.Position = linkedTransform.Position;
			Camera.Rotation = linkedTransform.Rotation;

			// estimate where the camera should be
			Camera.Position += teleportedPortal.ToLinkedVelocity( teleportedPortal.LastPlayerVelocity ) * (1.0f / 120.0f);
			//setup.Position += teleportedPortal.ToLinkedVelocity( teleportedPortal.LastPlayerVelocity ) * (1.0f / 120.0f);
		}
		else if ( Parent.PlayerInPortal( Player ) )
		{
			if ( Sandbox.Game.LocalPawn != null && Parent.PlayerPortal( Player ).IsOnOtherSide( Player.PhysicsBody ) )
			{
				Transform linkedTransform = Parent.PlayerPortal( Player ).GetLinkedCameraTransform( Player );

				Camera.Position = linkedTransform.Position;
				Camera.Rotation = linkedTransform.Rotation;
			}

		}



		return true;
	}
}
