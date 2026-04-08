using Newtonsoft.Json;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

class GameManager : MonoSingleton<GameManager>
{
	[HideInInspector]
	public UnityEvent allEnemiesRespawn;
	[HideInInspector]
	public UnityEvent allDestructibleRespawn;
	[HideInInspector]
	public UnityEvent restartAll;

	string username = "TestUsername";
	public string Username { get => username; }

	private HPShowApproach _curHPShowApproach;
	public HPShowApproach CurHPShowApproach
	{
		get => _curHPShowApproach;
		set
		{
			_curHPShowApproach = value;
			onHPShowApproachChange.Invoke();
		}
	}
	private HPRegenApproach _curHPRegenApproach;
	public HPRegenApproach CurHPRegenApproach
	{
		get => _curHPRegenApproach;
		set
		{
			_curHPRegenApproach = value;
			onHPRegenApproachChange.Invoke();
		}
	}
	[HideInInspector]
	public UnityEvent onHPShowApproachChange;
	[HideInInspector]
	public UnityEvent onHPRegenApproachChange;

	List<HPShowApproach> completedShowApproaches = new();
	List<HPRegenApproach> completedRegenApproaches = new();

	public int NumOfCompletedApproaches { get => Mathf.Max(completedShowApproaches.Count, completedRegenApproaches.Count); }

	InputAction menuAction;

	public bool menuActive = false;

	public bool gameStarted = false;
	public float gameTimeApproach = 0f;
	public float gameTimeOverall = 0f;

	public float gameTimeLimitSeconds = 5 * 60;
	bool fillingForm = false;

	private void Start()
	{
		PauseGame();

		CurHPShowApproach = GetRandomShowApproach();
		CurHPRegenApproach = GetRandomRegenApproach();

		menuAction = InputSystem.actions.FindAction(GlobalConstants.menuInputActionName);

	}

	private void Update()
	{
		//TODO: Remove this debug code and implement a proper changing of approaches based on randomness and after a form is filled
		//if (Input.GetKeyDown(KeyCode.L))
		//{
		//	ChangeHPRegenApproach((HPRegenApproach)((int)(CurHPRegenApproach + 1) % System.Enum.GetValues(typeof(HPRegenApproach)).Length));
		//	ChangeHPShowApproach((HPShowApproach)((int)(CurHPShowApproach + 1) % System.Enum.GetValues(typeof(HPShowApproach)).Length));
		//}

		if (menuAction.WasPressedThisFrame() && gameStarted)
			ToggleMenu();

		gameTimeOverall += Time.deltaTime;
		gameTimeApproach += Time.deltaTime;

		//Timeout Check
		if (gameTimeApproach > gameTimeLimitSeconds && !fillingForm)
		{
			ApproachFinished();
		}
	}

	public void RespawnAllEnemies()
	{
		allEnemiesRespawn.Invoke();
	}

	public void RespawnAllDestructible()
	{
		allDestructibleRespawn.Invoke();
	}

	public void ChangeHPShowApproach(HPShowApproach newApproach)
	{
		CurHPShowApproach = newApproach;
		onHPShowApproachChange.Invoke();
	}

	public void ChangeHPRegenApproach(HPRegenApproach newApproach)
	{
		CurHPRegenApproach = newApproach;
		onHPRegenApproachChange.Invoke();
	}

	HPRegenApproach GetRandomRegenApproach()
	{
		List<HPRegenApproach> possibleApproaches = new();
		foreach (HPRegenApproach approach in System.Enum.GetValues(typeof(HPRegenApproach)))
		{
			if (!completedRegenApproaches.Contains(approach))
				possibleApproaches.Add(approach);
		}
		if (possibleApproaches.Count > 0)
			return possibleApproaches[Random.Range(0, possibleApproaches.Count)];
		else
			return CurHPRegenApproach;
	}
	HPShowApproach GetRandomShowApproach()
	{
		List<HPShowApproach> possibleApproaches = new();
		foreach (HPShowApproach approach in System.Enum.GetValues(typeof(HPShowApproach)))
		{
			if (!completedShowApproaches.Contains(approach))
				possibleApproaches.Add(approach);
		}
		if (possibleApproaches.Count > 0)
			return possibleApproaches[Random.Range(0, possibleApproaches.Count)];
		else
			return CurHPShowApproach;
	}

	public void ApproachFinished()
	{
		PauseGame();

		completedRegenApproaches.Add(CurHPRegenApproach);
		completedShowApproaches.Add(CurHPShowApproach);

		HUD.Instance.StartForm(CurHPShowApproach, CurHPRegenApproach);
		fillingForm = true;
	}

	public void FormFilled(Dictionary<string, string> formAnswersJson)
	{
		fillingForm = false;
		SendEndOfApproachData(formAnswersJson);

		gameTimeApproach = 0f;
		CurHPRegenApproach = GetRandomRegenApproach();
		CurHPShowApproach = GetRandomShowApproach();

		if (NumOfCompletedApproaches >= 3)
			HUD.Instance.ShowFinalScreen();
		else
			RestartGame();
	}

	void SendEndOfApproachData(Dictionary<string, string> formAnswersJson)
	{

		DataCollectionManager.AddGameTimeRecord(new GameTimeData()
		{ gameTimeApproach = gameTimeApproach, gameTimeOverall = gameTimeOverall });

		Dictionary<string, object> data = new();
		data["UserNickname"] = Username;
		data["HPRegenApproach"] = CurHPRegenApproach.ToString();
		data["HPShowApproach"] = CurHPShowApproach.ToString();
		data["Approaches done"] = NumOfCompletedApproaches.ToString();

		data["Form answers"] = formAnswersJson;
		data["Game analytics"] = DataCollectionManager.GetData();
		DataCollectionManager.ClearData();

		string jsonData = JsonConvert.SerializeObject(data);
		StartCoroutine(APIs.PostAnswersJSON(jsonData));
	}

	void RestartGame()
	{
		RespawnAllEnemies();
		RespawnAllDestructible();
		restartAll.Invoke();
		HUD.Instance.ActivateTutorial();
	}

	public static void StartGame(string inputUsername)
	{
		Instance.username = inputUsername;
		Instance.gameStarted = true;
		HUD.Instance.ActivateTutorial();
	}

	public static void PauseGame()
	{
		Time.timeScale = 0f;
	}
	public static void ResumeGame()
	{
		Time.timeScale = 1f;
	}

	public static void QuitGame()
	{
		Application.Quit();
	}

	public void ToggleMenu()
	{
		if (!menuActive)
		{
			menuActive = true;
			HUD.Instance.ShowMenu();
			PauseGame();
		}
		else
		{
			menuActive = false;
			HUD.Instance.HideMenu();
			ResumeGame();
		}
	}
}

public enum HPShowApproach { 	
	EldenRing,	HollowKnight, SilentHill
}

public enum HPRegenApproach
{
	OverTime, PickUp, BloodBorneRally, DarkSoulsItems, BloodborneItems
}