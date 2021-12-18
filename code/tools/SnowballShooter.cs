namespace Sandbox.Tools
{
	[Library( "tool_snowballgun", Title = "Snowball Shooter", Description = "daaaam that a lot of balls", Group = "fun" )]
	public class SnowballShooter : BaseTool
	{
		TimeSince timeSinceShoot;

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
			var snowball = new SnowballProjctile();
			snowball.Position = Owner.EyePos + (Owner.EyeRot.Forward * 35);
			snowball.Rotation = Owner.EyeRot;
			snowball.Owner = Owner;
			snowball.Velocity = Owner.EyeRot.Forward * 15;
		}
	}

}
