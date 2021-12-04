namespace Sandbox.Tools
{
	[Library( "tool_melongun", Title = "Melon Gun", Description = "You will turn to dust.", Group = "fun" )]
	public class MelonGun : BaseTool
	{
		TimeSince timeSinceShoot;

		PreviewEntity previewModel;

		public override void Simulate()
		{
			if ( Host.IsServer )
			{
				if ( Input.Pressed( InputButton.Attack1 ) )
				{
					ShootMelon();
				}

				if ( Input.Down( InputButton.Attack2 ) && timeSinceShoot > 0.05f )
				{
					timeSinceShoot = 0;
					ShootMelon();
				}
			}
		}

		void ShootMelon()
		{
			var ent = new MelonEntity()
			{
				Position = Owner.EyePos + Owner.EyeRot.Forward * 150,
				Rotation = Owner.EyeRot
			};

			ent.Velocity = Owner.EyeRot.Forward * 1000;
			
			var player = Owner as SandboxPlayer;
			player.AddToUndo( ent );
		}

		public override void CreatePreviews()
		{
			if (TryCreatePreview(ref previewModel, "models/sbox_props/watermelon/watermelon.vmdl" ) )
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
