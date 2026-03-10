using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class APIs
{

	//TODO Post Form data, Post Game Analytics
	public static IEnumerator PostAnswersJSON(string json)
	{
		Debug.Log($"Post answers called -> sending: {json}");
		byte[] rawData = Encoding.UTF8.GetBytes(json);

		UnityWebRequest www = new UnityWebRequest(GlobalConstants.serverEndpoint, "POST");
		www.uploadHandler = new UploadHandlerRaw(rawData);
		www.downloadHandler = new DownloadHandlerBuffer();
		www.SetRequestHeader("Content-Type", "application/json");
		www.SetRequestHeader("If-Match", "*");

		yield return www.SendWebRequest();

		if (www.result != UnityWebRequest.Result.Success)
		{
			//This doesn't show the message returned from backend
			Debug.LogError("In the error of putAnalytics");
			//This is the message from backend
			Debug.LogError("Message returned: " + www.downloadHandler.text);

			Debug.LogError(www.error);
		}
		else
		{
			//return confirmation if analytics are successfully added to the db
			if (www.isDone)
			{
				var result = Encoding.UTF8.GetString(www.downloadHandler.data);
				Debug.Log("In post logs: " + result);
			}
		}
	}
}