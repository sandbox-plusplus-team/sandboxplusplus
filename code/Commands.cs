using Sandbox;

partial class SandboxPlayer
{
	[ServerCmd( "sbox_cleanup" )]
	public static void CleanUp()
	{
		var caller = ConsoleSystem.Caller.Pawn;

		if ( caller is SandboxPlayer player )
		{
			if ( player.UndoDictionary.Count <= 0 )
				return;

			while (player.UndoDictionary.Count > 0)
			{
				player.Undo();
			}	
		}			
	}

	[ServerCmd("sbox_admin_cleanup")]
	public static void AdminCleanUp()
	{
		var caller = ConsoleSystem.Caller;

		if ( !caller.IsListenServerHost )
			return;

		foreach(var client in Client.All)
		{
			if ( client.Pawn is SandboxPlayer player)
			{
				if ( player.UndoDictionary.Count <= 0 )
					break;

				while ( player.UndoDictionary.Count > 0 )
					player.Undo();
			}
		}
			
	}
}
