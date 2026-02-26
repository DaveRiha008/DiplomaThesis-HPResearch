using UnityEngine;
using UnityEngine.InputSystem;

public class CampfireScript : MonoBehaviour
{
    bool playerInRange = false;

    PlayerController player;

    InputAction restAction;
    InputAction levelUpAction;

    [SerializeField] AnimationClip campfireAnimation;
    [SerializeField] AnimationClip highlightedCampfireAnimation;

    Animator animator;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        restAction = InputSystem.actions.FindAction(GlobalConstants.interactInputActionName);    
        levelUpAction = InputSystem.actions.FindAction(GlobalConstants.interact2InputActionName);    

        animator = GetComponent<Animator>();
	}

    // Update is called once per frame
    void Update()
    {

        //TODO LevelUp is displayed in tooltip and available only after rest is triggered
        if (playerInRange)
        {
            if (restAction.WasPressedThisFrame())
                Rest();
            if (levelUpAction.WasPressedThisFrame())
                LevelUp();
		}
	}

    void PlayerEntered()
    {
        playerInRange = true;

        //UI Instruction to press E to rest
		HUD.Instance.ShowControlsPopUp(HUD.ControlsPopUpType.Heal);

		//UI Instruction to press F to level up
        if (player.CanLevelUp())
        {
            HUD.Instance.ShowControlsPopUp(HUD.ControlsPopUpType.LevelUp);
		}

        animator.Play(highlightedCampfireAnimation.name);
	}

    void PlayerLeft()
    {
		playerInRange = false;
        HUD.Instance.HideControlsPopUp(HUD.ControlsPopUpType.Heal);
        HUD.Instance.HideControlsPopUp(HUD.ControlsPopUpType.LevelUp);

        animator.Play(campfireAnimation.name);
	}

	void Rest()
    {
        if (player == null)
        {
            Debug.LogError("Cannot rest at checkpoint, because the player is null!");
            return;
		}

        player.RestAtCheckpoint();
        GameManager.Instance.RespawnAllEnemies();
        GameManager.Instance.RespawnAllDestructible();
	}

    void LevelUp()
    {
		if (player == null)
		{
			Debug.LogError("Cannot levelUp at checkpoint, because the player is null!");
			return;
		}

		player.LevelUpIfPossible();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent(out player))
            {
			    PlayerEntered();
			}
        }
	}

	private void OnTriggerExit2D(Collider2D collision)
    {
		if (collision.CompareTag("Player"))
		{
            PlayerLeft();
		}
	}

}
