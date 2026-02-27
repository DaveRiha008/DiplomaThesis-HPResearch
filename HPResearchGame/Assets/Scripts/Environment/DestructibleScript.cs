using UnityEngine;

public class DestructibleScript : MonoBehaviour
{
    const string animHitTrigger = "GotHit";
    const string animDestroyTrigger = "Destroyed";
    const string animRespawnTrigger = "Respawn";

    public float chanceToDropHealItem = 0.3f;

	int hitPoints = 2;

    Animator animator;
    Collider2D col;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        GameManager.Instance.allDestructibleRespawn.AddListener(Respawn);

		animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    ///<returns>If the object was destroyed</returns>
    public bool GotHit()
    {
        hitPoints--;
        if (hitPoints <= 0)
        {
            GotDestroyed();
            return true;
		}
        else
        {
            animator.SetTrigger(animHitTrigger);
            return false;
		}
	}

    void GotDestroyed()
    {
		animator.SetTrigger(animDestroyTrigger);
        col.enabled = false;

	}

	public void Respawn()
    {
        col.enabled = true;
		hitPoints = 2;
        animator.SetTrigger(animRespawnTrigger);
	}
}
