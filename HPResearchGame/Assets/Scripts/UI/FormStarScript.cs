using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class FormStarScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IFormInteractable
{
    Image myImage;

    //This star has the mouse over it
    bool mouseOver = false;

    //This star was selected -> will stay active until a lower star is clicked
    public bool selected = false;


    FormStarScript[] allStarsForThisQuestion;

	[SerializeField] Sprite activeSprite;
    [SerializeField] Sprite dormantSprite;


    FormScript parentForm;
    string myQuestion;

    string myLowOption;
    string myHighOption;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        LoadIn();
	}

    /// <summary>
    /// Initializes component references, assigns parent form and question, records a default answer, and deselects the
    /// current item.
    /// </summary>
    public void LoadIn()
    {
		myImage = GetComponent<Image>();
		allStarsForThisQuestion = transform.parent.GetComponentsInChildren<FormStarScript>();

		AssignParentFormAndQuestion();

		//Initially record all answers as empty
		parentForm.RecordAnswer(myQuestion, "", myLowOption, myHighOption);

        Deselect();
	}

	void AssignParentFormAndQuestion()
    {
        //With my index as answer -> save the answer to the form script

        Transform myExplanationTransform = transform.parent.parent.parent.Find("Explanation");
        if (myExplanationTransform == null) 
        { 
            Debug.LogError("No explanation object found under 3x parent -> won't record answer"); 
            return; 
        }
		TextMeshProUGUI parentExplanation = myExplanationTransform.GetComponent<TextMeshProUGUI>();
		if (parentExplanation == null)
		{
			Debug.LogError("Explanation object does not have TMPro, while it should be the question! -> won't record answer");
			return;
		}
		myQuestion = parentExplanation.text;

		Transform myLowOptionTransform = transform.parent.parent.Find("TextLow");
		if (myLowOptionTransform == null)
		{
			Debug.LogError("No TextLow object found under 2x parent -> won't record answer");
			return;
		}
		TextMeshProUGUI parentLowOption = myLowOptionTransform.GetComponent<TextMeshProUGUI>();
		if (parentLowOption == null)
		{
			Debug.LogError("Found TextLow object does not have TMPro, while it should be the left description! -> won't record answer");
			return;
		}
		myLowOption = parentLowOption.text;

		Transform myHighOptionTransform = transform.parent.parent.Find("TextHigh");
		if (myHighOptionTransform == null)
		{
			Debug.LogError("No TextLow object found under 2x parent -> won't record answer");
			return;
		}
		TextMeshProUGUI parentHighOption = myHighOptionTransform.GetComponent<TextMeshProUGUI>();
		if (parentHighOption == null)
		{
			Debug.LogError("Found TextHigh object does not have TMPro, while it should be the left description! -> won't record answer");
			return;
		}
		myHighOption = parentHighOption.text;

		FormScript parentFormScript = transform.parent.parent.parent.parent.parent.GetComponent<FormScript>();
		if (parentFormScript == null)
		{
			Debug.LogError("5xParent of star does not have FormScript! -> won't record answer");
			return;
		}
        parentForm = parentFormScript;

	}


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && mouseOver)
            Clicked();
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
        Hover();

		// Activate all the stars up to and including this one
		foreach (FormStarScript star in allStarsForThisQuestion)
        {
            if (star == this)
                break;
            star.ActivateSprite();
		}
	}

    public void OnPointerExit(PointerEventData eventData)
    {
        UnHover();

        DeactivateUnselectedSprites();
	}

    public void DeactivateUnselectedSprites()
    {
        foreach (FormStarScript star in allStarsForThisQuestion)
        {
            if (!star.selected)
                star.DeactivateSprite();
        }
    }

    public void ActivateSprite()
    {
		if (myImage != null)
			myImage.sprite = activeSprite;
	}

    public void DeactivateSprite()
    {
        if (myImage != null)
            myImage.sprite = dormantSprite;
	}

    void Hover()
    {
        ActivateSprite();
		mouseOver = true;
	}

    void UnHover()
    {
        mouseOver = false;
        if (selected)
            return;
        DeactivateSprite();
	}

    public void Select()
    {
        selected = true;
        ActivateSprite();
    }


	public void Clicked()
    {
        bool previous = true;
        int myRating = 0;
        foreach (FormStarScript star in allStarsForThisQuestion)
        {
            if (previous)
            {
                star.Select();
                myRating++;
            }
            else
                star.Deselect();
			if (star == this)
                previous = !previous;
		}

        Debug.Log($"Recording answer to question {myQuestion}: {myLowOption}_{myHighOption} with rating {myRating}");
        parentForm.RecordAnswer(myQuestion, myRating.ToString(), myLowOption, myHighOption);

	}

	public void Deselect()
    {
        selected = false;
        DeactivateSprite();
	}
}
