namespace Sandbox.Tools
{
	[Library( "tool_boxgun", Title = "Car Shooter", Description = "It's a hit and run", Group = "fun" )]
	public class CarShooter : BaseTool
	{
		TimeSince timeSinceShoot;

		PreviewEntity previewModel;

		public override void Simulate()
		{
			if ( Host.IsServer )
			{
				if ( Input.Pressed( InputButton.Attack1 ) )
				{
					ShootCar();
				}

				if ( Input.Down( InputButton.Attack2 ) && timeSinceShoot > 0.05f )
				{
					timeSinceShoot = 0;
					ShootCar();
				}
            }
        }

        void ShootCar()
        {
            var ent = new CarEntity
            {
                Position = Owner.EyePos + Owner.EyeRot.Forward * 150,
                Rotation = Owner.EyeRot
            };

            ent.Velocity = Owner.EyeRot.Forward * 1000;

            var player = Owner as SandboxPlayer;
            player.AddToUndo(ent);
        }

		public override void CreatePreviews()
		{
			if (TryCreatePreview(ref previewModel, "models/kart_preview.vmdl"))
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
