using System;
using UnityEngine;

public class EnemyAttackScriptBasic : AbstractEnemyAttackScript
{
	[SerializeField, Range(0f, 1f)]
	[Tooltip("Percentage of attack duration when hitbox is enabled")]
	float attackStartPercent = .2f;
	[SerializeField, Range(0f, 1f)]
	[Tooltip("Percentage of attack duration when hitbox is disabled")]
	float attackEndPercent = .9f;

    float startedAt = 0f;
    float endAt = 0f;
    float attackStartTime = 0f;
    float attackEndTime = 0f;

    bool colliderEnabled = false;

	// Update is called once per frame
	void Update()
    {
        if (!attackInProgress) return;

        if (Time.time >= attackStartTime && !colliderEnabled)
        {
            attackHitBox.GetComponent<Collider2D>().enabled = true;
            colliderEnabled = true;
		}
        if (Time.time >= attackEndTime && colliderEnabled)
        {
			attackHitBox.GetComponent<Collider2D>().enabled = false;
			colliderEnabled = false;
		}
        if (Time.time >= endAt)
        {
            attackInProgress = false;
		}
	}

    public override void Attack(Vector2 dir, float attackDuration, AttackAnimationFunction startAnimationFunction)
    {
        attackInProgress = true;
        startedAt = Time.time;

        attackStartTime = startedAt + attackDuration * attackStartPercent;
        attackEndTime = startedAt + attackDuration * attackEndPercent;
        endAt = startedAt + attackDuration;

        RotateHitBoxTowardsPlayer(dir);

		startAnimationFunction.Invoke(attackDuration, dir);
	}


}
