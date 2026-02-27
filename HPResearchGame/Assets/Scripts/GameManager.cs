using UnityEngine.Events;

class GameManager : MonoSingleton<GameManager>
{
	public UnityEvent allEnemiesRespawn;
	public UnityEvent allDestructibleRespawn;

	private HPShowApproach _curHPShowApproach;
	public HPShowApproach CurHPShowApproach { get => _curHPShowApproach; 
		set 
		 {
			 _curHPShowApproach = value;
			 onHPShowApproachChange.Invoke();
		 }
	}
	private HPRegenApproach _curHPRegenApproach;
	public HPRegenApproach CurHPRegenApproach { get => _curHPRegenApproach; 
		set 
		{
			_curHPRegenApproach = value;
			onHPRegenApproachChange.Invoke();
		}
	}
	public UnityEvent onHPShowApproachChange;
	public UnityEvent onHPRegenApproachChange;

	private void Start()
	{
		CurHPShowApproach = HPShowApproach.HollowKnight;
		CurHPRegenApproach = HPRegenApproach.BloodborneItems;
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
}

public enum HPShowApproach { 	
	EldenRing,	HollowKnight, SilentHill
}

public enum HPRegenApproach
{
	OverTime, PickUp, BloodBorneRally, DarkSoulsItems, BloodborneItems
}