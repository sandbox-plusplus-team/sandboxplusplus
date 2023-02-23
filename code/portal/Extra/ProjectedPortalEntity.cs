using Sandbox;

namespace tardis.Portals;

public partial class ProjectedPortalEntity : ModelEntity
{
	[Net, Change] public ModelEntity Target { get; set; }

	[Net] public BasePortalEntity SourcePortal { get; set; }

	public override void Spawn()
	{
		base.Spawn();
		Transmit = TransmitType.Always;

		Tags.Add( DWTags.NotPortable );
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();
		UpdateTarget();

		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
	}

	private void UpdateTarget()
	{
		if ( Target == null )
			return;

		Model = Target.Model;
	}

	private void OnTargetChanged( ModelEntity oldTarget, ModelEntity newTarget )
	{
		UpdateTarget();
	}

	[Event.PreRender]
	public void FrameTick()
	{
		if ( Game.IsClient && SceneObject.IsValid() && SourcePortal.Partner.IsValid() )
		{
			//make sure that anything is actually sticking out of the portal
			var portal = SourcePortal.Partner;

			var transf = portal.ToLinkedTransform( Target.Transform );

			var plane = new Plane( portal.Position, -portal.Rotation.Forward );

			var horizontaldistance = portal.Transform.PointToLocal( transf.Position ).WithX( 0 ).y;
			var verticaldistance = portal.Transform.PointToLocal( transf.Position ).WithX( 0 ).z;


			SceneObject.RenderingEnabled = (plane.GetDistance( transf.Position ) < Target.CollisionBounds.Size.Length * 0.5f) && horizontaldistance < portal.Width * 0.25f && verticaldistance < portal.Height * 0.25f;

		}
	}


	[Event.Tick.Server]
	public void Tick()
	{
		if ( SourcePortal == null || Target == null )
		{
			Delete();
			return;
		}
		Transform = SourcePortal.ToLinkedTransform( Target.Transform );

		ResetInterpolation();


	}
}
