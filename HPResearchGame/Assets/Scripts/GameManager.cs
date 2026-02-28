using UnityEngine.Events;
using UnityEngine;
using UnityEngine.InputSystem;

class GameManager : MonoSingleton<GameManager>
{
	[HideInInspector]
	public UnityEvent allEnemiesRespawn;
	[HideInInspector]
	public UnityEvent allDestructibleRespawn;

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

	InputAction menuAction;

	public bool menuActive = false;

	private void Start()
	{
		CurHPShowApproach = HPShowApproach.HollowKnight;
		CurHPRegenApproach = HPRegenApproach.BloodBorneRally;

		menuAction = InputSystem.actions.FindAction(GlobalConstants.menuInputActionName);
	}

	private void Update()
	{
		//TODO: Remove this debug code and implement a proper changing of approaches based on randomness and after a form is filled
		if (Input.GetKeyDown(KeyCode.L))
		{
			ChangeHPRegenApproach((HPRegenApproach)((int)(CurHPRegenApproach + 1) % System.Enum.GetValues(typeof(HPRegenApproach)).Length));
			ChangeHPShowApproach((HPShowApproach)((int)(CurHPShowApproach + 1) % System.Enum.GetValues(typeof(HPShowApproach)).Length));
		}

		if (menuAction.WasPressedThisFrame())
			ToggleMenu();
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