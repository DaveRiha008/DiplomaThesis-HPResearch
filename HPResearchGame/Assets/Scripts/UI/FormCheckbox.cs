using UnityEngine;
using UnityEngine.UI;

public class FormCheckbox : MonoBehaviour
{
    [SerializeField]
    string myQuestion = "I don't play combat videogames";

    [SerializeField]
    Color selectedColor = Color.green;
    Color originalColor;

    bool selected = false;

    FormScript parentForm;
    Image myImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myImage = GetComponent<Image>();
        originalColor = myImage.color;

		FormScript parentFormScript = transform.parent.parent.parent.GetComponent<FormScript>();
		if (parentFormScript == null)
		{
			Debug.LogError("3xParent of checkbox does not have FormScript! -> won't record answer");
			return;
		}
		parentForm = parentFormScript;

		parentForm.RecordCheckboxAnswer(myQuestion, false);
	}

	// Update is called once per frame
	void Update()
    {
        
    }

    void Clicked()
    {
        if (selected)
        {
            selected = false;
            myImage.color = originalColor;
        }
        else
        {
            selected = true;
            myImage.color = selectedColor;
        }
	}

    public void DontPlayCombatGamesToggle()
    {
        Clicked();


        parentForm.RecordCheckboxAnswer(myQuestion, selected);
    }
}
