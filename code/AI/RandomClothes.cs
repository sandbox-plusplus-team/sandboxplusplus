using System.Collections.Generic;
using Sandbox;

public static class RandomClothes
{
	static List<string> HeadAccessories = new ()
	{
		// hats
		"models/citizen_clothes/hat/hat.tophat.vmdl",
		"models/sboxhardhatbase.vmdl",
		"models/spame/dealmaker.vmdl",

		// hair
		"models/citizen_clothes/hair/hair_looseblonde/hair_looseblonde.vmdl",
	};

	//static List<string> Clothes = new()
	//{
		// tops
	//	"models/citizen_clothes/jacket/suitjacket/suitjacket.vmdl",
	//	"models/citizen_clothes/jacket/labcoat.vmdl",
	//	"models/citizen_clothes/jacket/jacket.red.vmdl",
	//	"models/citizen_clothes/jacket/jacket_heavy.vmdl",
		// bottoms
	//	"models/citizen_clothes/trousers/smarttrousers/smarttrousers.vmdl",
		// feet
	//	"models/citizen_clothes/shoes/smartshoes/smartshoes.vmdl",
	//};

	private static string GetRandomFromList( List<string> list )
	{
		return list[Rand.Int( 0, list.Count - 1 )];
	}
	public static string GetRandomHeadAccessory()
	{
		return RandomClothes.GetRandomFromList(HeadAccessories);
	}

	//public static string GetRandomClothes()
	//{
	//	return RandomClothes.GetRandomFromList( Clothes );
	//}

	public static ModelEntity ApplyRandomHeadAccessory( Entity entity )
	{
		var model = new ModelEntity( RandomClothes.GetRandomHeadAccessory(), entity );
		return model;
	}

	//public static ModelEntity ApplyRandomClothes( Entity entity )
	//{
	//	var model = new ModelEntity( RandomClothes.GetRandomClothes(), entity );
	//	return model;
	//}
}
