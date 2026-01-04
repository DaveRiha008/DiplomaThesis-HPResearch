using UnityEngine;

public class VecConvert
{
	public static Vector2 ToVec2(Vector3 vec3)
	{
		return new Vector2(vec3.x, vec3.y);
	}
	public static Vector3 ToVec3CustomZ(Vector3 vec3, float z)
	{
		return new Vector3(vec3.x, vec3.y, z);	
	}
	public static Vector3 ToVec3CustomZ(Vector2 vec2, float z)
	{
		return new Vector3(vec2.x, vec2.y, z);
	}
}