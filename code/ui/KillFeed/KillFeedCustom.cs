
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;

namespace Sandbox.UI
{
	public partial class KillFeedCustom : Sandbox.UI.KillFeed
	{
		public KillFeedCustom()
		{
			StyleSheet.Load( "/ui/killfeed/KillFeedCustom.scss" );
		}

		public override Panel AddEntry( long lsteamid, string left, long rsteamid, string right, string method )
		{
			var e = Current.AddChild<KillFeedCustomEntry>();

			e.Left.Text = left;
			e.Left.SetClass( "me", lsteamid == (Local.Client?.PlayerId) );

			e.Method.Text = method;

			e.Right.Text = right;
			e.Right.SetClass( "me", rsteamid == (Local.Client?.PlayerId) );

			return e;
		}
	}
}
