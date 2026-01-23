using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
	const string animXMoveID = "MoveX";
	const string animYMoveID = "MoveY";

    [Header("Combat information")]
	public int maxHP = 3;
    public int currentHP;

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

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        currentHP = maxHP;

        agent = GetComponent<NavMeshAgent>();
		sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

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
	}

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
}
