public static class BBoxExtensions
{
	public static bool Contains( this BBox box, Vector3 pos )
	{
		return pos.x >= box.Mins.x &&
			pos.y >= box.Mins.y &&
			pos.z >= box.Mins.z &&
			pos.x < box.Maxs.x &&
			pos.y < box.Maxs.y &&
			pos.z < box.Maxs.z;
	}
}
