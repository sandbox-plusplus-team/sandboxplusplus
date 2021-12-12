using Sandbox;

[Library("weapon_magnum", Title = ".357 Magnum", Spawnable = true)]
partial class Magnum : Weapon
{
	public override string ViewModelPath => "models/weapons/v_357.vmdl";

	public override float PrimaryRate => 1.35f;

	public TimeSince TimeSinceDischarge { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/weapons/w_357.vmdl" );
	}

	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack() && Input.Pressed(InputButton.Attack1);
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		(Owner as AnimEntity)?.SetAnimBool( "b_attack", true );

		ShootEffects();
		PlaySound( "rust_pistol.shoot" );
		ShootBullet( 0.05f, 1.5f, 11.0f, 3.0f );
	}
	private void Discharge()
	{
		if (TimeSinceDischarge < 0.5f)
			return;

		TimeSinceDischarge = 3;

		var muzzle = GetAttachment("muzzle") ?? default;
		var pos = muzzle.Position;
		var rot = muzzle.Rotation;

		ShootEffects();
		PlaySound("rust_pistol.shoot");
		ShootBullet(pos, rot.Forward, 0.05f, 1.5f, 9.0f, 3.0f);

		ApplyAbsoluteImpulse(rot.Backward * 200.0f);
	}

	protected override void OnPhysicsCollision(CollisionEventData eventData)
	{
		if (eventData.Speed > 500.0f)
		{
			Discharge();
		}
	}

	public override void SimulateAnimator(PawnAnimator anim)
	{
		anim.SetParam("holdtype", 1);
		anim.SetParam("aimat_weight", 1.0f);
		anim.SetParam("holdtype_handedness", 0);
	}
}
