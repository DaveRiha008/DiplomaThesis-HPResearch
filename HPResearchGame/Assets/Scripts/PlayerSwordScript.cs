using UnityEngine;

public class PlayerSwordScript : MonoBehaviour
{
    const string animAttackSpeedID = "AttackSpeed";
    const string animAttackTriggerID = "Attack";

	Animator animator;
    [SerializeField]
    PlayerController myPlayer;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        animator = GetComponent<Animator>();
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AnimateAttack(float attackSpeed, Vector2 mouseOffset)
    {
        //Null check, because this script can be initially disabled and then this
        //method called in the same frame as it is activated, which doesn't call the Start() in time
        if (animator == null)
            animator = GetComponent<Animator>();

        Vector3 rotation = Quaternion.FromToRotation(VecConvert.ToVec3CustomZ(Vector2.down, 0f), VecConvert.ToVec3CustomZ(mouseOffset, 0f)).eulerAngles;
        transform.parent.rotation = Quaternion.Euler(rotation);

        animator.SetFloat(animAttackSpeedID, attackSpeed);
        animator.SetTrigger(animAttackTriggerID);
    }


	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Enemy"))
		{
            bool isEnemy = collision.TryGetComponent(out EnemyController enemyHit);
            if (!isEnemy)
            {
                Debug.LogError("Enemy tagged object does not have EnemyController component");
                return;
			}

            myPlayer.EnemyHit(enemyHit);
		}
	}
}
