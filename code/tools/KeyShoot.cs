namespace Sandbox.Tools
{
	[Library( "tool_keygun", Title = "s&box key gen 2021", Description = "DEALS THAT WILL MAKE YOU [Drop your ballsack!!!]", Group = "fun" )]
	public class KeyGen: BaseTool
	{
		TimeSince timeSinceShoot;

		PreviewEntity previewModel;

		public override void Simulate()
		{
			if ( Host.IsServer )
			{
				if ( Input.Pressed( InputButton.Attack1 ) )
				{
					ShootKey();
				}

				if ( Input.Down( InputButton.Attack2 ) && timeSinceShoot > 0f )
				{
					timeSinceShoot = 0;
					ShootKey();
				}
			}
		}

		void ShootKey()
		{
			var ent = new KeyEntity()
			{
				Position = Owner.EyePos + Owner.EyeRot.Forward * 30,
				Rotation = Owner.EyeRot
			};

			ent.Velocity = Owner.EyeRot.Forward * 1000;
			
			var player = Owner as SandboxPlayer;
			player.AddToUndo( ent );
		}

		public override void CreatePreviews()
		{
			if (TryCreatePreview(ref previewModel, "models/sboxkey_preview.vmdl"))
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
