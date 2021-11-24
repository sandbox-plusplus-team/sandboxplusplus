using Sandbox;
using System.Collections.Generic;

// class that represents a path taken from one point to another.
public class NavPath
{


	public Vector3 TargetPosition; // the goal point
	public List<Vector3> Points = new List<Vector3>(); // ordered list of points the hypothetical npc would need to walk to linearly to reach the goal

	public bool IsEmpty => Points.Count <= 1; // bool that always returns whether or not the point list is empty or only contains one point (which isn't really a path)
	// note: it looks like this was intended to be used instead of "Points.Count <= 1" but it isn't. (see below)

	// for optimized path updating >400 units away
	public float LastUpdate;

	public void Update( Vector3 from, Vector3 to ) // update the path. should be called every tick. <from> is the origin, usually the npc. <to> is the target.
	{
		float dist = (from - to).Length;
		bool needsBuild = false;
		// the path will only be rebuilt if the target position is updated by more than 5 units and, either close enough to be updated every tick, or the last update happened more than 1 second ago.
		if ( !TargetPosition.IsNearlyEqual( to, 5 ) && 
		     (dist < 400 || (Time.Now - LastUpdate) > 1))
		{
			LastUpdate = Time.Now;
			TargetPosition = to;
			needsBuild = true;
		}

		if ( needsBuild )
		{
			// snap the from and to points to the closest points on the navmesh.
			var from_fixed = NavMesh.GetClosestPoint( from );
			var tofixed = NavMesh.GetClosestPoint( to );

			Points.Clear(); // clear the points list, we're about to refill it with an updated path
			NavMesh.GetClosestPoint( from ); // why is this called? it doesn't do anything. this method only exists to return a value, but it isn't assigned to a variable. likely a programming mistake.
			NavMesh.BuildPath( from_fixed.GetValueOrDefault(), tofixed.GetValueOrDefault(), Points ); // fill the points list with the points of the new path.
			//Points.Add( NavMesh.GetClosestPoint( to ) );
		}

		// don't do the rest if the points list is empty.
		if ( Points.Count <= 1 ) // (what was the point of declaring IsEmpty if we're just gonna do this?)
		{
			return;
		}

		var deltaToCurrent = from - Points[0];
		var deltaToNext = from - Points[1];
		var delta = Points[1] - Points[0];
		var deltaNormal = delta.Normal;

		// consider the current "start" point of the path completed if we're within 20 units of the second point.
		if ( deltaToNext.WithZ( 0 ).Length < 20 )
		{
			Points.RemoveAt( 0 );	
			return;
		}

		// calculate if our current position is "in front" of the "start" point of the path, relative to the second point of the path.
		// if we are, consider it passed and remove it.
		if ( deltaToNext.Normal.Dot( deltaNormal ) >= 1.0f )
		{
			Points.RemoveAt( 0 );
		}
	}

	// get the distance between the point with the index of <point> and <from>. ignores vertical (Z) axis.
	public float Distance( int point, Vector3 from )
	{
		if ( Points.Count <= point ) return float.MaxValue; // just return a giant fuckin number if the index is out of range

		return Points[point].WithZ( from.z ).Distance( from );
	}

	// get the direction that something standing at <position> would have to look to face the next point (or the final point if there's only one)
	public Vector3 GetDirection( Vector3 position )
	{
		if ( Points.Count == 1 )
		{
			return (Points[0] - position).WithZ(0).Normal;
		}

		return (Points[1] - position).WithZ( 0 ).Normal; 
	}
}
