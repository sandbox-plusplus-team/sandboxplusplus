using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Health : Panel
{
	public Label Label;

	public Health()
	{
		Label = Add.Label( "100", "value" );
	}

	public override void Tick()
	{
		var player = Game.LocalPawn;
		if ( player == null ) return;
		else 
		{
			Label.Text = $"{player.Health.CeilToInt()}";
		}
		
	}
}
