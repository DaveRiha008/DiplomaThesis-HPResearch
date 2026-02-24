using UnityEngine;
using UnityEngine.UI;

public class BloodyScreens : MonoBehaviour
{
    [SerializeField] Image border;
    [SerializeField] Image drip;
    [SerializeField] Image drops;

    float maxHealth = 100f;
    float currentHealth = 100f;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        drip.gameObject.SetActive(false);
        drops.gameObject.SetActive(false);
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        this.currentHealth = currentHealth;
        this.maxHealth = maxHealth;
        UpdateBloodyScreens();
	}

    void UpdateBloodyScreens()
    {
        float healthPercent = currentHealth / maxHealth;
        border.color = border.color.WithAlpha((1 - healthPercent)*1.3f);
        
        if (healthPercent < 0.33f)
            drip.gameObject.SetActive(true);
        else
			drip.gameObject.SetActive(false);

		if (healthPercent < 0.15f)
			drops.gameObject.SetActive(true);
		else
			drops.gameObject.SetActive(false);
	}
}
