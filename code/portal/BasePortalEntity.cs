


using Sandbox;

public partial class BasePortalEntity : BaseTrigger
{


	protected ScenePortal so;

	public bool PlayerInPortal( Player player )
	{
		if ( !player.IsValid() ) return false;
		return InPortal( player.PhysicsBody );
	}

	public bool PlayerInPortal( Entity player )
	{
		return PlayerInPortal( player as SandboxPlayer );
	}

	private bool PlayerWasInPortal = false;
	public Vector3 LastPlayerVelocity;

	public virtual float Height { get; set; } = 54f;
	public virtual float Width { get; set; } = 32;


	protected Entity m_shape;


	// only update the view angles if the following conditions are met:
	// * we were inside the portal last frame
	// * we are no longer inside the portal this frame
	// * we were heading into the portal (i.e. we weren't walking out)
	// * or, our teleport was successfully predicted on the client
	private bool ClientTeleported = false;
	public bool JustTeleported => (!PlayerInPortal( Sandbox.Game.LocalPawn ) && PlayerWasInPortal && Rotation.Backward.Dot( LastPlayerVelocity ) > 0.0f && PlayerInPortal( Sandbox.Game.LocalPawn )) || ClientTeleported;



	public Transform LastViewTransform;

	private Dictionary<Entity, Entity> ProjectedEntities = new();

	[Net] public PortalLink Link { get; set; }
	[Net] public bool IsEntrance { get; set; }
	public BasePortalEntity Partner => IsEntrance ? Link?.Entrance : Link?.Exit;

	public bool CanTeleport => IsOpen && Partner.IsValid() && Partner.IsOpen;



	protected List<GenericColliderEnt> m_collider;

	protected GenericColliderEnt PortalFaceCollider;



	[Net] public int RenderResolution { get; set; } = 2048;

	public virtual Material PortalMaterial { get; set; } = Material.Load( "materials/portal_monitor.vmat" );
	[Net]
	public bool IsOpen { get; set; } = false;

	public override void Spawn()
	{
		base.Spawn();
		Tags.Clear();
		Tags.Add( "trigger" );
		Tags.Add( DWTags.Portal );
		Tags.Add( DWTags.NotPortable );
		ActivationTags = DWTags.Player;
		ActivationTags.Add( DWTags.InPortal );
		ActivationTags.Add( DWTags.Solid );

		Transmit = TransmitType.Always;

		EnableAllCollisions = true;
		EnableSolidCollisions = false;
		EnableTouch = true;
		DelaySpawn();

		Predictable = false;

	}

	public async virtual void DelaySpawn()
	{
		await GameTask.CompletedTask;
	}



	public override void ClientSpawn()
	{
		base.ClientSpawn();
		CreateScenePortalClient();
	}
	[ClientRpc]
	public void CreateScenePortal()
	{
		CreateScenePortalClient();
	}

	protected void CreateScenePortalClient()
	{
		if ( so.IsValid() )
		{
			so.Delete();
		}
		so = new ScenePortal( Sandbox.Game.SceneWorld, GeneratePortalModel(), Transform, true, (int)Screen.Width )
		{
			Transform = this.Transform,
			Position = this.Position,
			RenderShadows = true,
		};
		so.Flags.IsTranslucent = false;
		so.Flags.CastShadows = false;
		so.Flags.WantsFrameBufferCopy = true;
		so.RenderingEnabled = false;
	}



	public void clientdelayspawn()
	{

	}

	public void delayedspawn()
	{
		clientdelayspawn();
	}

	public override void FrameSimulate( IClient cl )
	{

	}

	private void TeleportEntity( ModelEntity ent )
	{


		var t = ToLinkedTransform( ent.Transform );
		//var oldvel = ent.PhysicsBody.Velocity;
		//var newvel = 
		//Log.Info( $"Velocity: {oldvel} -> {newvel}" );
		ent.Transform = t;
		ent.PhysicsBody.Velocity = ToLinkedVelocity( ent.PhysicsBody.Velocity );
		ent.ResetInterpolation();


		if ( Game.IsClient )
		{
			ClientTeleported = true;

		}
		else
		{
			Teleported( To.Single( ent ) );
		}

		if ( ent is SandboxPlayer player )
		{
			player.Teleported = 0f;
		}

		Log.Info( $"Teleporting {ent.Name} to {t}" );
		EndTouch( ent );
		Partner.StartTouch( ent );
	}

	[ClientRpc]
	public void Teleported()
	{
		ClientTeleported = true;
	}

	[Event( "buildinput" )]
	public void OverrideInput()
	{

		if ( Sandbox.Game.LocalPawn == null || Sandbox.Game.LocalPawn is not SandboxPlayer player )
			return;

		if ( JustTeleported )
		{
			Transform linkedTransform = GetLinkedCameraTransform( Sandbox.Game.LocalPawn );

			player.EyeRotation = linkedTransform.Rotation;
			player.ViewAngles = linkedTransform.Rotation.Angles();
			player.Controller.Velocity = ToLinkedVelocity( player.Controller.Velocity );
		}

		// don't do otherside check
		PlayerWasInPortal = PlayerInPortal( Sandbox.Game.LocalPawn );
		ClientTeleported = false;
		var p = Sandbox.Game.LocalPawn as SandboxPlayer;
		//Is the velocity even being stored in client? -Rifter
		LastPlayerVelocity = Sandbox.Game.LocalPawn.Velocity;
		LastViewTransform = new Transform( p.EyePosition, p.EyeRotation );
	}



	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );



		// predict on both client and server
		// clone temporarily, as StartTouch/EndTouch is going to modify TouchingEntities
		var entities = new List<Entity>( TouchingEntities );
		foreach ( var ent in entities )
		{

			if ( ent.IsWorld || !ent.IsValid() || ent.Tags.Has( DWTags.PortalGround ) || ent is BasePortalEntity || ent is not ModelEntity mdl )
				continue;
			AddInPortalTags( ent );
			if ( IsOnOtherSide( mdl.PhysicsBody, 1f ) )
			{
				TeleportEntity( mdl );
			}
			if ( ent is not ModelEntity e || !InPortal( e.PhysicsBody ) )
			{
				//Log.Info( $"removing {ent.Name} from portal" );
				EndTouch( ent );
			}
		}

		if ( Game.IsServer && IsOpen )
		{
			//DebugOverlay.Box( WorldSpaceBounds.Mins, WorldSpaceBounds.Maxs, Color.Red );
			//DebugOverlay.Sphere( Position, CollisionBounds.Size.Length / 2, Color.Red );
			foreach ( var ent in Entity.FindInSphere( Position, CollisionBounds.Size.Length / 2 ).ToList() )
			{
				if ( ent is ModelEntity e )
				{
					if ( e.PhysicsBody.IsValid() )
					{
						e.PhysicsBody.Sleeping = false;
					}
				}
				//StartTouch( ent );
				if ( ent is SandboxPlayer player )
				{
					player.LastTouchedPortal = this;
				}
			}
		}

	}


	private float GetCorrectedFOV( float fov )
	{
		var aspect = Screen.Width / Screen.Height;
		return MathF.Atan( MathF.Tan( fov.DegreeToRadian() * 0.5f ) * (aspect * 0.75f) ).RadianToDegree() * 2.0f;
	}

	public virtual void PreUpdatePortalView()
	{
	}

	public virtual void PostUpdatePortalView()
	{


	}


	[Event.PreRender]
	public void UpdatePortalView()
	{
		if ( !so.IsValid() || !this.IsValid() ) return;
		so.Position = this.Position;
		so.Rotation = this.Rotation;
		so.RenderingEnabled = IsOpen;


		PreUpdatePortalView();
		if ( !Game.IsClient || Partner == null )
			return;


		//float zNear = 1.0f;
		Transform camTransform = GetLinkedCameraTransform( Game.LocalPawn, true /* ref zNear */);

		so.ViewPosition = camTransform.Position;
		so.ViewRotation = camTransform.Rotation;
		//so.ZNear = zNear;

		so.FieldOfView = Camera.FieldOfView;

		//so.Aspect = Render.Viewport.Size.x / Render.Viewport.Size.y;
		so.Aspect = Screen.Width / Screen.Height;

		Plane clippingPlane = new Plane( Partner.Position - so.ViewPosition, Partner.Rotation.Forward );
		// small tolerance to prevent seam
		clippingPlane.Distance -= 1.0f;

		so.SetClipPlane( clippingPlane );

		//so.SetClipPlane( new Plane( Partner.Position, Partner.Rotation.Right ) );

		//so.RenderedThisFrame = false;
		//so.CalculateViewportRect();


		PostUpdatePortalView();
	}

	public virtual void UpdateColliders()
	{

	}

	public override void OnTouchStart( Entity other )
	{
		if ( other.IsWorld || other is not ModelEntity ent || other.Tags.Has( DWTags.NotPortable ) || !CanTeleport )
			return;

		if ( ent.PhysicsBody.IsValid() )
		{
			ent.PhysicsBody.Sleeping = false;
			ent.PhysicsBody.AutoSleep = false;
		}
		AddInPortalTags( other );

		base.OnTouchStart( other );
		if ( Game.IsServer && other is not SandboxPlayer )
		{

			if ( ProjectedEntities.ContainsKey( other ) || !InPortalPair( ent.PhysicsBody ) )
				return;
			ProjectedEntities[other] = new ProjectedPortalEntity()
			{
				Target = ent,
				SourcePortal = this
			};
		}
		else if ( Game.IsServer && other is SandboxPlayer player )
		{

			player.LastTouchedPortal = this;
		}
		//if ( IsClient && other == Local.Pawn )
		//{
		//	PlayerInPortal = true;
		//}
	}

	public override void OnTouchEnd( Entity other )
	{
		if ( other.IsWorld || other is not ModelEntity ent )
		{
			return;
		}
		base.OnTouchEnd( other );

		if ( ent.PhysicsBody.IsValid() )
			ent.PhysicsBody.Sleeping = false;

		if ( Game.IsServer && ProjectedEntities.ContainsKey( other ) )
		{
			ProjectedEntities[other]?.Delete();
			ProjectedEntities.Remove( other );
		}

		//Log.Info( $"EndTouch {other}" );
		RemoveInPortalTags( other );
	}


	public virtual void AddInPortalTags( Entity entity )
	{
		if ( entity is SandboxPlayer || !Partner.IsValid() || !CanTeleport ) return; // make the player take care of tags
		entity.Tags.Add( DWTags.InPortal );
		entity.Tags.Remove( DWTags.Solid );
	}

	public virtual void RemoveInPortalTags( Entity entity )
	{
		if ( entity is SandboxPlayer ) return; // make the player take care of tags
		entity.Tags.Remove( DWTags.InPortal );
		entity.Tags.Add( DWTags.Solid );

	}

	public void AddSurfaceTag()
	{
		if ( m_shape.IsValid() )
		{
			m_shape.Tags.Remove( DWTags.PortaledSurface );
		}
		var tr = Trace.Ray( Position, Position + Rotation.Backward * 10 )
				.WithAllTags( "solid" )
				.Ignore( this )
				.Run();

		if ( tr.Hit )
		{
			tr.Entity.Tags.Add( DWTags.PortaledSurface );
			m_shape = tr.Entity;
			//m_shape.AddTag( DWTags.PortaledSurface );
		}
	}



	protected override void OnDestroy()
	{
		base.OnDestroy();

		so?.Delete();
		if ( ProjectedEntities != null )
			foreach ( var ent in ProjectedEntities )
				ent.Value?.Delete();
	}

	public virtual void OpenPortal()
	{

		IsOpen = true;


		OnPortalOpenedInternal();
		AddSurfaceTag();
	}

	[ClientRpc]
	public void OnPortalOpenedInternal()
	{
		OnPortalOpenedClient();
	}

	public virtual void OnPortalOpenedClient()
	{
		OnPortalDisplaced();
	}

	public virtual void ClosePortal()
	{
		IsOpen = false;
		OnPortalClosedInternal();
	}

	[ClientRpc]
	public void OnPortalClosedInternal()
	{

		OnPortalClosedClient();
	}

	public virtual void OnPortalClosedClient()
	{
		if ( so.IsValid() )
		{
			so.RenderingEnabled = false;
		}
	}




	public virtual void OnPortalDisplaced()
	{
		OnPortalDisplacedClientInternal();
	}

	[ClientRpc]
	private void OnPortalDisplacedClientInternal()
	{
		OnPortalDisplacedClient();
	}
	public virtual void OnPortalDisplacedClient()
	{

	}


	public virtual Model GeneratePortalModel()
	{
		return null;
	}
}


public static class PortalExtensions
{

	public static bool IsPortalOpen( this BasePortalEntity portal )
	{
		return portal.IsValid() && portal.IsOpen;
	}
}
