using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

	public class Credits : Panel
	{

	public Credits()
		{
			StyleSheet.Load( "/ui/Credits.scss" );

			Add.Label("s&box++ Credits", "header" );

		    var btn = AddChild<CreditsContent>();
	}

		public override void Tick()
		{
			SetClass( "open", Input.Down( InputButton.View ) );
		}
	}

/// <summary>
/// Controls for developers. (i fucking love intellisense)
/// </summary>
public class CreditsContent : Panel
{
	public CreditsContent()
	{
		Add.Label( "WIP", "section");
	}
}
