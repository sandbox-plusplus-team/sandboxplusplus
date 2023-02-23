using Sandbox;
using System;

public class GenericColliderEnt : BasePhysics
{
	readonly Vector3 max;
	readonly Vector3 min;

	public BBox BBox => new( Transform.PointToWorld( min ), Transform.PointToWorld( max ) );

	public Action<Entity> OnStartTouch;
	public GenericColliderEnt( Vector3 min, Vector3 max ) : base()
	{
		this.min = min;
		this.max = max;
		Tags.Add( "solid" );
		Transmit = TransmitType.Always;

	}

	public GenericColliderEnt( BBox boundingbox ) : base()
	{
		this.min = boundingbox.Mins;
		this.max = boundingbox.Maxs;
		Tags.Add( "solid" );
		Transmit = TransmitType.Always;

	}

	public GenericColliderEnt()
	{
	}

	public override void Spawn()
	{
		PhysicsClear();
		SpawnAsync();
	}
	public async void SpawnAsync()
	{
		await GameTask.NextPhysicsFrame();
		SetupPhysicsFromOBB( PhysicsMotionType.Keyframed, min, max );

	}

	public override void TakeDamage( DamageInfo info )
	{
	}

	public bool PointInside( Vector3 point )
	{
		return CollisionBounds.Contains( Transform.PointToLocal( point ) );
	}

	public override void StartTouch( Entity other )
	{
		//Log.Error( "Entered bounds!" );
		base.StartTouch( other );

		OnStartTouch?.Invoke( other );
	}

	public override void Touch( Entity other )
	{
		base.Touch( other );

		OnStartTouch?.Invoke( other );
	}
}
