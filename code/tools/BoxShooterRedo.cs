namespace Sandbox.Tools
{
	[Library( "tool_boxshooter", Title = "Box Shooter", Description = "Shoot boxes", Group = "fun" )]
	public class BoxShooter : BaseTool
	{
		TimeSince timeSinceShoot;

		PreviewEntity previewModel;

		public override void Simulate()
		{
			if ( Host.IsServer )
			{
				if ( Input.Pressed( InputButton.Attack1 ) )
				{
					ShootBox();
				}

				if ( Input.Down( InputButton.Attack2 ) && timeSinceShoot > 0.05f )
				{
					timeSinceShoot = 0;
					ShootBox();
				}
			}
		}

		void ShootBox()
		{
			var ent = new Prop
			{
				Position = Owner.EyePos + Owner.EyeRot.Forward * 50,
				Rotation = Owner.EyeRot
			};

			ent.SetModel( "models/citizen_props/crate01.vmdl" );
			ent.Velocity = Owner.EyeRot.Forward * 1000;
		}


		public override void CreatePreviews()
		{
			if (TryCreatePreview(ref previewModel, "models/citizen_props/early/crate_early.vmdl"))
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
