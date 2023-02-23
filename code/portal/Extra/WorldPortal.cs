using System;
using Sandbox;


namespace tardis.Portals;



public partial class WorldPortal : BasePortalEntity
{
	/// <summary>
	/// X is Depth, Y is Width , Z is Height
	/// </summary>
	[Net] public Vector3 Extends { get; set; }
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

		UpdatePortalCollisions();
	}

	public void UpdatePortalCollisions()
	{
		var Width = Extends.y / 2;
		var Height = Extends.z / 2;
		var Depth = Extends.x / 2;

		SetupPhysicsFromOBB( PhysicsMotionType.Keyframed, new Vector3( -Depth, -Width, -Height ), new Vector3( Depth, Width, Height ) );

		SetMaterialOverride( PortalMaterial );
	}



	public override Model GeneratePortalModel()
	{
		Mesh portalMesh = new( PortalMaterial );

		VertexBuffer buf = new();
		buf.Init( true );

		var Width = Extends.y / 2;
		var Height = Extends.z / 2;
		var Depth = Extends.x / 2;

		//create a box with the same size as the portal

		/* for ( int i = 0; i < Depth; i++ )
		{
			var v1 = new Vertex( new Vector3( -i, -Width, -Height ), Vector3.Up, Vector3.Right, new() );
			var v2 = new Vertex( new Vector3( -i, Width, -Height ), Vector3.Up, Vector3.Right, new() );
			var v3 = new Vertex( new Vector3( -i, Width, Height ), Vector3.Up, Vector3.Right, new() );
			var v4 = new Vertex( new Vector3( -i, -Width, Height ), Vector3.Up, Vector3.Right, new() );
			buf.AddQuad( v1, v2, v3, v4 );
		} */

		//Back
		{
			var v1 = new Vertex( new Vector3( -Depth, -Width, -Height ), Vector3.Up, Vector3.Right, new() );
			var v2 = new Vertex( new Vector3( -Depth, Width, -Height ), Vector3.Up, Vector3.Right, new() );
			var v3 = new Vertex( new Vector3( -Depth, Width, Height ), Vector3.Up, Vector3.Right, new() );
			var v4 = new Vertex( new Vector3( -Depth, -Width, Height ), Vector3.Up, Vector3.Right, new() );
			buf.AddQuad( v1, v2, v3, v4 );
		}
		//Left
		{
			var v1 = new Vertex( new Vector3( -Depth, -Width, -Height ), Vector3.Up, Vector3.Right, new() );
			var v2 = new Vertex( new Vector3( -Depth, -Width, Height ), Vector3.Up, Vector3.Right, new() );
			var v3 = new Vertex( new Vector3( 0, -Width, Height ), Vector3.Up, Vector3.Right, new() );
			var v4 = new Vertex( new Vector3( 0, -Width, -Height ), Vector3.Up, Vector3.Right, new() );
			buf.AddQuad( v1, v2, v3, v4 );
		}
		//Right
		{
			var v4 = new Vertex( new Vector3( -Depth, Width, -Height ), Vector3.Up, Vector3.Right, new() );
			var v3 = new Vertex( new Vector3( -Depth, Width, Height ), Vector3.Up, Vector3.Right, new() );
			var v2 = new Vertex( new Vector3( 0, Width, Height ), Vector3.Up, Vector3.Right, new() );
			var v1 = new Vertex( new Vector3( 0, Width, -Height ), Vector3.Up, Vector3.Right, new() );
			buf.AddQuad( v1, v2, v3, v4 );
		}
		//Top
		{
			var v1 = new Vertex( new Vector3( -Depth, -Width, Height ), Vector3.Up, Vector3.Right, new() );
			var v2 = new Vertex( new Vector3( -Depth, Width, Height ), Vector3.Up, Vector3.Right, new() );
			var v3 = new Vertex( new Vector3( 0, Width, Height ), Vector3.Up, Vector3.Right, new() );
			var v4 = new Vertex( new Vector3( 0, -Width, Height ), Vector3.Up, Vector3.Right, new() );
			buf.AddQuad( v1, v2, v3, v4 );
		}
		//Bottom
		{
			var v4 = new Vertex( new Vector3( -Depth, -Width, -Height ), Vector3.Up, Vector3.Right, new() );
			var v3 = new Vertex( new Vector3( -Depth, Width, -Height ), Vector3.Up, Vector3.Right, new() );
			var v2 = new Vertex( new Vector3( 0, Width, -Height ), Vector3.Up, Vector3.Right, new() );
			var v1 = new Vertex( new Vector3( 0, -Width, -Height ), Vector3.Up, Vector3.Right, new() );
			buf.AddQuad( v1, v2, v3, v4 );
		}



		portalMesh.CreateBuffers( buf );

		return Model.Builder.AddMesh( portalMesh )
			.Create();
	}


	public Model Generatecube()
	{
		Mesh portalMesh = new( PortalMaterial );

		var positions = new Vector3[]
			{
				new Vector3(-0.5f, -0.5f, 0.5f) * Extends.y,
				new Vector3(-0.5f, 0.5f, 0.5f) * Extends.y,
				new Vector3(0.5f, 0.5f, 0.5f) * Extends.y,
				new Vector3(0.5f, -0.5f, 0.5f) * Extends.y,
				new Vector3(-0.5f, -0.5f, -0.5f) * Extends.y,
				new Vector3(-0.5f, 0.5f, -0.5f) * Extends.y,
				new Vector3(0.5f, 0.5f, -0.5f) * Extends.y,
				new Vector3(0.5f, -0.5f, -0.5f) * Extends.y,
			};

		var faceIndices = new int[]
		{
				0, 1, 2, 3,
				7, 6, 5, 4,
				0, 4, 5, 1,
				1, 5, 6, 2,
				2, 6, 7, 3,
				3, 7, 4, 0,
		};

		var uAxis = new Vector3[]
		{
				Vector3.Forward,
				Vector3.Left,
				Vector3.Left,
				Vector3.Forward,
				Vector3.Right,
				Vector3.Backward,
		};

		var vAxis = new Vector3[]
		{
				Vector3.Left,
				Vector3.Forward,
				Vector3.Down,
				Vector3.Down,
				Vector3.Down,
				Vector3.Down,
		};

		List<SimpleVertex> verts = new();
		List<int> indices = new();

		for ( var i = 0; i < 6; ++i )
		{
			var tangent = uAxis[i];
			var binormal = vAxis[i];
			var normal = Vector3.Cross( tangent, binormal );

			for ( var j = 0; j < 4; ++j )
			{
				var vertexIndex = faceIndices[(i * 4) + j];
				var pos = positions[vertexIndex];

				verts.Add( new SimpleVertex()
				{
					position = pos,
					normal = normal,
					tangent = tangent,
					texcoord = Planar( (Position + pos) / 32, uAxis[i], vAxis[i] )
				} );
			}

			indices.Add( i * 4 + 0 );
			indices.Add( i * 4 + 2 );
			indices.Add( i * 4 + 1 );
			indices.Add( i * 4 + 2 );
			indices.Add( i * 4 + 0 );
			indices.Add( i * 4 + 3 );
		}

		portalMesh.CreateVertexBuffer<SimpleVertex>( verts.Count, SimpleVertex.Layout, verts.ToArray() );
		portalMesh.CreateIndexBuffer( indices.Count, indices.ToArray() );


		return Model.Builder.AddMesh( portalMesh )
			.Create();
	}
	protected static Vector2 Planar( Vector3 pos, Vector3 uAxis, Vector3 vAxis )
	{
		return new Vector2()
		{
			x = Vector3.Dot( uAxis, pos ),
			y = Vector3.Dot( vAxis, pos )
		};
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
	[ClientRpc]
	public void DeletePortal()
	{
		if ( so.IsValid() )
		{
			so.Delete();
		}
	}
}
