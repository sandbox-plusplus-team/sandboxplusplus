namespace PortalS2;

public class CameraMode : EntityComponent
{
	public Vector3 Position
	{
		get
		{
			return Camera.Position;
		}
		set
		{
			Camera.Position = value;
		}
	}
	public Rotation Rotation
	{
		get
		{
			return Camera.Rotation;
		}
		set
		{
			Camera.Rotation = value;
		}
	}

	public float FieldOfView
	{
		get
		{
			return Camera.FieldOfView;
		}
		set
		{
			Camera.FieldOfView = value;
		}
	}

	public Entity Viewer
	{
		get
		{
			return Camera.FirstPersonViewer as Entity;
		}
		set
		{
			Camera.FirstPersonViewer = value;
		}
	}

	public float ZNear
	{
		get
		{
			return Camera.ZNear;
		}
		set
		{
			Camera.ZNear = value;
		}
	}

	public float ZFar
	{
		get
		{
			return Camera.ZFar;
		}
		set
		{
			Camera.ZFar = value;
		}
	}
	public virtual void Deactivated() { }
	public virtual void Activated() { }

	[Event.Client.PostCamera]
	public virtual void Build()
	{
		Update();
	}

	public virtual void BuildInput() { }

	public virtual void Update()
	{

	}
}
