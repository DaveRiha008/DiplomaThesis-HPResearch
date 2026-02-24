using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoSingleton<HUD>
{

    int maxHealthThreshold = 50;

    [Header("References")]
	[SerializeField] Slider healthBar;
	[SerializeField] Slider attackCDBar;
	[SerializeField] Slider rollCDBar;
    [SerializeField] Slider xpBar;
    [SerializeField] TextMeshProUGUI levelLabel;
    [SerializeField] TextMeshProUGUI healItemCountLabel;
    [SerializeField] GameObject controlsPopUpParent;


	/// <summary>
	/// Dictionary to store the names of the controls pop-ups, so that they can be easily accessed and changed in one place if needed.
	/// </summary>
	Dictionary<ControlsPopUpType, string> controlsPopUpNames = new ()
    {
        {ControlsPopUpType.Heal, "E to Heal" },
        {ControlsPopUpType.LevelUp, "F to Level Up" }
    };
	/// <summary>
	/// All the types of controls that can be shown in the pop-up in HUD
	/// </summary>
	public enum ControlsPopUpType
    {
        Heal,
        LevelUp
    }


    public void UpdateHealthBar(float curHealth, float maxHealth)
    {
        healthBar.value = curHealth/maxHealth;
        healthBar.gameObject.GetComponent<RectTransform>().anchorMax = new Vector2(maxHealth/(float)maxHealthThreshold, 1);
	}

    public void StartAttackCDBarRegen(float regenDuration)
    {
        attackCDBar.value = 0;
        attackCDBar.DOValue(1, regenDuration);
	}
    public void StartRollCDBarRegen(float regenDuration)
    {
        rollCDBar.value = 0;
        rollCDBar.DOValue(1, regenDuration);
	}

    public void UpdateXPBar(float xpPercentage)
    {
        xpBar.value = xpPercentage;
    }
    public void UpdateLevelLabel(int level)
    {
        levelLabel.text = $"{level}";
    }
    public void UpdateHealItemCount(int count)
    {
        healItemCountLabel.text = $"{count}";
    }
    public void AddHealItem()
    {
        healItemCountLabel.text = $"{int.Parse(healItemCountLabel.text) + 1}";
	}
    public void RemoveHealItem()
    {
        healItemCountLabel.text = $"{int.Parse(healItemCountLabel.text) - 1}";
    }

	public void ShowControlsPopUp(ControlsPopUpType type)
    {
        string popUpName = controlsPopUpNames[type];
        Transform popUpTransform = controlsPopUpParent.transform.Find(popUpName);
        if (popUpTransform != null)
        {
            popUpTransform.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError($"No pop-up found with the name {popUpName}");
        }
    }
    public void HideControlsPopUp(ControlsPopUpType type)
    {
        string popUpName = controlsPopUpNames[type];
        Transform popUpTransform = controlsPopUpParent.transform.Find(popUpName);
        if (popUpTransform != null)
        {
            popUpTransform.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError($"No pop-up found with the name {popUpName}");
        }
	}
}
