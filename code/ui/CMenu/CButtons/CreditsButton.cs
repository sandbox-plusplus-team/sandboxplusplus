using Sandbox;
using Sandbox.UI;

[Library( "cb_credits", Title = "Credits" )]
public class CBCredits : CButton
{
	public override string IconPath => "ui/cmenu/credits.png";

	Window current;
	public CBCredits() : base()
	{
		AddEventListener( "onclick", () =>
		{
			if ( current?.Parent == null )
			{
				current = Window.With( new Credits() )
					.WithSize( 450, 500 )
					.WithTitle( "Credits" )
					.WithResizable( true );

				CMenu.Current.AddChild( current );

				current.Center();

				Sound.FromScreen( "ui.popup.message.close" );
			}
			else current.Delete();
		} );
	}

	public override void OnDeleted()
	{
		base.OnDeleted();
		current?.Delete();
	}
}
