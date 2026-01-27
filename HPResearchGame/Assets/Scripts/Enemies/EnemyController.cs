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
    float attackEndedAt = -1000f;
    bool isAttacking = false;


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

    //Components
	NavMeshAgent agent;
	SpriteRenderer sr;
    Animator animator;
    AbstractEnemyAttackScript attackScript;

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


		originalColor = sr.color;
	}

    // Update is called once per frame
    void Update()
    {
        RetargetDestination();

        SetAnimationMove();

        if (!isAttacking && agent.remainingDistance <= agent.stoppingDistance && Time.time - attackEndedAt >= attackCooldown)
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

        agent.SetDestination(target.position);
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

	public void GetHit(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            Die();
            return;
        }
        TweenAfterHit();


	}

    void TweenAfterHit()
    {
		sr.DOKill(); //Stop any ongoing tweens on the SpriteRenderer to avoid color conflicts
		sr.color = afterHitColor;
		sr.DOColor(originalColor, afterHitTweenDuration);
	}


    void Die()
    {
        sr.DOKill(); //Stop any ongoing tweens on the SpriteRenderer to avoid null reference issues
		Destroy(gameObject);
	}

	#endregion
}
