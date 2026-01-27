using System;
using UnityEngine;

public abstract class AbstractEnemyAttackScript:MonoBehaviour
{
	[SerializeField]
	protected PublicCollisionEvents attackHitBox;

	protected bool attackInProgress = false;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	protected virtual void Start()
	{
		attackHitBox.GetComponent<Collider2D>().enabled = false;
		attackHitBox.triggerEnter2D.AddListener(HitSomething);
	}



	abstract public void Attack(Vector2 dir, float attackDuration, AttackAnimationFunction startAnimationFunction);

	/// <summary>
	/// Rotates the hitbox in the direction of the player
	/// <br>Should match the current sprite and its rotation</br>
	/// </summary>
	protected void RotateHitBoxTowardsPlayer(Vector2 directionToPlayer)
	{
		Vector3 angle = Quaternion.FromToRotation(Vector3.down, directionToPlayer).eulerAngles;
		attackHitBox.transform.rotation = Quaternion.Euler(angle);
	}

	void HitSomething(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			PlayerController player = collision.gameObject.GetComponent<PlayerController>();
			if (player != null)
			{
				player.GetHit(1);
			}
		}
	}

	public delegate void AttackAnimationFunction(float attackDuration, Vector2 directionToPlayer);
}