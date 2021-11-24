using Sandbox;
using System;

[Library( "ent_radio", Title = "Portal Radio", Spawnable = true )]
public partial class RadioEntity : Prop, IUse
{

	public bool Enable { get; set; } = false;
	private Sound RadioSnd;

	public override void Spawn()
	{
		base.Spawn();
		CheckSnd();
		SetModel("models/props/portal/radio_remade.vmdl");
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
	}

	public bool IsUsable( Entity user )
	{
		return true;
	}

	public bool OnUse( Entity user )
	{
		if ( user is Player player )
		{
			CheckSnd();
		}

		return false;
	}

	public void CheckSnd()
	{
		if(Enable==true){
			Enable=false;
			RadioSnd = base.PlaySound("radioloop");
		}else{
			Enable=true;
			RadioSnd.Stop();
		}
	}

	protected override void OnDestroy()
	{
		RadioSnd.Stop();
		base.OnDestroy();
	}
}
