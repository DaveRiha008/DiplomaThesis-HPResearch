using TMPro;
using UnityEngine;

public class StartMenuScript : MonoBehaviour
{
    public void StartGame(TMP_InputField inputUsername)
    {
		if (inputUsername == null || string.IsNullOrEmpty(inputUsername.text))
			return;

		print($"Starting game with username: {inputUsername.text}");

		GameManager.StartGame(inputUsername.text);
		gameObject.SetActive(false);
	}
}
