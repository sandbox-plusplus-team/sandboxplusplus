using Sandbox;
using System;
using System.Collections.Generic;

namespace tardis.Portals;

public partial class Portal : BasePortalEntity
{
	[ConVar.Replicated] public static bool DebugFitting { get; set; } = false;

	private Trace PortalTrace => Trace.Sphere( 2f, 0, 0 ).WithAnyTags( "solid", "portalface" ).WithoutTags( "prop", "p2physics", DWTags.NotPortable ).Ignore( this ).WorldAndEntities();

	private float MaxBumpDistance => (Width + Height) * 4;
	private const float BumpDistance = 3f;
	private const int MaxRecursionAmount = 30;

	private Vector3 OriginalPosition;

	struct CornerFitData
	{
		public TraceResult trace;
		public bool hitsomething;

		public Vector3 BumpDirection;
		public float BumpFraction;
		public bool IsBackTrace;

		public bool HitPortal()
		{
			return hitsomething && trace.Entity is GenericColliderEnt portal && portal.Tags.Has( "portalface" );
		}
		public bool IsPortalSameWall( Vector3 normal )
		{
			return hitsomething && trace.Entity is GenericColliderEnt portal && portal.Tags.Has( "portalface" ) && portal.Rotation.Forward.AlmostEqual( normal, 0.1f );
		}

		public static bool IsPortalSameWall( TraceResult tr, Vector3 normal )
		{
			return tr.Entity is GenericColliderEnt portal && portal.Tags.Has( "portalface" ) && portal.Rotation.Forward.AlmostEqual( normal, 0.1f );
		}
	}
	public bool FitPortal( int recursionamount = 0, bool fullcheck = false )
	{
		if ( recursionamount == 0 )
		{
			OriginalPosition = Position;
		}
		if ( recursionamount > MaxRecursionAmount || OriginalPosition.Distance( Position ) > MaxBumpDistance ) return false;
		//Make sure we actually are on a surface. because bumping can cause us to be in the air
		Position = PortalTrace.FromTo( Position, Position + Rotation.Backward * 100 ).Ignore( this ).Run().EndPosition;
		//Check if the portal is already fitted


		//Log.Info( prefitTest.StartedSolid );
		//DebugOverlay.Box( CollisionBounds.Mins + new Vector3( Position + Rotation.Forward * PortalThickness * 2 ), CollisionBounds.Maxs + new Vector3( Position + Rotation.Forward * PortalThickness * 2 ), Color.Red, 2, true );
		//DebugOverlay.TraceResult( prefitTest, 2f );



		List<Vector3> corners = new(){
			((Rotation.Up * Height) + (Rotation.Right * Width)),
			((Rotation.Up * Height) + (Rotation.Left * Width)),
			((Rotation.Down * Height) + (Rotation.Right * Width)),
			((Rotation.Down * Height) + (Rotation.Left * Width)),
			/* (Rotation.Right * Width),
			(Rotation.Left * Width),
			(Rotation.Up * Height),
			(Rotation.Down * Height) */
		};
		if ( fullcheck )
		{
			corners.Add( (Rotation.Right * Width) );
			corners.Add( (Rotation.Left * Width) );
			corners.Add( (Rotation.Up * Height) );
			corners.Add( (Rotation.Down * Height) );

			corners.Add( ((Rotation.Right + Rotation.Up).Normal * Height) );
			corners.Add( ((Rotation.Right + Rotation.Down).Normal * Height) );
			corners.Add( ((Rotation.Left + Rotation.Up).Normal * Height) );
			corners.Add( ((Rotation.Left + Rotation.Down).Normal * Height) );

		}

		List<CornerFitData> cornerFits = new();
		foreach ( var item in corners )
		{
			if ( !TracePortalCorners( Position, item, Rotation.Forward, out var result ) )
			{
				cornerFits.Add( result );
			}
		}
		int IntersectionCount = cornerFits.Count;
		switch ( IntersectionCount )
		{
			case 0:
				if ( fullcheck )
				{
					/* var finaltrace = Trace.Sweep( PhysicsBody, new Transform( Position + (Rotation.Forward * PortalThickness * 2), Rotation ), new Transform( Position + (Rotation.Forward * (PortalThickness * 2)), Rotation ) ).WithTag( "solid" ).WorldAndEntities().Run();
					if ( finaltrace.Hit )
					{
						DebugOverlay.TraceResult( finaltrace, 2f );
						return false;
					} */
					return true;
				}
				return FitPortal( recursionamount + 1, true );
			case 1:
				//One intersection therefore we can try to move it to fit
				Position += cornerFits[0].BumpDirection * BumpDistance;
				Vector3 LocalPoint = Transform.PointToLocal( cornerFits[0].trace.HitPosition );
				Vector2 point = new Vector2( LocalPoint.y, LocalPoint.z );
				if ( !CheckIfPointIsinPortal( point * (MathF.Abs( cornerFits[0].trace.Normal.Dot( -cornerFits[0].trace.Direction ) ) / 1) ) ) return true;
				return FitPortal( recursionamount + 1 );
			case 2:
				//Two intersections
				float fDot = cornerFits[0].BumpDirection.Dot( cornerFits[1].BumpDirection );
				//this means the two intersections are on the same line so scoot the portal a bit away from it and try again
				if ( fDot > 0.9f )
				{
					Position += cornerFits[0].BumpDirection * (BumpDistance);
					return FitPortal( recursionamount + 1 );
				}
				else
				{
					//The two intersections are on different lines so we can scoot the portal to the intersection
					Position += (cornerFits[0].BumpDirection + cornerFits[1].BumpDirection) * BumpDistance;
					return FitPortal( recursionamount + 1 );
				}
			case 3:
				//Three intersections
				float[] fdots = new float[3]{
					cornerFits[0].BumpDirection.Dot( cornerFits[1].BumpDirection ),
					cornerFits[1].BumpDirection.Dot( cornerFits[2].BumpDirection ),
					cornerFits[2].BumpDirection.Dot( cornerFits[0].BumpDirection )
				};
				int similarwalls = 0;
				foreach ( var item in fdots )
				{
					if ( item > 0.9f )
					{
						similarwalls++;
					}
				}

				if ( similarwalls == 3 )
				{
					//All three intersections are on the same line so scoot the portal a bit away from it and try again
					var oldposition = Position;
					Position += cornerFits[0].BumpDirection;
					DebugOverlay.Line( Position, oldposition, Color.Magenta, 2, true );
					return FitPortal( recursionamount + 1 );
				}

				Position += (cornerFits[0].BumpDirection + cornerFits[1].BumpDirection + cornerFits[2].BumpDirection) * BumpDistance * 6;
				return FitPortal( recursionamount + 1 );
			default:
				if ( cornerFits[0].trace.Entity is GenericColliderEnt portal && portal.Tags.Has( "portalface" ) && cornerFits[0].IsPortalSameWall( Rotation.Forward ) )
				{
					if ( (Position - portal.Position).Dot( portal.Rotation.Right ) > 0.9f )
					{
						Position += portal.Rotation.Right * BumpDistance * 4;
						return FitPortal( recursionamount + 1 );
					}
					else
					{
						Position += portal.Rotation.Left * BumpDistance * 4;
						return FitPortal( recursionamount + 1 );
					}
				}
				else
					foreach ( var item in cornerFits )
					{
						Position += (item.BumpDirection * BumpDistance);
					}
				return FitPortal( recursionamount + 1 );

		}
	}
	private bool TracePortalCorners( Vector3 PortalCenter, Vector3 ToCorner, Vector3 Normal, out CornerFitData TraceResult )
	{
		TraceResult = new();
		var portalcenter = PortalCenter + Normal * 2f;
		var MiddleToCornerTrace = PortalTrace.FromTo( portalcenter, portalcenter + ToCorner ).Run();
		if ( DebugFitting )
		{
			if ( !MiddleToCornerTrace.Hit )
				DebugOverlay.Line( portalcenter, portalcenter + ToCorner, Color.Green, 2f, true );
			else
				DebugOverlay.Line( portalcenter, portalcenter + ToCorner, Color.Red, 2f, true );
		}
		if ( MiddleToCornerTrace.Hit )
		{
			TraceResult = new()
			{
				trace = MiddleToCornerTrace,
				BumpDirection = (-MiddleToCornerTrace.Direction).Normal,
				BumpFraction = MiddleToCornerTrace.Fraction / 2,
				hitsomething = true
			};
			return false;
		}
		var CornerToWallTrace = PortalTrace.FromTo( portalcenter + ToCorner, portalcenter + ToCorner - Normal * 3 ).Run();
		if ( DebugFitting )
		{
			if ( CornerToWallTrace.Hit )
				DebugOverlay.Line( portalcenter + ToCorner, portalcenter + ToCorner - Normal * 3, Color.Green, 2f, true );
			else
				DebugOverlay.Line( portalcenter + ToCorner, portalcenter + ToCorner - Normal * 3, Color.Red, 2f, true );
		}
		if ( CornerToWallTrace.Hit )
		{
			return true;
		}

		var CornerToWallToCenterTrace = PortalTrace.FromTo( CornerToWallTrace.EndPosition, portalcenter - Normal * 3 ).Run();
		if ( DebugFitting )
		{
			if ( CornerToWallTrace.Hit )
				DebugOverlay.Line( CornerToWallTrace.EndPosition, CornerToWallToCenterTrace.EndPosition, Color.Green, 2f, true );
			else
				DebugOverlay.Line( CornerToWallTrace.EndPosition, CornerToWallToCenterTrace.EndPosition, Color.Red, 2f, true );
		}

		if ( CornerToWallToCenterTrace.Hit || CornerToWallToCenterTrace.Fraction == 1 )
		{
			TraceResult = new()
			{
				trace = CornerToWallTrace,
				BumpDirection = (CornerToWallToCenterTrace.Direction).Normal,
				BumpFraction = (1 - CornerToWallToCenterTrace.Fraction) / 2,
				hitsomething = true,
				IsBackTrace = true
			};
			DebugOverlay.Line( Position, Position + TraceResult.BumpDirection, Color.Gray, 4f, true );
			return false;
		}
		return true;
	}

	bool CheckIfPointIsinPortal( Vector2 point )
	{
		var halfwidth = Width + BumpDistance / 3;
		var halfheight = Height + BumpDistance / 3;
		return (point.x * point.x) / (halfwidth * halfwidth) + (point.y * point.y) / (halfheight * halfheight) <= 1;
	}
}
