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


    public void RecordAnswer(string question, string rating)
    {
        questionAnswers[question] = rating;
    }

    public void SubmitAnswers()
    {
        Dictionary<string, string> data = new();
        data["UserNickname"] = GameManager.Instance.Username;
        data["HPRegenApproach"] = GameManager.Instance.CurHPRegenApproach.ToString();
        data["HPShowApproach"] = GameManager.Instance.CurHPShowApproach.ToString();
        data["Approaches done"] = GameManager.Instance.NumOfCompletedApproaches.ToString();
        
        string jsonAnswers = JsonConvert.SerializeObject(questionAnswers);
        data["Form answers"] = (jsonAnswers);

        string jsonData = JsonConvert.SerializeObject(data);
        
        GameManager.Instance.StartCoroutine(APIs.PostAnswersJSON(jsonData));
        Deactivate();
        GameManager.Instance.FormFilled();
    }
}
