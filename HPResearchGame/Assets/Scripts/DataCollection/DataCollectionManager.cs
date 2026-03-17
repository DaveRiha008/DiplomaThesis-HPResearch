using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class DataCollectionManager
{
    static DataCollectionAllData allData = new();

    public static object GetData()
    {
        return allData;
    }

    public static void ClearData()
    {
        allData = new();
    }

    public static void AddDeathRecord(DeathData data)
    {
        allData.deathDatas.Add(data);

        Debug.Log($"Adding a death record");
    }

    public static void AddCombatRecord(CombatData data)
    {
		allData.combatDatas.Add(data);

        Debug.Log($"Adding a combat record");
    }

    public static void AddGetHitRecord(GetHitData data)
    {
		allData.getHitDatas.Add(data);

        Debug.Log($"Adding a getHit record");
    }

    public static void AddGameTimeRecord(GameTimeData data)
    {
        allData.gameTimeData = data;
    }

    public static void AddHealRecord(HealData data)
    {
        allData.healDatas.Add(data);
    }

    public static void AddCheckpointRecord(CheckpointData data)
    {
        allData.checkpointDatas.Add(data);
    }

    public static void AddBoxBreakRecord(BoxBreakData data)
    {
        allData.boxBreakDatas.Add(data);
    }

    public static void AreaEntered(int areaIndex)
    {
        allData.reachedArea = Mathf.Max(areaIndex, allData.reachedArea);
    }

    [Serializable]
    class DataCollectionAllData
    {
	    public List<DeathData> deathDatas = new();
	    public List<CombatData> combatDatas = new();
	    public List<GetHitData> getHitDatas = new();
        public GameTimeData gameTimeData = new();
        public List<HealData> healDatas = new();
        public List<CheckpointData> checkpointDatas = new();
        public List<BoxBreakData> boxBreakDatas = new();
        public int reachedArea = 0;
    }
}
