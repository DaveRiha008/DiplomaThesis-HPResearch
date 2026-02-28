using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HUD : MonoSingleton<HUD>
{

	int maxHealthThreshold = 50;

	[Header("References")]
	[Header("HP")]
	[SerializeField] Slider healthBar;
	[SerializeField] HeartGroupUI heartGroup;
	[SerializeField] BloodyScreens bloodyScreens;
	[SerializeField] Image rallyIcon;
	[SerializeField] GameObject healItemDS;
	[SerializeField] GameObject healItemBB;
	[SerializeField] TextMeshProUGUI healItemCountLabelDS;
	[SerializeField] TextMeshProUGUI healItemCountLabelBB;
	public Sprite CurHealItemSprite { get 
		{
			if (GameManager.Instance.CurHPRegenApproach == HPRegenApproach.DarkSoulsItems) return healItemDS.GetComponent<Image>().sprite;
			else if (GameManager.Instance.CurHPRegenApproach == HPRegenApproach.BloodborneItems) return healItemBB.GetComponent<Image>().sprite;
			else return null;
		} }

	[Header("CD")]
	[SerializeField] Slider attackCDBar;
	[SerializeField] Slider rollCDBar;

	[Header("XP")]
	[SerializeField] Slider xpBar;
	[SerializeField] TextMeshProUGUI levelLabel;

	[Header("Controls")]
	[SerializeField] GameObject controlsPopUpParent;
	[SerializeField] GameObject controls;
	InputAction toggleOptionsAction;

	[Header("Menu")]
	[SerializeField] GameObject menu;
	[SerializeField] PlayerStatsPanel playerStatsPanel;

	/// <summary>
	/// Dictionary to store the names of the controls pop-ups, so that they can be easily accessed and changed in one place if needed.
	/// </summary>
	Dictionary<ControlsPopUpType, string> controlsPopUpNames = new()
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

	private void Start()
	{
		GameManager.Instance.onHPShowApproachChange.AddListener(HPShowApproachChange);
		GameManager.Instance.onHPRegenApproachChange.AddListener(HPRegenApproachChange);
		HPShowApproachChange();
		HPRegenApproachChange();

		toggleOptionsAction = InputSystem.actions.FindAction(GlobalConstants.toggleOptionsInputActionName);
	}

	private void Update()
	{
		if (toggleOptionsAction.WasPressedThisFrame())
			ToggleControls();
	}

	void HPShowApproachChange()
	{
		switch (GameManager.Instance.CurHPShowApproach)
		{
			case HPShowApproach.EldenRing:
				healthBar.gameObject.SetActive(true);
				heartGroup.gameObject.SetActive(false);
				bloodyScreens.gameObject.SetActive(false);
				break;
			case HPShowApproach.HollowKnight:
				healthBar.gameObject.SetActive(false);
				heartGroup.gameObject.SetActive(true);
				bloodyScreens.gameObject.SetActive(false);
				break;
			case HPShowApproach.SilentHill:
				healthBar.gameObject.SetActive(false);
				heartGroup.gameObject.SetActive(false);
				bloodyScreens.gameObject.SetActive(true);
				break;
			default:
				Debug.LogError("Invalid HP show approach");
				break;
		}
	}

	void HPRegenApproachChange()
	{
		switch (GameManager.Instance.CurHPRegenApproach)
		{
			case HPRegenApproach.OverTime:
			case HPRegenApproach.BloodBorneRally:
			case HPRegenApproach.PickUp:
				healItemDS.SetActive(false);
				healItemBB.SetActive(false);
				break;
			case HPRegenApproach.DarkSoulsItems:
				healItemDS.SetActive(true);
				healItemBB.SetActive(false);
				break;
			case HPRegenApproach.BloodborneItems:
				healItemDS.SetActive(false);
				healItemBB.SetActive(true);
				break;
			default:
				Debug.LogError("Invalid HP regen approach");
				break;
		}
	}


	#region HP Show Approaches
	public void UpdateHealthBar(float curHealth, float maxHealth)
	{
		//Elden ring approach
		if (GameManager.Instance.CurHPShowApproach == HPShowApproach.EldenRing)
		{
			healthBar.value = curHealth / maxHealth;
			healthBar.gameObject.GetComponent<RectTransform>().anchorMax = new Vector2(maxHealth / (float)maxHealthThreshold, 1);
		}

		//Hollow knight approach
		else if (GameManager.Instance.CurHPShowApproach == HPShowApproach.HollowKnight)
			heartGroup.UpdateHealth((int)curHealth, (int)maxHealth);

		//Silent Hill approach
		else if (GameManager.Instance.CurHPShowApproach == HPShowApproach.SilentHill)
			bloodyScreens.UpdateHealth(curHealth, maxHealth);

	}

	#endregion


	#region HP Regen Approaches
	public void UpdateHealItemCount(int count)
	{
		if (GameManager.Instance.CurHPRegenApproach == HPRegenApproach.DarkSoulsItems)
			healItemCountLabelDS.text = $"{count}";
		else if (GameManager.Instance.CurHPRegenApproach == HPRegenApproach.BloodborneItems)
			healItemCountLabelBB.text = $"{count}";
	}
	public void AddHealItem()
	{
		if (GameManager.Instance.CurHPRegenApproach == HPRegenApproach.DarkSoulsItems)
			healItemCountLabelDS.text = $"{int.Parse(healItemCountLabelDS.text) + 1}";
		else if (GameManager.Instance.CurHPRegenApproach == HPRegenApproach.BloodborneItems)
			healItemCountLabelBB.text = $"{int.Parse(healItemCountLabelBB.text) + 1}";
	}
	public void RemoveHealItem()
	{
		if (GameManager.Instance.CurHPRegenApproach == HPRegenApproach.DarkSoulsItems)
			healItemCountLabelDS.text = $"{int.Parse(healItemCountLabelDS.text) - 1}";
		else if (GameManager.Instance.CurHPRegenApproach == HPRegenApproach.BloodborneItems)
			healItemCountLabelBB.text = $"{int.Parse(healItemCountLabelBB.text) - 1}";
	}

	#endregion

	#region Other UI Elements
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

	public void ShowRallyIcon()
	{
		rallyIcon.color = rallyIcon.color.WithAlpha(1);
	}

	public void UpdateRallyIcon(float alpha)
	{
		rallyIcon.color = rallyIcon.color.WithAlpha(alpha);
	}

	public void HideRallyIcon()
	{
		rallyIcon.color = rallyIcon.color.WithAlpha(0);
	}

	void ToggleControls()
	{
		if (controls.activeSelf)
			HideControls();
		else
			ShowControls();
	}

	void ShowControls()
	{
		controls.SetActive(true);
	}
	void HideControls()
	{
		controls.SetActive(false);
	}

	public void ToggleMenu()
	{
		if (menu.activeSelf)
		{
			HideMenu(); ;
		}
		else
		{
			ShowMenu();
		}
	}
	public void ShowMenu()
	{
		ShowControls();
		playerStatsPanel.MenuShown();
		menu.SetActive(true);
	}

	public void HideMenu()
	{
		HideControls();
		menu.SetActive(false);
	}
	#endregion
}
