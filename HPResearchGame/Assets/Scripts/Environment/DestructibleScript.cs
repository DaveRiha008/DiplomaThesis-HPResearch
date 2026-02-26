using UnityEngine;

public class DestructibleScript : MonoBehaviour
{
    const string animHitTrigger = "GotHit";
    const string animDestroyTrigger = "Destroyed";
    const string animRespawnTrigger = "Respawn";

    int hitPoints = 2;

    Animator animator;
    Collider2D collider;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        GameManager.Instance.allDestructibleRespawn.AddListener(Respawn);

		animator = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
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
        collider.enabled = false;

	}

	public void Respawn()
    {
        collider.enabled = true;
		hitPoints = 2;
        animator.SetTrigger(animRespawnTrigger);
	}
}
