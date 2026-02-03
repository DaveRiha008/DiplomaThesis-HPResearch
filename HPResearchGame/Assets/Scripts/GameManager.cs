using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

class GameManager : MonoSingleton<GameManager>
{
	public UnityEvent allEnemiesRespawn;

	//public void TemporarilyDisableEnemy(EnemyController enemy)
	//{
	//	//killedEnemies.Add(enemy);
	//	//enemy.gameObject.SetActive(false);
	//}

	public void RespawnAllEnemies()
	{
		//foreach (var enemy in killedEnemies)
		//{
		//	enemy.Respawn();
		//}
		//killedEnemies = new();
		allEnemiesRespawn.Invoke();
	}
}