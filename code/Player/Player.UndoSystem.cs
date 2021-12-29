using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.UI;

partial class SandboxPlayer
{
	public Dictionary<Entity, int> UndoDictionary = new Dictionary<Entity, int>();

	//Add entities to the undo list, so they can be removed later and up the index
	public void AddToUndo(Entity ent)
	{
		UndoDictionary.Add( ent, UndoDictionary.Count);
	}

	//On zoom key pressed, undo the last spawned entity (can be prop etc.)
	public void Undo(bool isCMDExecuted)
	{
		if ( UndoDictionary.Count <= 0 )
			return;
		
		//Checks if there is an entity in the dictionary (taken away by 1 because weird index)
		//and deletes it
		if ( UndoDictionary.ElementAt( UndoDictionary.Count - 1).Key is not null)
			UndoDictionary.ElementAt( UndoDictionary.Count - 1 ).Key.Delete();
		
		//Remove the key from the dictionary (NOT THE ENTITY)
		UndoDictionary.Remove( UndoDictionary.ElementAt( UndoDictionary.Count - 1 ).Key );

		if ( isCMDExecuted == true )
			return;
		else
			UndoMessage( $"{Client.Name} despawned an entity/prop" );

	}

	public void UndoMessage(string message)
	{
		//Say you undid something
		ClassicChatBox.AddInformation( To.Everyone, message, $"avatar:{Client.PlayerId}" );
	}
}

