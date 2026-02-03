using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

class GameManager : MonoSingleton<GameManager>
{
	public List<EnemyController> killedEnemies = new();

	public void TemporarilyDisableEnemy(EnemyController enemy)
	{
		killedEnemies.Add(enemy);
		enemy.gameObject.SetActive(false);
	}

	public void RespawnAllEnemies()
	{
		foreach (var enemy in killedEnemies)
		{
			enemy.Respawn();
		}
		killedEnemies = new();
	}
}