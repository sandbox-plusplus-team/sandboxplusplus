using Sandbox;
using System;
using System.Buffers;

// used to control NPCs to follow a path. not always directly on the path, because it might need to avoid other NPCs
public class NavSteer
{
	// the NavPath that the NavSteer uses to determine walk direction.
	protected NavPath Path { get; private set; }

	public NavSteer()
	{
		Path = new NavPath(); // init the NavPath
	}

	// called every tick. updates the direction the NPC needs to walk in
	public virtual void Tick( Vector3 currentPosition )
	{
		Path.Update( currentPosition, Target );

		Output.Finished = Path.IsEmpty;

		if ( Output.Finished )
		{
			Output.Direction = Vector3.Zero;
			return;
		}

		Output.Direction = Path.GetDirection( currentPosition );

		var avoid = GetAvoidance( currentPosition, 25 );
		if ( !avoid.IsNearlyZero() )
		{
			Output.Direction = (Output.Direction + avoid).Normal;
		}
	}

	// get the direction needed to avoid any NPCs in a certain radius.
	Vector3 GetAvoidance( Vector3 position, float radius )
	{
		var center = position + Output.Direction * radius * 0.5f;

		var objectRadius = 200.0f;
		Vector3 avoidance = default;

		foreach ( var ent in Physics.GetEntitiesInSphere( center, radius ) )
		{
			if ( ent is not BaseNpc ) continue;
			if ( ent.IsWorld ) continue;

			var delta = (position - ent.Position).WithZ( 0 );
			var closeness = delta.Length;
			if ( closeness < 0.001f ) continue;
			var thrust = ((objectRadius - closeness) / objectRadius).Clamp( 0, 1 );
			if ( thrust <= 0 ) continue;

			//avoidance += delta.Cross( Output.Direction ).Normal * thrust * 2.5f;
			avoidance += delta.Normal * thrust * thrust;
		}

		return avoidance;
	}

	public Vector3 Target { get; set; }

	public NavSteerOutput Output;


	public struct NavSteerOutput
	{
		public bool Finished;
		public Vector3 Direction; // unit vector representing the direction to walk
	}
}
