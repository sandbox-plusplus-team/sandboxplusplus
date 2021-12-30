using Sandbox;
using System;

[Library( "ent_chair", Title = "Chair", Spawnable = true )]
public partial class ChairEntity : Prop, IUse
{
	private TimeSince timeSinceDriverLeft;

	[Net] public bool Grounded { get; private set; }

	private struct InputState
	{
		public float throttle;
		public float turning;
		public float breaking;
		public float tilt;
		public float roll;

		public void Reset()
		{
			throttle = 0;
			turning = 0;
			breaking = 0;
			tilt = 0;
			roll = 0;
		}
	}

	private InputState currentInput;

	[Net] public Player Driver { get; private set; }

	public override void Spawn()
	{
		base.Spawn();

		var modelName = "models/citizen_props/chair01.vmdl";

		SetModel( modelName );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
		SetInteractsExclude( CollisionLayer.Player );
		EnableSelfCollisions = false;

		var trigger = new ModelEntity
		{
			Parent = this,
			Position = Position,
			Rotation = Rotation,
			EnableTouch = true,
			CollisionGroup = CollisionGroup.Trigger,
			Transmit = TransmitType.Never,
			EnableSelfCollisions = false,
		};

		trigger.SetModel( modelName );
		trigger.SetupPhysicsFromModel( PhysicsMotionType.Keyframed, false );
	}


	protected override void OnDestroy()
	{
		base.OnDestroy();

		if ( Driver is SandboxPlayer player )
		{
			RemoveDriver( player );
		}
	}

	public void ResetInput()
	{
		currentInput.Reset();
	}

	[Event.Tick.Server]
	protected void Tick()
	{
		if ( Driver is SandboxPlayer player )
		{
			if ( player.LifeState != LifeState.Alive || player.Vehicle != this )
			{
				RemoveDriver( player );
			}
		}
	}

	public override void Simulate( Client owner )
	{
		if ( owner == null ) return;
		if ( !IsServer ) return;

		using ( Prediction.Off() )
		{
			currentInput.Reset();

			if ( Input.Pressed( InputButton.Use ) )
			{
				if ( owner.Pawn is SandboxPlayer player && !player.IsUseDisabled() )
				{
					RemoveDriver( player );

					return;
				}
			}

		}
	}

	private void RemoveDriver( SandboxPlayer player )
	{
		Driver = null;
		timeSinceDriverLeft = 0;

		ResetInput();

		if ( !player.IsValid() )
			return;

		player.Vehicle = null;
		player.VehicleController = null;
		player.VehicleAnimator = null;
		player.VehicleCamera = null;
		player.Parent = null;

		player.MainCamera = new FirstPersonCamera();

		if ( player.PhysicsBody.IsValid() )
		{
			player.PhysicsBody.Enabled = true;
			player.PhysicsBody.Position = player.Position;
		}
	}

	public bool OnUse( Entity user )
	{
		if ( user is SandboxPlayer player && player.Vehicle == null && timeSinceDriverLeft > 1.0f )
		{
			player.Vehicle = this;
			player.VehicleController = new ChairController();
			player.VehicleAnimator = new ChairAnimator();
			player.VehicleCamera = new ChairCamera();
			player.Parent = this;
			player.LocalPosition = Vector3.Up * 5;
			player.LocalRotation = Rotation.Identity;
			player.LocalScale = 1;
			player.PhysicsBody.Enabled = false;

			Driver = player;
		}

		return false;
	}

	public bool IsUsable( Entity user )
	{
		return Driver == null;
	}

}
