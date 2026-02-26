using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

class GameManager : MonoSingleton<GameManager>
{
	public UnityEvent allEnemiesRespawn;
	public UnityEvent allDestructibleRespawn;

	public void RespawnAllEnemies()
	{
		allEnemiesRespawn.Invoke();
	}

	public void RespawnAllDestructible()
	{
		allDestructibleRespawn.Invoke();
	}
}