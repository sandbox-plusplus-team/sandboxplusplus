using System;
using Sandbox;
using Editor;
using System.ComponentModel.DataAnnotations;

namespace tardis.Portals;

/// <summary>
/// A linked portal door, similar to those used in Portal 2.
/// </summary>
[Library( "linked_portal_door" ), HammerEntity]
[Editor.DrawAngles]
[Display( Name = "Linked Portal Door" )/*, Category( "Player" )*/, Icon( "settings_ethernet" ), AutoApplyMaterial( "materials/portal_monitor.vmat" )]
public partial class PortalDoor : BasePortalEntity
{
	public float portal_width;
	public float portal_depth;
	public float portal_height;

	[Property( "Linked Partner" )]
	public EntityTarget PartnerName { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		var P = FindByName( PartnerName.Name ) as PortalDoor;

		// one of the partners will not exist at the time of spawn, so update it here
		IsEntrance = true;
		if ( P.IsValid() )
		{
			LinkPortal( P );
		}

		Transmit = TransmitType.Always;

		var Width = CollisionBounds.Size.y / 2;
		var Height = CollisionBounds.Size.z / 2;

		SetupPhysicsFromOBB( PhysicsMotionType.Keyframed, new Vector3( -4.0f, -Width, -Height ), new Vector3( 4.0f, Width, Height ) );

		SetMaterialOverride( Material.Load( "materials/portal_monitor.vmat" ) );
	}

	public override Model GeneratePortalModel()
	{
		Mesh portalMesh = new( PortalMaterial );

		VertexBuffer buf = new();
		buf.Init( true );

		var Depth = CollisionBounds.Size.x / 2;
		var Width = CollisionBounds.Size.y / 2;
		var Height = CollisionBounds.Size.z / 2;
		//create a box with the same size as the portal

		for ( int i = 0; i < CollisionBounds.Size.x; i++ )
		{
			var v1 = new Vertex( new Vector3( -i, -Width, -Height ), Vector3.Up, Vector3.Right, new() );
			var v2 = new Vertex( new Vector3( -i, Width, -Height ), Vector3.Up, Vector3.Right, new() );
			var v3 = new Vertex( new Vector3( -i, Width, Height ), Vector3.Up, Vector3.Right, new() );
			var v4 = new Vertex( new Vector3( -i, -Width, Height ), Vector3.Up, Vector3.Right, new() );
			buf.AddQuad( v1, v2, v3, v4 );
		}


		portalMesh.CreateBuffers( buf );

		return Model.Builder.AddMesh( portalMesh )
			.Create();
	}

	public override void PreUpdatePortalView()
	{
		base.PreUpdatePortalView();
		if ( SceneObject.IsValid() )
			SceneObject.RenderingEnabled = false;
	}

	public override void OpenPortal()
	{
		base.OpenPortal();
		var trace = Trace.Ray( Position, Position - Rotation.Forward * 1000 )
			.Ignore( this )
			.WithTag( "world" )
			.Run();

		if ( trace.Hit )
		{
			trace.Entity.Tags.Add( DWTags.PortaledSurface );
		}
	}



}
