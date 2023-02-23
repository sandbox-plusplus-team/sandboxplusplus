using Sandbox.UI;
using Sandbox.UI.Construct;

public class MyWorldPanel : WorldPanel
{
	
	public string Message { get; set; }
	public MyWorldPanel()
	{
		var w = 3;
		var h = 3;
		PanelBounds = new Rect( -(w / 2), -(h / 2), w, h );
		StyleSheet.Load( "/ui/partWorldPanel.scss" );
		SceneObject.Flags.OverlayLayer = true;
		SceneObject.Flags.ViewModelLayer = true;
	}

	[Event.Client.Frame]
	public void FrameUpdate()
	{
		
		//SceneObject.Flags.ViewModelLayer = true;
		//SceneObject.Flags.
		//tx.Position += Vector3.Up * 10.0f;
		Rotation = Rotation.LookAt( -Camera.Rotation.Forward );

		
	}
	
	public override void Tick()
	{
		base.Tick();

		var w = 300;
		var h = 90;
		PanelBounds = new Rect( -(w / 2), -(h / 2), w, h );
	}
	public void SetMessage( string message )
	{
		Add.Label( message );
	}




}




