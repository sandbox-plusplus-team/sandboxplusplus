using Sandbox;

[Library( "weapon_pipeshotgun", Title = "Rust Waterpipe Shotgun", Spawnable = true )]
partial class PipeShotgun : Weapon
{
	public override string ViewModelPath => "weapons/rust_shotgun/v_rust_shotgun.vmdl";
	public override float PrimaryRate => 1;
	public override float ReloadTime => 0.5f;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_shotgun/rust_shotgun.vmdl" );
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
		PlaySound( "rust_pumpshotgun.shoot" );

		//
		// Shoot the bullets
		//
		ShootBullets( 10, 0.1f, 10.0f, 9.0f, 3.0f );
	}

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );
		Particles.Create( "particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point" );

		ViewModelEntity?.SetAnimBool( "fire", true );

		if ( IsLocalPawn )
		{
			new Sandbox.ScreenShake.Perlin( 1.0f, 1.5f, 2.0f );
		}

		CrosshairPanel?.CreateEvent( "fire" );
	}

	[ClientRpc]
	protected virtual void DoubleShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );

		ViewModelEntity?.SetAnimBool( "fire_double", true );
		CrosshairPanel?.CreateEvent( "fire" );

		if ( IsLocalPawn )
		{
			new Sandbox.ScreenShake.Perlin( 3.0f, 3.0f, 3.0f );
		}
	}

	public override void OnReloadFinish()
	{
		IsReloading = false;

		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		FinishReload();
	}

	[ClientRpc]
	protected virtual void FinishReload()
	{
		ViewModelEntity?.SetAnimBool( "reload_finished", true );
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 3 ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
	}
}
