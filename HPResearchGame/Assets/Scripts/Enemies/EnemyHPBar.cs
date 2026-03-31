using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBar : MonoBehaviour
{
    [SerializeField] EnemyController myController;

    Slider mySlider;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        mySlider = GetComponent<Slider>();
        mySlider.minValue = 0;
        mySlider.maxValue = myController.MaxHP;
	}

    // Update is called once per frame
    void Update()
    {
        mySlider.value = myController.currentHP;
	}
}
