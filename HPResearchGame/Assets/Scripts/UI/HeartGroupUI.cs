using UnityEngine;

public class HeartGroupUI : MonoBehaviour
{
    [SerializeField]
    Sprite fullHeartSprite;
    [SerializeField]
    Sprite emptyHeartSprite;
    [SerializeField]
    Sprite halfHeartSprite;

    int maxHealth = 5;
    int curHealth = 5;

	public void UpdateMaxHealth(int maxHealth)
    {
        this.maxHealth = maxHealth;
        UpdateHearts();
    }

    public void UpdateCurHealth(int curHealth)
    {
        this.curHealth = curHealth;
        UpdateHearts();
    }

    public void UpdateHealth(int curHealth, int maxHealth)
    {
        this.curHealth = curHealth;
        this.maxHealth = maxHealth;
        UpdateHearts();
	}

	void UpdateHearts()
    {
        int curChildCount = transform.childCount;
        int numOfHeartContainers = Mathf.CeilToInt(maxHealth / 2f);
        int numOfFullHearts = Mathf.FloorToInt(curHealth / 2f);
        bool hasHalfHeart = curHealth % 2 == 1;
		while (curChildCount < numOfHeartContainers)
        {
            Instantiate(transform.GetChild(0).gameObject, transform);
            curChildCount++;
		}
        while (curChildCount > numOfHeartContainers)
        {
            Destroy(transform.GetChild(curChildCount - 1).gameObject);
            curChildCount--;
        }

		for (int i = 0; i < numOfHeartContainers; i++)
        {
            if (i < numOfFullHearts)
            {
                transform.GetChild(i).GetComponent<UnityEngine.UI.Image>().sprite = fullHeartSprite;
            }
            else if (i == numOfFullHearts && hasHalfHeart)
            {
                transform.GetChild(i).GetComponent<UnityEngine.UI.Image>().sprite = halfHeartSprite;
			}
			else
            {
                transform.GetChild(i).GetComponent<UnityEngine.UI.Image>().sprite = emptyHeartSprite;
            }
		}
	}

}
