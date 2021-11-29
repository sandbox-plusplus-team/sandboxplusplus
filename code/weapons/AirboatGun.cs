using Sandbox;

[Library( "weapon_airboatgun", Title = "Airboat Gun", Spawnable = true )]
partial class AirboatGun : Weapon
{
	public override string ViewModelPath => "models/weapons/c_rust_toolgun.vmdl";

	public override float PrimaryRate => 12.0f;
	public override float SecondaryRate => 1.0f;
	public override float ReloadTime => 5.0f;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/weapons/toolgun.vmdl" );
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		(Owner as AnimEntity)?.SetAnimBool( "b_attack", true );

		//
		// Tell the clients to play the shoot effects
		//
		ShootEffects();
		PlaySound( "airboat_gun_lastshot1" );
		//
		// Shoot the bullets
		//
		ShootBullet( 0.0f, 25.5f, 25.0f, 3.0f );

		Owner.Velocity = Owner.EyeRot.Forward * -70;
	}

	public override void AttackSecondary()
	{
		// Grenade lob
	}

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		var Muzzle = Particles.Create( "particles/tool_hit.vpcf", EffectEntity, "muzzle" );
		Muzzle.Destroy( false );

		ViewModelEntity?.SetAnimBool( "fire", true );
		CrosshairPanel?.CreateEvent( "fire" );
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 1 ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
	}

}

