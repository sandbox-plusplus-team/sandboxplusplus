using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.UI.Construct;
using Sandbox.UI;
using Sandbox;

public partial class Notifications : Panel
{
	public static Notifications Instance;

	public Notifications()
	{
		StyleSheet.Load( "/ui/Notifications.scss" );
		Instance = this;
	}

	[ClientRpc]
	public static void AddNotification(string icon, string msg, int length )
	{
		Panel notif = Instance.Add.Panel( "notif" );
		notif.Add.Label( icon, "text emote" );
		notif.Add.Label( msg, "text msg" );
		_ = DeleteAfter( notif, length );
	}

	private static async Task DeleteAfter(Panel pnl, int length)
	{
		await Instance.Task.DelaySeconds( length );
		pnl.Delete();
	}
}
