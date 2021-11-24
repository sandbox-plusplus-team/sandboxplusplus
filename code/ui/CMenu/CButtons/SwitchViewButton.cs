using Sandbox;

[Library("cb_switch_view", Title = "Switch View")]
public class SwitchViewButton : CButton
{
	public override string IconPath => "ui/cmenu/cameratoggle.png";
	public SwitchViewButton() : base()
	{
		AddEventListener( "onclick", () => ConsoleSystem.Run( "switchview" ) );
	}
}
