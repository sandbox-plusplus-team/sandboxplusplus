using Sandbox;

namespace tardis.Portals;

public partial class PortalLink : Entity
{
	[Net] public BasePortalEntity Entrance { get; set; }
	[Net] public BasePortalEntity Exit { get; set; }
	public bool PlayerInPortal( SandboxPlayer player )
	{
		if ( !player.IsValid() ) return false;
		return Entrance.PlayerInPortal( player ) || Exit.PlayerInPortal( player );
	}

	public bool CanTeleport => Entrance.IsValid() && Exit.IsValid() && Entrance.CanTeleport;



	public BasePortalEntity JustTeleportedPortal
	{
		get
		{
			if ( Entrance?.JustTeleported ?? false )
				return Entrance;
			if ( Exit?.JustTeleported ?? false )
				return Exit;
			return null;
		}
	}
	public BasePortalEntity PlayerPortal( SandboxPlayer player )
	{
		if ( !PlayerInPortal( player ) )
			return null;

		return (Entrance?.PlayerInPortal( player ) ?? false) ? Entrance : Exit;
	}

	public BasePortalEntity PlayerPortal( Entity player )
	{
		return PlayerPortal( player as SandboxPlayer );
	}

	public override void Spawn()
	{
		base.Spawn();
		Transmit = TransmitType.Always;
	}

	public bool IsInPortalPair( Vector3 Position )
	{
		if ( !Entrance.IsValid() || !Exit.IsValid() || !Entrance.CanTeleport )
			return false;

		return Entrance.InPortalPair( Position );
	}
	public bool IsInPortalPair( PhysicsBody collider )
	{
		if ( !Entrance.IsValid() || !Exit.IsValid() || !Entrance.CanTeleport )
			return false;

		return Entrance.InPortalPair( collider );
	}
	public bool IsInPortalPairFace( Vector3 eyePosition )
	{
		if ( !Entrance.IsValid() || !Exit.IsValid() || !Entrance.CanTeleport )
			return false;
		return Entrance.InPortalPairFace( eyePosition );
	}

	public bool IsInPortalPairFace( PhysicsBody collider )
	{
		if ( !Entrance.IsValid() || !Exit.IsValid() || !Entrance.CanTeleport )
			return false;
		return Entrance.InPortalPairFace( collider );
	}

	public void ClosePortals()
	{
		CloseSinglePortal( true );
		CloseSinglePortal( false );
	}

	public void CloseSinglePortal( bool IsEntrance )
	{
		if ( IsEntrance )
		{
			if ( Entrance.IsValid() )
				Entrance.ClosePortal();
		}
		else
		{
			if ( Exit.IsValid() )
				Exit.ClosePortal();
		}
	}


}
