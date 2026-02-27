using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class PickUpHeal : MonoBehaviour
{
    public int HealAmount = 5;

    [SerializeField]
    float respawnTime = 5f;
    float timePickedUp = 0f;

    bool isActive = true;

	SpriteRenderer spriteRenderer;
    Collider2D myCollider;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        GameManager.Instance.onHPRegenApproachChange.AddListener(() =>
        {
            if (GameManager.Instance.CurHPRegenApproach == HPRegenApproach.PickUp)
                Appear();
            else
                Disappear();

        });

        GameManager.Instance.allDestructibleRespawn.AddListener(() =>
        {
            if (GameManager.Instance.CurHPRegenApproach == HPRegenApproach.PickUp)
                Appear();
        });

        spriteRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();

        StartTweenOfMoving();
	}

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.CurHPRegenApproach != HPRegenApproach.PickUp)
            return;

        if (Time.time - timePickedUp > respawnTime && !isActive)
            Appear();
	}
    void Appear()
    {
        isActive = true;
        myCollider.enabled = true;
        spriteRenderer.enabled = true;
	}
    void Disappear()
    {
        isActive = false;
        myCollider.enabled = false;
        spriteRenderer.enabled = false;
        timePickedUp = Time.time;
	}

    void StartTweenOfMoving()
    {
        Vector3 origPos = transform.position;
		transform.DOMove(origPos.WithY(origPos.y + .1f), .5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isActive)
        {
            if(!other.TryGetComponent(out PlayerController player))
            {
                Debug.LogError("Object tagged Player does not have PlayerController component.");
                return;
			}

            player.Heal(HealAmount);
            Disappear();
        }
	}
}
