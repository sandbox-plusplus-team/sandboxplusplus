
using Sandbox;
using System.Linq;
using tardis.Portals;



public partial class PortalClientPairComponent : EntityComponent
{
	[Net]
	public PortalLink PortalPair { get; set; }

	public BasePortalEntity PrimaryPortal => PortalPair?.Entrance;
	public BasePortalEntity SecondaryPortal => PortalPair?.Exit;

	[Net]
	public Color PrimaryColor { get; set; } = Color.FromBytes( 64, 160, 255 );

	[Net]
	public Color SecondaryColor { get; set; } = Color.FromBytes( 255, 160, 32 );

	public void CreatePortalAt( Vector3 Position, Vector3 Normal, bool IsPrimary )
	{
		if ( Game.IsClient ) return;
		if ( !PortalPair.IsValid() )
		{
			PortalPair = new PortalLink();
		}
		Rotation PortalRotation = Rotation.LookAt( Normal, Vector3.Up );
		Vector3 PortalPosition = Position + Normal * 0.2f;

		


		


		if ( (Normal.Angle( Vector3.Up ) < 25f || Normal.Angle( Vector3.Down ) < 25f)  )
		{
			//if the portal is facing up, rotate it on the normal axis so the bottom of the portal is facing the player
			Vector3 forward = (Position - Entity.Position).Normal;

			PortalRotation = Rotation.LookAt( Normal, forward );

		}
		if ( PrimaryPortal.IsValid() && !IsPrimary )
		{
			PrimaryPortal.Position = PortalPosition;
			PrimaryPortal.Rotation = PortalRotation;

			PrimaryPortal.OpenPortal();

			
		}
		else if ( SecondaryPortal.IsValid() && IsPrimary )
		{
			SecondaryPortal.Position = PortalPosition;
			SecondaryPortal.Rotation = PortalRotation;
			SecondaryPortal.OpenPortal();
		
		}
		else
		{
			var portal = new Portal()
			{
				Position = PortalPosition,
				Rotation = PortalRotation,
				IsEntrance = !IsPrimary,
				Link = PortalPair,
				Owner = Entity
			};
			if ( !IsPrimary )
			{
				PortalPair.Entrance = portal;
			}
			else
			{
				PortalPair.Exit = portal;
			}
			portal.OpenPortal();
		
		}

		if ( SecondaryPortal.IsValid() )
		{
			SecondaryPortal.Link = PortalPair;
			SecondaryPortal.IsEntrance = true;

		}
		if ( PrimaryPortal.IsValid() )
		{
			PrimaryPortal.Link = PortalPair;
			PrimaryPortal.IsEntrance = false;
		}


	}
}

// Extension method to get the component from the Client

public static class PortalClientPairComponentExtensions
{
	public static PortalClientPairComponent GetPortalClientPairComponent( this IClient client )
	{
		if ( Game.IsServer )
			return client.Components.GetOrCreate<PortalClientPairComponent>();
		else
			return client.Components.Get<PortalClientPairComponent>();
	}

	public static PortalLink GetPortalPair( this IClient client )
	{
		var paircomponent = client.GetPortalClientPairComponent();
		var pair = paircomponent.PortalPair;
		if ( Game.IsServer && !pair.IsValid() )
		{
			paircomponent.PortalPair = new PortalLink();
			pair = paircomponent.PortalPair;
		}
		return pair;
	}

	public static BasePortalEntity GetPrimaryPortal( this IClient client )
	{
		return client.GetPortalClientPairComponent()?.PrimaryPortal;
	}

	public static BasePortalEntity GetSecondaryPortal( this IClient client )
	{
		return client.GetPortalClientPairComponent()?.SecondaryPortal;
	}

	public static void CreatePortalAt( this IClient client, Vector3 Position, Vector3 Normal, bool IsPrimary )
	{
		client.GetPortalClientPairComponent().CreatePortalAt( Position, Normal, IsPrimary );
	}

	public static bool IsInAPortal( this IClient client )
	{

		//CanTeleport is not set up yet. I had to change it back to IsOpen for now to get the gamemode to compile.

		if ( client.Pawn is Player player )
		{
			return client.GetPrimaryPortal()?.InPortalPair( player.PhysicsBody ) ?? false;
		}
		return false;
	}
	public static bool IsInAPortalFace( this IClient client )
	{
		if ( client.Pawn is Player player )
		{
			return (client.GetPrimaryPortal()?.InPortalPairFace( player.PhysicsBody ) ?? false);
		}
		return false;
	}


	public static Player GetPlayer( this IClient client )
	{
		return client.Pawn as Player;
	}

	public static void ClosePlayerPortals( this IClient client )
	{
		client.GetPrimaryPortal()?.ClosePortal();
		client.GetSecondaryPortal()?.ClosePortal();
	}

	public static void ClosePortal( this IClient client, bool Entrance = true )
	{
		if ( Entrance )
			client.GetPrimaryPortal()?.ClosePortal();
		else
			client.GetSecondaryPortal()?.ClosePortal();
	}
}

