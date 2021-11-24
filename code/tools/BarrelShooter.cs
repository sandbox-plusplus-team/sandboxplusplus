namespace Sandbox.Tools
{
	[Library( "tool_barrelgun", Title = "Barrel Shooter", Description = "die", Group = "fun" )]
	public class BarrelShooter : BaseTool
	{
		TimeSince timeSinceShoot;

		PreviewEntity previewModel;

		public override void Simulate()
		{
			if ( Host.IsServer )
			{
				if ( Input.Pressed( InputButton.Attack1 ) )
				{
					ShootBarrel();
				}

				if ( Input.Down( InputButton.Attack2 ) && timeSinceShoot > 0.05f )
				{
					timeSinceShoot = 0;
					ShootBarrel();
				}
			}
		}

		void ShootBarrel()
		{
			var ent = new Prop
			{
				Position = Owner.EyePos + Owner.EyeRot.Forward * 50,
				Rotation = Owner.EyeRot
			};

			ent.SetModel( "models/rust_props/barrels/fuel_barrel.vmdl" );
			ent.Velocity = Owner.EyeRot.Forward * 1000;
			
			var player = Owner as SandboxPlayer;
			player.AddToUndo( ent );
			
		}

		public override void CreatePreviews()
		{
			if (TryCreatePreview(ref previewModel, "models/rust_props/barrels/fuel_barrel.vmdl"))
			{
				previewModel.RelativeToNormal = false;
			}
		}

		protected override bool IsPreviewTraceValid(TraceResult tr)
		{
			if (!base.IsPreviewTraceValid(tr))
				return false;

			if (tr.Entity is BalloonEntity)
				return false;

			return true;
		}
	}

}
