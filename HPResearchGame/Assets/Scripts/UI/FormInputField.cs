using TMPro;
using UnityEngine;

public class FormInputField : MonoBehaviour, IFormInteractable
{

    TMP_InputField myInputField;

	FormScript parentForm;
	string myQuestion;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myInputField = GetComponent<TMP_InputField>();
		myInputField.onEndEdit.AddListener(EndEdit);
		LoadIn();
    }
	void AssignParentFormAndQuestion()
	{
		//With my index as answer -> save the answer to the form script

		TextMeshProUGUI parentTMPro = transform.parent.GetComponent<TextMeshProUGUI>();
		if (parentTMPro == null)
		{
			Debug.LogError("Parent of star does not have TMPro, while it should be the question! -> won't record answer");
			return;
		}
		myQuestion = parentTMPro.text;

		FormScript parentFormScript = parentTMPro.transform.parent.parent.GetComponent<FormScript>();
		if (parentFormScript == null)
		{
			Debug.LogError("Grandparent of star does not have FormScript! -> won't record answer");
			return;
		}
		parentForm = parentFormScript;

	}

	public void LoadIn()
    {
		AssignParentFormAndQuestion();

		//Initially record all answers as nothing
		parentForm.RecordAnswer(myQuestion, "");

		if (myInputField != null) 
			myInputField.text = string.Empty;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EndEdit(string newText)
    {
		parentForm.RecordAnswer(myQuestion, newText);
    }
}
