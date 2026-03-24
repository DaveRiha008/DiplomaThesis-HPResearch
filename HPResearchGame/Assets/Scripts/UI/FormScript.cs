using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
public class FormScript : MonoBehaviour
{

    [SerializeField] Button submitButton;


    Dictionary<string, string> questionAnswers = new Dictionary<string, string>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        submitButton.onClick.AddListener(SubmitAnswers);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Activate(HPShowApproach showApp, HPRegenApproach regenApp)
    {
        gameObject.SetActive(true);

        //TODO: Load different questions based on approaches

        foreach (IFormInteractable star in transform.Find("Questions").GetComponentsInChildren<IFormInteractable>())
        {
            star.LoadIn();
		}
	}

    public void Deactivate()
    {
        gameObject.SetActive(false);
        questionAnswers.Clear();
    }


    public void RecordAnswer(string question, string rating, string lowOption = "", string highOption = "")
    {
        if (lowOption == string.Empty && highOption == string.Empty)
            questionAnswers[question] = rating;
        else
            questionAnswers[$"{question}___{lowOption}_{highOption}"] = rating;
    }

    public void SubmitAnswers()
    {
        GameManager.Instance.FormFilled(questionAnswers);
        
        Deactivate();
    }
}
