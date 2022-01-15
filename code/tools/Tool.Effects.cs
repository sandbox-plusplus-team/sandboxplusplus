using Sandbox;

public partial class Tool
{
	[ClientRpc]
	public void CreateHitEffects( Vector3 hitPos )
	{
		var particle = Particles.Create( "particles/entity/player_teleport_sparks.vpcf", hitPos );
		particle.SetPosition( 0, hitPos );
		particle.Destroy( false );

		var beam = Particles.Create( "particles/tool_tracer.vpcf", hitPos );
		beam.SetEntityAttachment( 0, EffectEntity, "muzzle", true );
		beam.SetPosition( 1, hitPos );

		var particle2 = Particles.Create( "particles/tool_hit.vpcf", EffectEntity, "muzzle" ); ;
		particle2.Destroy( false );

		ViewModelEntity?.SetAnimBool("fire", true);

		PlaySound("toolgun_hit");
	}
}
