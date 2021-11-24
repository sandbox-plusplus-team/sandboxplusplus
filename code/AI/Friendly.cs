using Sandbox;
using System;
using System.Linq;

[Library("npc_friend", Title = "Terry Follower", Spawnable = true)]
public class Zombie : BaseNpc
{
	private static float lastAttack;
	public override void Spawn()
	{
		base.Spawn();

		Health = 10;
		Speed = 300;
		SetMaterialGroup( Rand.Int( 1, 5 ) );

		RandomClothes.ApplyRandomHeadAccessory( this );
	}

	public void DoDamage (Player entity, float damage)
	{
		entity.TakeDamage( DamageInfo.Generic( damage ).WithAttacker( this ).WithHitbox( 1 ) );
	}

	public override void Tick()
	{
		base.Tick();

		if ( IsClient ) return;

		Vector3 targetPos = new(); // the target position for the zombie to run towards

		// find a target. we're looking for the closest live player to the zombie.
		// TODO - maybe the zombie could have a targeting bias for players its already chasing?
		var players = Entity.All.OfType<Player>();
		Player closest = null;
		foreach ( var player in players )
		{
			if ( closest == null )
			{
				closest = player; // make sure we always set the closest player to /something/, to avoid it being null if a lone player dies.
				continue;
			}

			if ( player.LifeState != LifeState.Alive ) continue;

			var bestDist = (closest.Position - this.Position).Length;
			var thisDist = (player.Position - this.Position).Length;

			if ( thisDist < bestDist )
			{
				closest = player;
				continue;
			}
		}

		targetPos = closest.Position;
		var dist = (closest.Position - this.Position).Length;

		// zombies run fast until they get close to you
		if ( dist > 1000 )
		{
			Speed = 500;
		}
		else
		{
			Speed = 300;
		}

		var ignoreZDist = (targetPos.WithZ( 0 ) - this.Position.WithZ( 0 )).Length;
		var timeSinceSpawn = Time.Now - ( (SandboxPlayer)closest).LastSpawnTime;

		// once we've found a target position, update the navigation to bring the zombie there
		Steer.Target = targetPos;
	}

	public override void OnKilled()
	{
		LastAttacker.Client.AddInt( "kills" );

		var game = (SandboxGame)Game.Current;
		game.PlaySoundFromEntity("balloon_pop_cute", new ZombieRagdoll( this ).Ragdoll );

		base.OnKilled();
	}

	public override void TakeDamage(DamageInfo info)
	{
		base.TakeDamage( info );
		var attacker = info.Attacker as SandboxPlayer;
		if ( attacker == null ) return;

		//((DeathmatchGame)Game.Current)?.Hud?.ShowScoreIndicator( 10 );

		attacker.DidDamage( Position, info.Damage, Health.LerpInverse( 10, 0 ) );
	}
}
