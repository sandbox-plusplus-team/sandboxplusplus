using Sandbox;

[Library( "weapon_hands", Title = "Hands", Spawnable = false )]
partial class Hands : Weapon 
{
    public override string ViewModelPath => "";

    public override void Spawn() 
    {
        base.Spawn();

        SetModel( "" );
    }

    public override void SimulateAnimator(PawnAnimator anim) 
    {
        anim.SetParam("holdtype", 0);
        anim.SetParam("aimat_weight", 1.0f);
    }
	public override void OnCarryDrop( Entity dropper )
	{
	}
}
