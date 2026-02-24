using System;
using System.Collections;
using UnityEngine;


public static class MonoBehaviourExtensions
{
	public static void CallWithDelay(this MonoBehaviour mb, Action f, float delay)
	{
		mb.StartCoroutine(InvokeRoutine(f, delay));
	}

	private static IEnumerator InvokeRoutine(System.Action f, float delay)
	{
		yield return new WaitForSeconds(delay);
		f();
	}
}


public static class ColorExtensions
{
	public static Color WithAlpha(this Color color, float alpha)
	{
		return new Color(color.r, color.g, color.b, alpha);
	}
}