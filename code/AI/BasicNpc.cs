using Sandbox;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

[Library("npc_basic", Title = "Terry Basic", Spawnable = true)]
public partial class BaseNpc : AnimEntity
{
	public float Speed { get; set; } // unit/s that this npc moves

	public NavSteer Steer;

	public BaseNpc ()
	{
		Steer = new NavSteer();
	}

	public override void Spawn()
	{
		base.Spawn();

		Health = 100;

		SetModel( "models/citizen/citizen.vmdl" );
		EyePos = Position + Vector3.Up * 64;
		CollisionGroup = CollisionGroup.Player;
		SetupPhysicsFromCapsule( PhysicsMotionType.Keyframed, Capsule.FromHeightAndRadius( 72, 8 ) ); // Keyframed motion just means the engine doesn't move the object at all. it simply lets other things bounce off of it and stuff.
		// "capsule" is just kind of a pill-shaped hitbox.

		EnableHitboxes = true;

		SetBodyGroup( 1, 0 );

		Speed = 300f;
	}

	public override void OnKilled()
	{
		LastAttacker.Client.AddInt("kills");

		var game = (SandboxGame)Game.Current;
		game.PlaySoundFromEntity("balloon_pop_cute", new ZombieRagdoll(this).Ragdoll);

		base.OnKilled();
	}

	Vector3 InputVelocity; // desired velocity. this value will have snappy movements, so it's better if we store this separately and update the actual Velocity to follow this with some dampening applied.

	Vector3 LookDir;

	[Event.Tick.Server]
	public virtual void Tick()
	{
		if ( this.LifeState != LifeState.Alive )
		{
			EnableDrawing = false;
			CollisionGroup = CollisionGroup.Never;
			return;
		}

		InputVelocity = 0; // reset InputVelocity so it can be updated

		if ( Steer != null )
		{
			Steer.Tick( Position );

			if ( !Steer.Output.Finished )
			{
				InputVelocity = Steer.Output.Direction.Normal;
				Velocity = Velocity.AddClamped( InputVelocity * Time.Delta * 500, Speed ); // speed of the npc is increased by 500ups per second until it reaches the peak speed of the npc (100-300).
			}
		}

		Move( Time.Delta );

		var walkVelocity = Velocity.WithZ( 0 ); // walk velocity is just the normal velocity given by the NavSteer minus the vertical axis.
		if ( walkVelocity.Length > 0.5f ) // only proceed if the walk velocity is actually meaningfully large
		{
			var turnSpeed = walkVelocity.Length.LerpInverse( 0, 100, true ); // changes the turn speed scale (which will be multiplied by 20/s below) based on how fast the npc will move.
                                                                    // 100 units/s will max out the turning rate (turn the correct direction in 1/20 of a second)
																	// purely for visual purposes to make the npc look where its going.
			var targetRotation = Rotation.LookAt( walkVelocity.Normal, Vector3.Up );
			Rotation = Rotation.Lerp( Rotation, targetRotation, turnSpeed * Time.Delta * 20.0f );
		}

		var animHelper = new CitizenAnimationHelper( this );

		// constantly lerp the lookdir to face the walking direction. creates a dampened movement effect here. purely for visual purposes.
		LookDir = Vector3.Lerp( LookDir, InputVelocity.WithZ( 0 ) * 1000, Time.Delta * 100.0f );
		// update the animations
		animHelper.WithLookAt( EyePos + LookDir );
		animHelper.WithVelocity( Velocity );
		animHelper.WithWishVelocity( InputVelocity );		
	}

	protected virtual void Move( float timeDelta )
	{
		var bbox = BBox.FromHeightAndRadius( 64, 4 );

		// define a MoveHelper for the hl2 style movement
		MoveHelper move = new( Position, Velocity );
		move.MaxStandableAngle = 50;
		move.Trace = move.Trace.Ignore( this ).Size( bbox ); // configure the MoveHelper's trace (basically its collision detection) to be a bounding box that ignores itself

		// only run this if the player should be moving
		if ( !Velocity.IsNearlyZero( 0.001f ) )
		{
			move.TryUnstuck(); // call this in case we're stuck, if we are then it will either jump to get unstuck or move around like a dumbass until it works

			move.TryMoveWithStep( timeDelta, 30 ); // increment the movement of the NPC by timeDelta (with a vertical tolerance of 30 units)
		}
		// trace towards the ground
		var tr = move.TraceDirection( Vector3.Down * 10.0f );

		// check if it's the floor
		if ( move.IsFloor( tr ) )
		{
			// if it is, we'll handle friction on the floor here

			GroundEntity = tr.Entity;

			if ( !tr.StartedSolid )
			{
				move.Position = tr.EndPos;
			}

			if ( InputVelocity.Length > 0 )
			{
				var movement = move.Velocity.Dot( InputVelocity.Normal );
				move.Velocity = move.Velocity - movement * InputVelocity.Normal;
				move.ApplyFriction( tr.Surface.Friction * 10.0f, timeDelta );
				move.Velocity += movement * InputVelocity.Normal;

			}
			else
			{
				move.ApplyFriction( tr.Surface.Friction * 10.0f, timeDelta );
			}
		}
		else
		{
			// if it isn't, apply world gravity to the velocity
			GroundEntity = null;
			move.Velocity += PhysicsWorld.Gravity * timeDelta;
		}

		// update the position and velocity of the NPC
		// note that while Position does affect where the NPC's model is located, Velocity has no effect on the NPC internally
		// due to its PhysicsMotionType being set to Keyframed.
		// we could actually just be storing it in a separate variable if we wanted, this is just a convenient way to store it.
		Position = move.Position;
		Velocity = move.Velocity;
	}
}
