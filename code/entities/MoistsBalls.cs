using Sandbox;
using System;

[Library( "ent_balls", Title = "Moist Fellows Balls", Spawnable = true )]
public partial class MoistsBalls : Prop, IUse
{
	public float MaxSpeed { get; set; } = 1000.0f;
	public float SpeedMul { get; set; } = 1.2f;

	public override void Spawn()
	{	
		base.Spawn();

		SetModel("models/moistsballs.vmdl");
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
	}

	public bool IsUsable( Entity user )
	{
		return true;
	}

	public bool OnUse( Entity user )
	{
		return false;
	}
}
