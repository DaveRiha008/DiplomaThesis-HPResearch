using UnityEngine;
using UnityEngine.Networking;

public static class APIs
{

	//TODO Post Form data, Post Game Analytics
	public static void PostAnswersJSON(string json, string endpoint)
	{
		Debug.Log($"Post answers called, but not implemented yes -> reiceved {json}");
		UnityWebRequest request = new UnityWebRequest(endpoint);
	}
}