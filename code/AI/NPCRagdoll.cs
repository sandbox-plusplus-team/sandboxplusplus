using Sandbox;
using System;

public class ZombieRagdoll
{
	private const float fadeStartDelay = 5;
	private const float fadeTime = 2;
	public ModelEntity Ragdoll;
	private float timeSpawned;

	public ZombieRagdoll( ModelEntity entity )
	{
		Event.Register( this );

		Ragdoll = new ModelEntity( entity.GetModelName() );
		Ragdoll.Transform = entity.Transform;
		Ragdoll.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );

		Ragdoll.AngularVelocity = entity.AngularVelocity;
		Ragdoll.RenderColor = entity.RenderColor;
		Ragdoll.SetMaterialGroup( entity.GetMaterialGroup() );

		for ( var i = 0; i < Ragdoll.BoneCount; i++ )
		{
			Ragdoll.SetBoneTransform( i, entity.GetBoneTransform( i ) );
			var physBody = Ragdoll.GetBonePhysicsBody( i );
			if ( physBody != null )
			{
				physBody.Velocity = -entity.Velocity * 30;
			}
		}

		Ragdoll.SetInteractsAs( CollisionLayer.Debris );
		Ragdoll.SetInteractsWith( CollisionLayer.WORLD_GEOMETRY );
		Ragdoll.SetInteractsExclude( CollisionLayer.Player | CollisionLayer.Debris );

		foreach ( Entity child in entity.Children )
		{
			if ( child is ModelEntity m )
			{
				new ModelEntity( m.GetModelName(), Ragdoll );
			}
		}

		timeSpawned = Time.Now;
	}

	~ZombieRagdoll ()
	{
		Event.Unregister( this );
	}

	[Event.Tick.Server]
	public virtual void Tick()
	{

	}
}
