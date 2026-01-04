using UnityEngine;

public class VecConvert
{
	public static Vector2 ToVec2(Vector3 vec3)
	{
		return new Vector2(vec3.x, vec3.y);
	}
}