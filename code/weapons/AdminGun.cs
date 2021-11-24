using Sandbox;

[Library( "weapon_admingun", Title = "Sexyness", Spawnable = true )]
partial class Sexyness : Weapon
{
	public override string ViewModelPath => "models/weapons/v_pist_weagon.vmdl";

	public override float PrimaryRate => 1500.0f;
	public override float SecondaryRate => 1.0f;
	public override float ReloadTime => 5.0f;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/weapons/w_pist_weagon.vmdl" );
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
		PlaySound( "admin_gun.shoot" );

		//
		// Shoot the bullets
		//
		ShootBullet( 0.0f, 100.5f, 100.0f, 3.0f );

		Sound.FromScreen("ui.button.press");
	}

	public override void AttackSecondary()
	{
		// Grenade lob
	}

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );
		Particles.Create( "particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point" );

		if ( Owner == Local.Pawn )
		{
			new Sandbox.ScreenShake.Perlin( 0.5f, 4.0f, 1.0f, 0.5f );
		}

		ViewModelEntity?.SetAnimBool( "fire", true );
		CrosshairPanel?.CreateEvent( "fire" );
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 2 ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
	}

}

