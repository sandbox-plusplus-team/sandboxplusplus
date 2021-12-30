﻿using Sandbox;
using System;

[Library( "ent_chair2", Title = "Chair 2", Spawnable = true )]
public partial class Chair2Entity : ChairEntity
{
	public override void Spawn()
	{
		base.Spawn();

		var modelName = "models/citizen_props/chair02.vmdl";

		SetModel( modelName );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
		SetInteractsExclude( CollisionLayer.Player );
		EnableSelfCollisions = false;

		var trigger = new ModelEntity
		{
			Parent = this,
			Position = Position,
			Rotation = Rotation,
			EnableTouch = true,
			CollisionGroup = CollisionGroup.Trigger,
			Transmit = TransmitType.Never,
			EnableSelfCollisions = false,
		};

		trigger.SetModel( modelName );
		trigger.SetupPhysicsFromModel( PhysicsMotionType.Keyframed, false );
	}
}