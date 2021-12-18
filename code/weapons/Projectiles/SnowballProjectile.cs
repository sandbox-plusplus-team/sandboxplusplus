using System;
using Sandbox;

	[Library("sf_snowball_projectile", Spawnable = true )]
	[Hammer.Skip]
partial class SnowballProjctile : Prop
{
	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/snowball.vmdl" );
	}


	[Event.Tick.Server]
	public virtual void Tick()
	{
		if ( !IsServer )
			return;

		float Speed = 2000.0f;
		var velocity = Rotation.Forward * Speed;

		var start = Position;
		var end = start + velocity * Time.Delta;

		var tr = Trace.Ray( start, end )
			.UseHitboxes()
			//.HitLayer( CollisionLayer.Water, !InWater )
			.Ignore( Owner )
			.Ignore( this )
			.Size( 1.0f )
			.Run();

		if ( tr.Entity is SandboxPlayer player )
		{
			if(Owner is SandboxPlayer plOwner )
			{

			}

			int hitBone = player.GetHitboxBone( tr.HitboxIndex );

			//Head
			if(hitBone == 5)
				player.Health -= 50f;
			else
				player.Health -= 25f;

			if ( player.Health <= 0 )
			{
				player.LastAttacker = this;
				player.OnKilled();

			}
			Delete();
		}

		if ( tr.Hit )
		{
			tr.Surface.DoSnowImpact( tr );
			Delete();
		}
		else
			Position = end;
	}
}
