using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    const string animXMoveID = "MoveX";
    const string animYMoveID = "MoveY";
    const string animAttackTriggerID = "AttackTrigger";
    const string animAttackXID = "AttackX";
    const string animAttackYID = "AttackY";
    const string animAttackSpeedID = "AttackSpeed";

    [Header("Combat information")]
    [SerializeField]
    int maxHP = 3;
    public int currentHP;
    [SerializeField]
    float attackCooldown = 1f;
    [SerializeField]
    float attackDuration = .3f;
    float attackStartedAt = 0f;
    float attackEndedAt = 0f;
    bool isAttacking = false;

    [Header("Experience gain")]
    public int experienceOnDeath = 5;

	[Header("Tween information")]
    [SerializeField]
    float afterHitTweenDuration = 0.2f;
    [SerializeField]
    Color afterHitColor = Color.red;

    Color originalColor;

    //AI Navigation variables
    [Header("AI Navigation")]
    [SerializeField]
    Transform target;
    [SerializeField]
    float minDistanceToTarget = 2f;

    //Awareness variables
    [Header("Awareness")]
    [SerializeField]
    float timeToForgetTarget = 10f;
    [SerializeField]
    float timeToReturnToOriginalLocationAfterForget = 3f;
    EnemyState curEnemyState = EnemyState.Idle;
    bool targetInAwareness = false;
    Vector3 originalLocation;
    float lastTimeTargetInAwareness = -1000f;
    float timeStartedIdling = -1000f;

    //Components
    NavMeshAgent agent;
    SpriteRenderer sr;
    Animator animator;
    AbstractEnemyAttackScript attackScript;

    //Object References
    [Header("Object References")]
	[SerializeField]
	PublicCollisionEvents awarenessArea;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        currentHP = maxHP;

        agent = GetComponent<NavMeshAgent>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        attackScript = GetComponent<AbstractEnemyAttackScript>();

        //Agent setup
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.stoppingDistance = minDistanceToTarget;

        BindAwarenessTriggerMethods();

        originalColor = sr.color;
        originalLocation = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEnemyState();

        RetargetDestination();

        SetAnimationMove();

        if (!isAttacking && 
            agent.remainingDistance <= minDistanceToTarget && 
            Time.time - attackEndedAt >= attackCooldown)
        {
			Attack();
        }

        else if (isAttacking)
        {
            if (Time.time - attackStartedAt >= attackDuration)
            {
                AttackEnd();
            }
        }
    }

    #region Movement
    void SetAnimationMove()
    {
        Vector3 velocity = agent.velocity.normalized;
        float ratio = Mathf.Abs(velocity.x) + Mathf.Abs(velocity.y); //Get the ratio to normalize diagonal movement
        if (ratio == 0) ratio = 1f; //Avoid division by zero

        //Normalize, so the sum always equals 1
        animator.SetFloat(animXMoveID, velocity.x / ratio);
        animator.SetFloat(animYMoveID, velocity.y / ratio);
    }

    void RetargetDestination()
    {
        if (awarenessArea == null)
        {
            agent.SetDestination(target.position);
            return;
        }

        switch (curEnemyState)
        {
            case EnemyState.Idle:
                agent.SetDestination(transform.position); //Stay in place
                break;
            case EnemyState.Following:
                agent.SetDestination(target.position); //Follow the target
                break;
            case EnemyState.Returning:
                agent.SetDestination(originalLocation); //Return to original location
                break;
        }
    }

    #endregion

    #region Combat

    void Attack()
    {
        isAttacking = true;
        attackStartedAt = Time.time;

        //Calculate attack direction (only cardinal directions)
        Vector2 attackDir = (target.position - transform.position).normalized;
        if (Mathf.Abs(attackDir.x) > Mathf.Abs(attackDir.y))
            attackDir.y = 0f;
        else
            attackDir.x = 0f;



        //Call attack script
        if (attackScript != null)
            attackScript.Attack(attackDir, attackDuration, StartAttackAnimation);

        //Stop movement during attack
        agent.isStopped = true;
        agent.velocity = Vector2.zero;
	}

    void StartAttackAnimation(float duration, Vector2 attackDir)
    {
        //Set attack animation parameters
        animator.SetFloat(animAttackXID, attackDir.x);
        animator.SetFloat(animAttackYID, attackDir.y);
        animator.SetFloat(animAttackSpeedID, 1 / (duration / 0.667f)); //0.667f is the duration of the original attack animation
        animator.SetTrigger(animAttackTriggerID);
    }
    void AttackEnd()
    {
        isAttacking = false;
        attackEndedAt = Time.time;
        agent.isStopped = false;
    }

    public bool GetHit(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            Die();
            return true;
        }
        TweenAfterHit();
        UIFlashingNumbers.ShowFlashingNumber(transform, damage, Color.red, GetComponent<Collider2D>().bounds.size/2 * Vector2.up);
        return false;
	}

    void TweenAfterHit()
    {
        sr.DOKill(); //Stop any ongoing tweens on the SpriteRenderer to avoid color conflicts
        sr.color = afterHitColor;
        sr.DOColor(originalColor, afterHitTweenDuration).SetLink(gameObject);
    }


    void Die()
    {
        sr.DOKill(); //Stop any ongoing tweens on the SpriteRenderer to avoid null reference issues
        Destroy(gameObject);
    }

    #endregion

    #region Awareness



    void BindAwarenessTriggerMethods()
    {
        if (awarenessArea != null)
        {
            awarenessArea.triggerEnter2D.AddListener(AwarenessArea_OnEnterTrigger);
            awarenessArea.triggerExit2D.AddListener(AwarenessArea_OnExitTrigger);
            curEnemyState = EnemyState.Idle; //Stop movement until the target is detected
            //Detach the transform to avoid moving the awareness area with the enemy
            awarenessArea.transform.parent = null;
		}
        else 
        { 
			curEnemyState = EnemyState.Following; //If no awareness area is set, always follow the target
        } 
    }

    void AwarenessArea_OnEnterTrigger(Collider2D collider)
    {
        if (collider.transform == target)
            TargetEnteredAwareness();
	}
    void AwarenessArea_OnExitTrigger(Collider2D collider)
    {
        if (collider.transform == target)
            TargetExitedAwareness();
	}


    void TargetEnteredAwareness()
    {
        targetInAwareness = true;
		curEnemyState = EnemyState.Following;
	}

    void TargetExitedAwareness()
    {
        targetInAwareness = false;
		lastTimeTargetInAwareness = Time.time;
	}

    void UpdateEnemyState()
    {
        float timeSinceLastInAwareness = Time.time - lastTimeTargetInAwareness;
        float timeSinceLastAttack = Time.time - attackEndedAt;
        float timeIdling = Time.time - timeStartedIdling;

		//If the target is in awareness or the enemy recently attacked, keep following
		if (targetInAwareness || timeSinceLastAttack < timeToForgetTarget)
        {
            curEnemyState = EnemyState.Following;
            return;
        }

		//If the target is not in awareness and enough time has passed, switch to idle
		if (curEnemyState == EnemyState.Following && timeSinceLastInAwareness >= timeToForgetTarget)
        {
            curEnemyState = EnemyState.Idle;
            timeStartedIdling = Time.time;
			return;
		}

		//If idle for enough time, return to original location
		if (curEnemyState == EnemyState.Idle && timeIdling >= timeToReturnToOriginalLocationAfterForget &&
			Vector3.Distance(transform.position, originalLocation) > agent.stoppingDistance)
        {
            curEnemyState = EnemyState.Returning;
            return;
        }

		//If returned to original location, switch to idle
		if (curEnemyState == EnemyState.Returning && Vector3.Distance(transform.position, originalLocation) <= agent.stoppingDistance)
        {
            curEnemyState = EnemyState.Idle;
            return;
		}

	}

	#endregion

    enum EnemyState
    {
        Idle,
        Following,
        Returning
	}
}