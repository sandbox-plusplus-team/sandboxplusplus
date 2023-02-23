using Sandbox;

[Spawnable]
public partial class npclooktest : AnimatedEntity,IUse
{
	public Vector3 EyePos;
	public Rotation EyeRot;
	public Entity ter;
	public override void Spawn()
	{
		
		SetModel( "models/tf/daleks/nsd_dalek.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Static );
		//EyePos = GetAttachment( "eyePos" ).Value.Position;
		//EyeRot = GetAttachment( "eyeRot" ).Value.Rotation;
	}

	[Event.Tick]
	public void tick()
	{
		if ( ter == null ) return;
		SetLookAt( ter.Position );
	}

	public void SetLookAt( Vector3 here )
	{
		//here = (here - Position) * Rotation.Inverse;
		Log.Info( here );
		//SetAnimLookAt( "aim_eyes", here );
		///SetAnimParameter( "aim_pitch", here.Pitch().Degrees() );
		//if (here.nor)
		//SetAnimParameter( "LRDir", here.x );
		//SetAnimParameter( "UDDir", here.z);
	}

	public bool OnUse( Entity user )
	{
		ter = user;
		return false;
	}

	public bool IsUsable( Entity user )
	{
		return true;
	}
}
