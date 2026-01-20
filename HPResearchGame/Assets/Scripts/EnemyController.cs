using DG.Tweening;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int maxHP = 3;
    public int currentHP;

    [Header("Tween information")]
    [SerializeField]
    float afterHitTweenDuration = 0.2f;
    [SerializeField]
    Color afterHitColor = Color.red;




    Color originalColor;

	SpriteRenderer sr;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        currentHP = maxHP;

        sr = GetComponent<SpriteRenderer>();

        originalColor = sr.color;
	}

    // Update is called once per frame
    void Update()
    {
        
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
