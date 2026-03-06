using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class FormStarScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Image myImage;

    //This star has the mouse over it
    bool mouseOver = false;

    //This star was selected -> will stay active until a lower star is clicked
    public bool selected = false;


    FormStarScript[] otherStars;

	[SerializeField] Sprite activeSprite;
    [SerializeField] Sprite dormantSprite;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        myImage = GetComponent<Image>();
        otherStars = transform.parent.GetComponentsInChildren<FormStarScript>();
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
		foreach (FormStarScript star in otherStars)
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
        foreach (FormStarScript star in otherStars)
        {
            if (!star.selected)
                star.DeactivateSprite();
        }
    }

    public void ActivateSprite()
    {
        myImage.sprite = activeSprite;
	}

    public void DeactivateSprite()
    {
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
        myImage.sprite = dormantSprite;
	}

    public void Select()
    {
        selected = true;
        myImage.sprite = activeSprite;
    }


	public void Clicked()
    {
        bool previous = true;
        foreach (FormStarScript star in otherStars)
        {
            if (previous)
                star.Select();
            else
                star.Deselect();
			if (star == this)
                previous = !previous;
		}
    }

	public void Deselect()
    {
        selected = false;
        myImage.sprite = dormantSprite;
	}
}
