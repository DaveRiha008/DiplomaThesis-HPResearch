using UnityEngine;

public static class VecConvert
{
	public static Vector2 ToVec2(this Vector3 vec3)
	{
		return new Vector2(vec3.x, vec3.y);
	}
	public static Vector3 WithZ(this Vector3 vec3, float z)
	{
		return new Vector3(vec3.x, vec3.y, z);
	}
	public static Vector3 WithY(this Vector3 vec3, float y)
	{
		return new Vector3(vec3.x, y, vec3.z);
	}
	public static Vector3 WithZ(this Vector2 vec2, float z)
	{
		return new Vector3(vec2.x, vec2.y, z);
	}
}