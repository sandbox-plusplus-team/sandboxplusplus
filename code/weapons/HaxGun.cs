using System;
using Sandbox;

[Library( "weapon_haxgun", Title = "HaxGun", Spawnable = true )]
partial class Haxgun : Weapon
{
	public override string ViewModelPath => "models/weapons/v_shooter_pistol.vmdl";

	public override float PrimaryRate => 15.0f;
	public override float SecondaryRate => 15.0f;
	public override float ReloadTime => 5.0f;
	Random rnd = new Random();

	public override void Spawn()
	{
		base.Spawn();
		SetModel( "models/weapons/shooter/w_pistol.vmdl" );
	}

	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack() && Input.Pressed( InputButton.Attack1 );
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		(Owner as AnimEntity)?.SetAnimBool( "b_attack", true );

		ShootEffects();
		PlaySound( "haxgun.shoot.attack1" );

		if ( Host.IsServer )
		{
			var ent = new Prop
			{
				Position = Owner.EyePos + Owner.EyeRot.Forward * 50,
				Rotation = Owner.EyeRot
			};

			ent.SetModel( "models/sm_computer_02a_crt.vmdl" );
			ent.Velocity = Owner.EyeRot.Forward * 10000;
		}
		// ShootBullet( 0.05f, 1.5f, 9.0f, 3.0f );
	}

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		// Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );
		// Particles.Create( "particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point" );

		ViewModelEntity?.SetAnimBool( "fire", true );
		CrosshairPanel?.CreateEvent( "fire" );
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 1 );
		anim.SetParam( "aimat_weight", 1.0f );
	}
	public override void Reload()
	{

	}
}
