using System;
using UnityEngine;

public interface CollectableObject
{
}

[Serializable]
public struct DeathData: CollectableObject
{
	public DateTime timestamp;
}

[Serializable]
public class CombatData: CollectableObject
{
	static DateTime timeStartTemp;
	static bool startTimeInitiated;

	public DateTime timeStart;
	public DateTime timeEnd;

	public static void StartCombat()
	{
		startTimeInitiated = true;
		timeStartTemp = DateTime.Now;
	}

	public static void StopCombat()
	{
		if (!startTimeInitiated)
		{
			Debug.LogError("Combat stopped, but dataCollector never recognized start");
			return;
		}

		DataCollectionManager.AddCombatRecord(new CombatData() { timeStart = timeStartTemp, timeEnd = DateTime.Now });
		startTimeInitiated = false;
	}
}

[Serializable]
public struct GetHitData: CollectableObject
{
	public DateTime timestamp;
	public float fromHP;
	public float toHP;
}

[Serializable]
public struct GameTimeData: CollectableObject
{
	public float gameTimeApproach;
	public float gameTimeOverall;
}

[Serializable]
public struct HealData: CollectableObject
{
	public float fromHP;
	public float toHP;
	public DateTime timestamp;
}

[Serializable]
public struct CheckpointData: CollectableObject
{
	public DateTime timestamp;
}

[Serializable]
public struct BoxBreakData: CollectableObject
{
	public DateTime timestamp;
}