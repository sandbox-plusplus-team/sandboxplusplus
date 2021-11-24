using Sandbox;
using System;

[Library( "ent_goldenglizzy", Title = "Golden Glizzy", Spawnable = true )]
public partial class GoldenGlizzyEntity : Prop, IUse
{
	public float MaxSpeed { get; set; } = 1000.0f;
	public float SpeedMul { get; set; } = 1.2f;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/food/goldglizzy.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
	}

	public bool IsUsable( Entity user )
	{
		return true;
	}

	public bool OnUse( Entity user )
	{
		if ( user is Player player )
		{
			player.Health += 10000000;

			Delete();
		}

		return false;
	}
}
