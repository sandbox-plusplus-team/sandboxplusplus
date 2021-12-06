using System.Numerics;
using System.Runtime.InteropServices;
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Threading.Tasks;

[Library( "weapon_DBShotgun", Title = "Double Barrel Shotgun", Spawnable = true )]
partial class DBShotgun : BaseDmWeapon
{ 
	// TODO nerf dbshotgun but good
	public override string ViewModelPath => "models/weapons/dbshotgun/v_doublebarrel.vmdl";
	public override float PrimaryRate => 1;
	public override float SecondaryRate => 1;
	public override int ClipSize => 4;
	public override float ReloadTime => 0.5f;
	public override int Bucket => 2;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/weapons/dbshotgun/doublebarrel.vmdl" );  

		AmmoClip = 4;
	}

	public override void AttackPrimary()
	{
		// TODO:player pushback
		// applyForce(new Vector3(0, 30, 0));
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;
		

		(Owner as AnimEntity).SetAnimBool( "b_attack", true );

		//
		// Tell the clients to play the shoot effects
		//
		ShootEffects();
		PlaySound("dbshotgun");

		//
		// Shoot the bullets
		//
		for ( int i = 0; i < 10; i++ )
		{
			ShootBullet( 0.15f, 0.3f, 16.0f, 6.0f );
		}

		Owner.Velocity = Owner.EyeRot.Forward * -700;
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
			new Sandbox.ScreenShake.Perlin(2.0f, 3.0f, 4.0f);
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
			new Sandbox.ScreenShake.Perlin(3.0f, 3.0f, 3.0f);
		}
	}

	public override void OnReloadFinish()
	{
		IsReloading = false;

		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if ( AmmoClip >= ClipSize )
			return;

		if ( Owner is SandboxPlayer player )
		{

		}
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
