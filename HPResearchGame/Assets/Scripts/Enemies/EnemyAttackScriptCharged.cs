using DG.Tweening;
using UnityEngine;
using System;

public class EnemyAttackScriptCharged : AbstractEnemyAttackScript
{
    [SerializeField]
    SpriteRenderer attackIndicatorSprite;

    [SerializeField, Range(0f,1f)]
	[Tooltip("Percentage of attack duration for charging")]
	float attackChargeTimePercent = .8f;

    [SerializeField]
    SpriteRenderer enemySpriteR;
    [SerializeField]
    ChargingSprites chargingSprites;
	Sprite originalSprite;

	float startedAt = 0f;
    float endAt = 0f;
    float chargeDuration = 0f;
	float chargeEndTime = 0f;

    bool colliderEnabled = false;

    AttackAnimationFunction animationFunction;
    Vector2 attackDir;
    float attackDur;

	protected override void Start()
	{
		base.Start();
        attackIndicatorSprite.enabled = false;
	}

	// Update is called once per frame
	void Update()
    {
        if (!attackInProgress) return;

        if (Time.time >= chargeEndTime && !colliderEnabled)
            EndCharge();

        if (Time.time >= endAt)
        {
            EndAttack();
		}
	}

    void EndCharge()
    {
        EnableCollider();
        animationFunction.Invoke(attackDur-chargeDuration, attackDir);
    }

    void EndAttack()
    {
		DisableCollider();
		enemySpriteR.sprite = originalSprite;
		attackIndicatorSprite.enabled = false;
		attackInProgress = false;
	}

    void EnableCollider()
    {
		attackHitBox.GetComponent<Collider2D>().enabled = true;
		colliderEnabled = true;
	}

    void DisableCollider()
	{
		attackHitBox.GetComponent<Collider2D>().enabled = false;
		colliderEnabled = false;
	}

    Sprite GetChargingSprite(Vector2 dir)
    {
        if (dir.x > 0)
            return chargingSprites.right;
        else if (dir.x < 0)
            return chargingSprites.left;
        else if (dir.y > 0)
            return chargingSprites.up;
		else
            return chargingSprites.down;
	}

	public override void Attack(Vector2 dir, float attackDuration, AttackAnimationFunction startAnimationFunction)
    {
		//Save the parameters for later use
		animationFunction = startAnimationFunction;
        attackDir = dir;
        attackDur = attackDuration;

		//Save the time variables
		attackInProgress = true;
        startedAt = Time.time;
        chargeDuration = attackDuration * attackChargeTimePercent;
        chargeEndTime = startedAt + chargeDuration;
		endAt = startedAt + attackDuration;

		//Rotate for correct hitbox direction
		RotateHitBoxTowardsPlayer(dir);

		//Sprite management
		originalSprite = enemySpriteR.sprite;
        enemySpriteR.sprite = GetChargingSprite(dir);

		//Attack indicator animation
		attackIndicatorSprite.enabled = true;
        attackIndicatorSprite.color = attackIndicatorSprite.color.WithAlpha(0);
        attackIndicatorSprite.DOColor(attackIndicatorSprite.color.WithAlpha(1f), attackDuration * attackChargeTimePercent)
            .SetEase(Ease.Linear);

	}

    [Serializable]
    private struct ChargingSprites
    {
        public Sprite down;
        public Sprite right;
        public Sprite up;
        public Sprite left;
	}
}
