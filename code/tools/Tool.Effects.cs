using Sandbox;

public partial class Tool
{
	[ClientRpc]
	public void CreateHitEffects( Vector3 hitPos )
	{
		var particle = Particles.Create("particles/physgun_freeze.vpcf", hitPos );
		particle.SetPosition( 0, hitPos );
		particle.Destroy( false );

		ViewModelEntity?.SetAnimBool("fire", true);

		PlaySound("toolgun_hit");
	}
}
