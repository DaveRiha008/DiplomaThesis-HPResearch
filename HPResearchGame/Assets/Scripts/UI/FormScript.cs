using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
public class FormScript : MonoBehaviour
{

    [SerializeField] Button submitButton;


    Dictionary<string, int> questionAnswers = new Dictionary<string, int>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        submitButton.onClick.AddListener(SubmitAnswers);
    }

    // Update is called once per frame
    void Update()
    {

    }



    public void RecordAnswer(string question, int rating)
    {
        questionAnswers[question] = rating;
    }

    public void SubmitAnswers()
    {
        string jsonAnswers = JsonConvert.SerializeObject(questionAnswers);
        APIs.PostAnswersJSON(jsonAnswers, "www.endpoint.random");
    }
}
