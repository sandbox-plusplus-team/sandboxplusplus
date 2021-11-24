using System.Collections.Generic;
using Sandbox;

public static class RandomClothes
{
	static List<string> HeadAccessories = new ()
	{
		// hats
		"models/citizen_clothes/hat/hat.tophat.vmdl",
		"models/citizen_clothes/hat/hat_beret.black.vmdl",
		"models/citizen_clothes/hat/hat_beret.red.vmdl",
		"models/citizen_clothes/hat/hat_cap.vmdl",
		"models/sboxhardhatbase.vmdl",
		"models/citizen_clothes/hat/hat_leathercap.vmdl",
		"models/citizen_clothes/hat/hat_leathercapnobadge.vmdl",
		"models/citizen_clothes/hat/hat_securityhelmet.vmdl",
		"models/citizen_clothes/hat/hat_securityhelmetnostrap.vmdl",
		"models/citizen_clothes/hat/hat_service.vmdl",
		"models/citizen_clothes/hat/hat_uniform.police.vmdl",
		"models/citizen_clothes/hat/hat_woolly.vmdl",
		"models/citizen_clothes/hat/hat_woollybobble.vmdl",

		// hair
		"models/citizen_clothes/hair/hair_femalebun.black.vmdl",
		"models/citizen_clothes/hair/hair_femalebun.blonde.vmdl",
		"models/citizen_clothes/hair/hair_femalebun.brown.vmdl",
		"models/citizen_clothes/hair/hair_femalebun.red.vmdl",
		"models/citizen_clothes/hair/hair_looseblonde/hair_looseblonde.vmdl"
	};

	private static string GetRandomFromList( List<string> list )
	{
		return list[Rand.Int( 0, list.Count - 1 )];
	}
	public static string GetRandomHeadAccessory()
	{
		return RandomClothes.GetRandomFromList(HeadAccessories);
	}

	public static ModelEntity ApplyRandomHeadAccessory( Entity entity )
	{
		var model = new ModelEntity( RandomClothes.GetRandomHeadAccessory(), entity );
		return model;
	}
}
