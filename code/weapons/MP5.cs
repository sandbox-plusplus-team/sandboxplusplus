using System.Linq;
using Sandbox;

[Library( "weapon_mp5", Title = "MP5", Spawnable = true )]
partial class MP5 : Weapon
{
	public override string ViewModelPath => "weapons/rust_smg/v_rust_smg.vmdl";

	public override float PrimaryRate => 8.0f;
	public override float SecondaryRate => 1.0f;
	public override float ReloadTime => 5.0f;
	
	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_smg/rust_smg.vmdl" );
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
		PlaySound( "rust_smg.shoot" );

		//
		// Shoot the bullets
		//
		ShootBullet( 0.1f, 0.3f, 3.0f, 0.3f );
	}

	public override void AttackSecondary()
	{

		TimeSincePrimaryAttack = -0.5f;
		TimeSinceSecondaryAttack = -0.5f;
		
		PlaySound( "rust_pumpshotgun.shoot" );
		ShootGrenade();
	}

	private void ShootGrenade()
	{
		if ( Host.IsClient )
			return;
		
		var grenade = new Prop
		{
			Position = Owner.EyePos + Owner.EyeRot.Forward * 50,
			Rotation = Owner.EyeRot,
			Scale = 0.25f
		};
		
		//TODO: Should be replaced with an actual grenade model
		grenade.SetModel( "models/rust_props/barrels/fuel_barrel.vmdl" );
		grenade.Velocity = Owner.EyeRot.Forward * 1000;

		grenade.ExplodeAsync( 1f );

	}
	
	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );
		Particles.Create( "particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point" );

		if ( Owner == Local.Pawn )
		{
			new Sandbox.ScreenShake.Perlin( 0.5f, 1.0f, 1.0f, 0.5f );
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
