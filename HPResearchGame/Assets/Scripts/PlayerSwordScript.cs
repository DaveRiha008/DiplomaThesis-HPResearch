using UnityEngine;

public class PlayerSwordScript : MonoBehaviour
{
    const string animAttackSpeedID = "AttackSpeed";
    const string animAttackTriggerID = "Attack";

	Animator animator;

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
        Vector3 rotation = Quaternion.FromToRotation(VecConvert.ToVec3CustomZ(Vector2.down, 0f), VecConvert.ToVec3CustomZ(mouseOffset, 0f)).eulerAngles;
        transform.parent.rotation = Quaternion.Euler(rotation);

        animator.SetFloat(animAttackSpeedID, attackSpeed);
        animator.SetTrigger(animAttackTriggerID);
    }
}
