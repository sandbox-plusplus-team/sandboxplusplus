
using System;
using System.Linq;
using Sandbox;

namespace tardis.Portals;

public partial class Portal : BasePortalEntity
{
	public override float Height => 54f;
	public override float Width => 32;
	public int Segments = 36;



	public float PortalThickness => 30f;
	public float PortalDepth => 8f;
	public float BorderThickness => 30f;
	public float BorderOffset => 0f;
	public float BorderSize => 10f;


	public Particles portalParticles;

	public PortalClientPairComponent PairComponent => Client?.GetPortalClientPairComponent();

	//public PortalFunnelEntity Funnel { get; set; }

	public override Material PortalMaterial { get; set; } = Material.Load( "materials/portal_monitor_special.vmat" );

	public override Model GeneratePortalModel()
	{
		Mesh portalMesh = new( PortalMaterial );

		VertexBuffer buf = new();
		buf.Init( true );

		for ( int i = 0; i < Segments; i++ )
		{
			float a = (float)i / Segments * MathF.PI * 2;
			float b = (float)(i + 1) / Segments * MathF.PI * 2;

			Vector2 UV1 = new Vector2( MathF.Cos( a ), MathF.Sin( a ) );
			Vector2 UV2 = new Vector2( MathF.Cos( b ), MathF.Sin( b ) );

			buf.Add( new Vertex( new Vector3( -0.05f, MathF.Cos( a ) * Width, MathF.Sin( a ) * Height ), UV1, Color.White ) );
			buf.Add( new Vertex( new Vector3( -0.05f, MathF.Cos( b ) * Width, MathF.Sin( b ) * Height ), UV2, Color.White ) );
			buf.Add( new Vertex( new Vector3( -0.05f, 0, 0 ), new Vector2( 0, 0 ), Color.White ) );
		}

		//Create a second plane to make z-fighting less noticable
		for ( int i = 0; i < Segments; i++ )
		{
			float a = (float)i / Segments * MathF.PI * 2;
			float b = (float)(i + 1) / Segments * MathF.PI * 2;

			Vector2 UV1 = new Vector2( MathF.Cos( a ), MathF.Sin( a ) );
			Vector2 UV2 = new Vector2( MathF.Cos( b ), MathF.Sin( b ) );

			buf.Add( new Vertex( new Vector3( -1.2f, MathF.Cos( a ) * Width, MathF.Sin( a ) * Height ), UV1, Color.White ) );
			buf.Add( new Vertex( new Vector3( -1.2f, MathF.Cos( b ) * Width, MathF.Sin( b ) * Height ), UV2, Color.White ) );
			buf.Add( new Vertex( new Vector3( -1.2f, 0, 0 ), new Vector2( 0, 0 ), Color.White ) );
		}


		portalMesh.CreateBuffers( buf );

		return Model.Builder.AddMesh( portalMesh )
			.Create();
	}
	public void ClearColliders()
	{
		if ( m_collider != null )
			foreach ( var item in m_collider )
			{
				if ( item.IsValid() )
					item.Delete();
			}
	}
	public void CreateColliders()
	{
		ClearColliders();

		//if ( !Funnel.IsValid() )
		//{
		//	Funnel = new();
		//	Funnel.SetParent( this );
		//	Funnel.Transform = Transform;
		//}

		SetupPhysicsFromOBB( PhysicsMotionType.Static, new Vector3( -PortalDepth, -Width, -Height ), new Vector3( PortalThickness, Width, Height ) );



		//Funnel.SetupPhysicsFromOBB( PhysicsMotionType.Static, new Vector3( -PortalDepth, -Width * 1.8f, -Height * 1.8f ), new Vector3( PortalThickness * 5f, Width * 1.8f, Height * 1.8f ) );

		PhysicsBody.AutoSleep = false;

		m_collider = new();

		var facecollide = AddCollider( 0, new Vector3( -1, -Width, -Height ), new Vector3( 1, Width, Height ), Rotate: false );
		facecollide?.Tags.Clear();
		facecollide?.Tags.Add( DWTags.PortalFace );
		facecollide?.Tags.Add( DWTags.Portal );
		PortalFaceCollider = facecollide;
		//Bottom Collider

		//AddCollider( new Vector3( -BorderThickness, -Width, Height ), new Vector3( BorderThickness + BorderSize, Width, -Height - 3 ) );

		//LeftCollider
		AddCollider( new Vector3( Rotation.Left * (Width + BorderOffset) ), new Vector3( 0, 0, -Height ), new Vector3( BorderThickness, BorderSize, Height ) );
		////RightCollider
		AddCollider( new Vector3( Rotation.Right * (Width + BorderOffset) ), new Vector3( 0, -BorderSize, -Height ), new Vector3( BorderThickness, 0, Height ) );


		if ( PortalIsVertical( Rotation.Forward ) )
		{
			//Bottom Collider
			AddCollider( new Vector3( Rotation.Down * (Height + BorderOffset) ), new Vector3( -BorderSize, -Width * 2, -BorderThickness ), new Vector3( 0, Width * 2, 0 ), Rotate: false );

			//Top Collider
			var topcol = AddCollider( new Vector3( Rotation.Up * (Height + BorderOffset) ), new Vector3( -BorderSize, -Width * 2, 0 ), new Vector3( 0, Width * 2, BorderThickness ), Rotate: false );
			topcol?.Tags.Add( DWTags.TopPortalGround );
			return;
		}
		// Top Collider
		var top = AddCollider( new Vector3( Rotation.Up * (Height + BorderOffset) ), new Vector3( -BorderSize, -Width * 2, 0 ), new Vector3( 0, Width * 2, BorderThickness ), Rotate: false );
		top?.Tags.Add( DWTags.TopPortalGround );




		var tr = Trace.Ray( Position, Position + (Rotation.Forward * 10f) + (Rotation.Down * 140) )
			.Ignore( this )
			.WithAllTags( "world", "solid" )
			.WorldOnly()
			.Run();
		var portaltr = Trace.Ray( Position, Position + (Rotation.Forward * 10f) + (Rotation.Down * 150) )
		.Ignore( this )
		.WithTag( DWTags.PortalFace )
		.Run();
		if ( !portaltr.Hit && tr.Hit )
		{
			//Bottom Collider
			AddCollider( new Vector3( Rotation.Down * (Height) ), new Vector3( 0, -Width * 2, -BorderSize ), new Vector3( BorderThickness, Width * 2, 0 ), 60 );
			AddCollider( new Vector3( Rotation.Down * (Height) ), new Vector3( 0, -Width * 2, -BorderSize ), new Vector3( BorderThickness - 3, Width * 2, 0 ), 80 );
			AddCollider( new Vector3( Rotation.Down * (Height) ), new Vector3( 0, -Width * 2, -BorderSize ), new Vector3( BorderThickness - 7, Width * 2, 0 ), 100 );
			AddCollider( new Vector3( Rotation.Down * (Height) ), new Vector3( 0, -Width * 2, -BorderSize ), new Vector3( BorderThickness - 10, Width * 2, 0 ), 120 );

			AddCollider( new Vector3( Transform.PointToLocal( tr.HitPosition ) ), new Vector3( -2f, -Width * 3, -BorderSize ), new Vector3( PortalThickness * 3f, Width * 3, 0 ), Rotate: false );
		}
		else
		{
			AddCollider( new Vector3( Rotation.Down * (Height + BorderOffset) ), new Vector3( -BorderSize, -Width * 2, -BorderThickness ), new Vector3( 0, Width * 2, 0 ), Rotate: false );

		}

	}




	public GenericColliderEnt AddCollider( Vector3 PositionOffset, Vector3 Mins, Vector3 Maxs, float RotationAxisDistance = 0, bool Rotate = true )
	{
		BBox box = new( Mins, Maxs );

		var portaltr = Trace.Ray( Position, Position + ((PositionOffset).Normal * (Maxs.Length)) )
		.Ignore( this )
		.WithTag( DWTags.Portal )
		.Radius( 3f )
		.Run();

		//DebugOverlay.TraceResult( portaltr, 5f );

		if ( portaltr.Hit )
		{
			//Log.Info( "Portal Collision" );
			if ( portaltr.Entity is Portal )
			{
				//Log.Info( "Portal Collision" );
				//return null;
				//Get the direction and shorten maxs
				Maxs = portaltr.EndPosition - portaltr.StartPosition;
				return null;
			}
			if ( portaltr.Entity.Parent is Portal )
			{
				//Log.Info( "Portal Collision" );
				Maxs = portaltr.EndPosition - portaltr.StartPosition;
				return null;
				//return null;
			}
		}


		var colliderEnt = new GenericColliderEnt( box )
		{
			Position = Position + PositionOffset,
			Rotation = Rotate ? Rotation.LookAt( PositionOffset + Rotation.Forward * RotationAxisDistance, Rotation.Up ) : Rotation,
		};
		colliderEnt.Tags.Clear();
		colliderEnt.Tags.Add( DWTags.PortalGround );
		colliderEnt.SetParent( this );
		colliderEnt.EnableSolidCollisions = true;
		//colliderEnt.EnableTouch = true;
		//colliderEnt.EnableTouchPersists = true;

		//colliderEnt.OnStartTouch += StartTouch;



		m_collider.Add( colliderEnt );
		return colliderEnt;
	}



	protected override void OnDestroy()
	{
		base.OnDestroy();
		if ( m_shape.IsValid() )
			m_shape.Tags.Remove( "portaledsurface" );
		if ( m_collider != null )
			foreach ( var item in m_collider )
			{
				if ( item.IsValid() )
					item.Delete();
			}
		if ( portalParticles != null )
			portalParticles.Destroy( true );
	}

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );
		
		if ( PairComponent != null && Game.IsServer )
		{
			Link = PairComponent.PortalPair;
		}
		if ( portalParticles == null ) return;
		portalParticles.SetOrientation( 0, Rotation * Rotation.FromPitch( 90f ) );
		portalParticles.SetPosition( 0, Position );
	}

	public override void PreUpdatePortalView()
	{
		base.PreUpdatePortalView();
		if ( so.IsValid() && PairComponent != null )
		{
			so.Transform = Transform;
			so.Attributes.Set( "HasPartner", Partner.IsPortalOpen() );
			so.Attributes.Set( "PortalColor", IsEntrance ? PairComponent.SecondaryColor : PairComponent.PrimaryColor );
			so.Attributes.Set( "PortalOpeningAmount", Displaced * 4 );
			if ( Displaced > 0.3f )
			{
				so.Flags.IsTranslucent = false;
				so.Flags.IsOpaque = true;
			}
			else
			{
				so.Flags.IsTranslucent = true;
				so.Flags.IsOpaque = false;
			}
		}
	}

	public override void OnPortalDisplacedClient()
	{
		base.OnPortalDisplacedClient();
		if ( so.IsValid() )
			so.Transform = Transform;
	}

	public override void ClosePortal()
	{
		base.ClosePortal();
		portalParticles?.Destroy( true );
		ClearColliders();
		PhysicsClear();
		//if ( Funnel.IsValid() )
		//	Funnel.IsActive = false;
		Sound.FromEntity( "portal_close", this );
	}
	public override void OpenPortal()
	{

		SetupPhysicsFromOBB( PhysicsMotionType.Static, new Vector3( -PortalDepth, -Width, -Height ), new Vector3( PortalThickness, Width, Height ) );
		if ( !FitPortal() )
		{
			ClosePortal();
			return;
		}
		Displaced = 0;
		IsOpen = true;
		base.OpenPortal();
		portalParticles?.Destroy( true );
		CreatePortalPariclesAsync();
		CreateColliders();
		//if ( Funnel.IsValid() )
		//	Funnel.IsActive = true;

		var ents = Entity.All.OfType<BasePortalEntity>().Where( x => x.Position.Distance( Position ) < 1000 && x != this );
		foreach ( var item in ents.ToList() )
		{
			item.UpdateColliders();
			//Log.Info( "Updating colliders for" + item );
		}

		Sound.FromEntity( "portal_open", this );
	}
	public override void UpdateColliders()
	{
		base.UpdateColliders();
		if ( IsOpen )
			CreateColliders();
		else
		{
			ClearColliders();
			PhysicsClear();
		}
	}
	[Net]
	private TimeSince Displaced { get; set; }

	private async void CreatePortalPariclesAsync()
	{

		await GameTask.DelaySeconds( 0.4f );
		CreatePortalParticles();
	}

	private void CreatePortalParticles()
	{
		portalParticles = Particles.Create( "particles/portals_effect_parent.vpcf", this, false );
		if ( PairComponent == null )
		{
			return;
		}
		if ( !IsEntrance )
		{
			portalParticles.SetPosition( 5, PairComponent.PrimaryColor );
		}
		else
			portalParticles.SetPosition( 5, PairComponent.SecondaryColor );
	}




	[ConCmd.Server]
	public static void CreatePortal( int number )
	{
		if ( ConsoleSystem.Caller.Pawn is not SandboxPlayer player ) return;
		var pair = Entity.All.OfType<PortalLink>().FirstOrDefault();
		if ( !pair.IsValid() )
		{
			pair = new();
		}

		var tr = Trace.Ray( player.EyePosition, player.EyePosition + player.EyeRotation.Forward * 1000 )
			.UseHitboxes()
			.WithAllTags( "solid", "world" )
			.Ignore( player )
			.Run();
		if ( !tr.Hit ) return;

		var comp = ConsoleSystem.Caller.GetPortalClientPairComponent();

		comp.CreatePortalAt( tr.HitPosition, tr.Normal, number == 0 );


	}



	public static bool PortalIsVertical( Vector3 normal )
	{
		return normal.Angle( Vector3.Up ) < 25f || normal.Angle( Vector3.Down ) < 25f;
	}
}
