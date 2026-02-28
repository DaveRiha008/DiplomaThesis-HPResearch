using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	//Animation parameter IDs
	const string animRollingTriggerID = "RollTrigger";
	const string animRollingSpeedID = "RollSpeed";
	const string animXMoveID = "XMove";
	const string animYMoveID = "YMove";
	const string animAttackingTriggerID = "AttackTrigger";
	const string animAttackSpeedID = "AttackSpeed";
	const string animAttackDirXID = "AttackDirX";
	const string animAttackDirYID = "AttackDirY";


	[Header("Movement variables")]
	public int moveSpeed = 10;

	//Small offset to avoid getting stuck in walls due to precision errors
	[SerializeField]
	float collisionOffset = 0.05f;


	[SerializeField]
	ContactFilter2D movementFilter;

	/// <summary>
	/// The velocity the player is currently actually moving with
	/// </summary>
	public Vector2 currentVelocity = Vector2.zero;
	/// <summary>
	/// The velocity the player is trying to move with based on input (this can be different from currentVelocity if there are obstacles)
	/// </summary>
	public Vector2 currentInputMoveVector = Vector2.zero;

	[Header("Roll variables")]
	/// <summary>
	/// Duration of the roll animation and movement (next roll instantly available)
	/// </summary>
	[SerializeField]
	float rollDuration = 0.5f;
	/// <summary>
	/// Duration of the invincibility in the roll (should be lower than rollDuration)
	/// </summary>
	[SerializeField]
	float eyeFrameDuration = 0.25f;

	bool isRolling = false;
	bool isInvincibleInRoll = false;

	float rollStarted = 0f;
	[SerializeField]
	float rollDistance = 5f;
	[SerializeField]
	[Tooltip("Color to change to after eyeFrames")]
	Color rollColorChange = Color.cyan;
	[SerializeField]
	[Tooltip("Color of the eye frame effect")]
	Color eyeFrameColorChange = Color.green;

	[Header("Combat variables")]

	[SerializeField]
	[Tooltip("Exact duration of the attack animation")]
	float attackDuration = 0.2f;
	float attackStarted = 0f;
	float attackCooldown = 0.2f;
	float attackLastUsed = -10f;
	bool isAttacking = false;
	[SerializeField]
	int attackDamage = 1;
	[SerializeField]
	float maxHP = 10;
	float currentHP;
	Vector3 respawnLocation;
	/// <summary>List of enemies following the player -> defines combat</summary>
	public List<EnemyController> enemiesFollowing = new();
	public bool IsInCombat { get => enemiesFollowing.Count > 0; }

	[Header("Healing variables")]
	[SerializeField]
	int maxHealItemCount = 3;
	int currentHealItemCount = 0;
	int healAmountFromHealItem = 5;
	float healItemUseDuration = 0.5f;
	bool isUsingHealItem = false;
	float rallyDuration = .5f;
	float rallyStarted = 0f;
	bool isRallyActive = false;
	float rallyHealAmount = 1f;

	[SerializeField]
	float healthRegenPerSecond = .5f;

	[SerializeField]
	Sprite usingHealItemSprite;

	[Header("Leveling variables")]
	int experiencePoints = 0;
	int currentLevel = 0;
	[SerializeField]
	[Tooltip("Max HP increase per level")]
	int HPPerLevel = 5;
	[SerializeField]
	[Tooltip("Attack damage increase per level")]
	int attackDamagePerLevel = 1;
	[SerializeField]
	[Tooltip("Movement speed increase per level")]
	int moveSpeedPerLevel = 1;

	Color origSpriteColor;

	//Input actions
	InputAction moveAction;
	InputAction rollAction;
	InputAction attackAction;
	InputAction useHealAction;

	//Cached components
	Rigidbody2D rb;
	SpriteRenderer spriteRenderer;
	Animator animator;

	[Header("References to objects")]
	//Cached object references
	[SerializeField]
	[Tooltip("Reference to the PlayerSwordScript attached to the player's sword object")]
	PlayerSwordScript playerSword;
	CameraFollowPlayer cameraFollow;

	//Rigidbody cast hits list
	List<RaycastHit2D> moveCastHits = new();

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		moveAction = InputSystem.actions.FindAction(GlobalConstants.moveInputActionName);
		rollAction = InputSystem.actions.FindAction(GlobalConstants.rollInputActionName);
		attackAction = InputSystem.actions.FindAction(GlobalConstants.attackInputActionName);
		useHealAction = InputSystem.actions.FindAction(GlobalConstants.useHealInputActionName);

		rb = gameObject.GetComponent<Rigidbody2D>();
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		animator = gameObject.GetComponent<Animator>();

		cameraFollow = Camera.main.GetComponent<CameraFollowPlayer>();
		playerSword.gameObject.SetActive(false);

		origSpriteColor = spriteRenderer.color;

		respawnLocation = transform.position;

		currentHP = maxHP;
		currentHealItemCount = maxHealItemCount;

		this.CallWithDelay(() =>
			HUD.Instance.UpdateHealthBar(currentHP, maxHP), 
			.1f);
	}

	// Update is called once per frame
	void Update()
	{
		//Rolling check
		if (isRolling)
			RollUpdate();
		else if (rollAction.triggered)
			InitiateRoll();

		//Attack check
		if (isAttacking)
			AttackUpdate();
		else if (attackAction.triggered)
			InitiateAttack();

		//Heal item use check
		if (useHealAction.triggered)
			UseHealItem();

		//Passive health regen approach
		if (GameManager.Instance.CurHPRegenApproach == HPRegenApproach.OverTime && !IsInCombat)
		{
			Heal(healthRegenPerSecond * Time.deltaTime, silent:true);
		}
		//Bloodborne rally approach
		if (GameManager.Instance.CurHPRegenApproach == HPRegenApproach.BloodBorneRally)
			UpdateRally();
		else if (isRallyActive)
			EndRally();



		//Move every frame -> idle if nothing is pressed
		Move(moveAction.ReadValue<Vector2>());

	}

	#region MOVEMENT

	void Move(Vector2 inputMove)
	{
		currentInputMoveVector = inputMove;

		animator.SetFloat(animXMoveID, inputMove.x);
		animator.SetFloat(animYMoveID, inputMove.y);


		if (isRolling || isAttacking || isUsingHealItem) return;

		Vector2 moveVector = GetPossibleMovement(inputMove, moveSpeed * Time.fixedDeltaTime);

		//Actually move the RB
		Vector2 moveVectorFinal = moveVector * moveSpeed * Time.fixedDeltaTime;

		rb.MovePosition(rb.position + moveVectorFinal);

		currentVelocity = moveVectorFinal;
	}

	bool CanMovePlayer(Vector2 moveVec, float speed)
	{
		return CanMovePlayer(moveVec, speed, out _);
	}

	bool CanMovePlayer(Vector2 moveVec, float speed, out float nearestHitPoint)
	{
		//Cast in the moveVec
		int collisionCount = rb.Cast(
			 moveVec, //Direction
			 movementFilter, //Filter -> such as layer mask (set in the inspector)
			 moveCastHits, //The output list
			 speed + collisionOffset //The cast offset
			 );

		if (collisionCount > 0)
		{
			nearestHitPoint = moveCastHits[0].fraction;
			return false;
		}

		nearestHitPoint = 1;
		return true;
	}

	Vector2 GetPossibleMovement(Vector2 moveInput, float speed)
	{
		//Check if you can move in the diagonal, horizontal and vertical direction (if there is not an obstacle)
		//Check seperately to ensure movement in a situation where you hold up and right and can't go right, you still go up

		float nearestHitXFraction;
		float nearestHitYFraction;
		float nearestHitDiagFraction;


		bool canMoveInX = CanMovePlayer(new Vector2(moveInput.x, 0), speed, out nearestHitXFraction);
		bool canMoveInY = CanMovePlayer(new Vector2(0, moveInput.y), speed, out nearestHitYFraction);
		bool canMoveFull = CanMovePlayer(moveInput, speed, out nearestHitDiagFraction);

		//Debug.Log($"Nearest X hit fraction: {nearestHitXFraction}");
		//Debug.Log($"Nearest Y hit fraction: {nearestHitYFraction}");
		//Debug.Log($"Nearest Diag hit fraction: {nearestHitDiagFraction}");

		Vector2 moveVector = Vector2.zero;

		//If the way is clear, just go with it
		if (canMoveFull)
			moveVector = moveInput;

		//Else if we can move a noticeable distance in the intended way, go at least the part of it
		else if (nearestHitDiagFraction * speed > collisionOffset)
			moveVector = (nearestHitDiagFraction - collisionOffset) * moveInput;

		else
		{
			//Seperate movement in the axis -> for the situation explained above
			//If the move in x is bigger and big enough, move horizontally
			if (nearestHitXFraction > nearestHitYFraction && nearestHitXFraction * speed > collisionOffset)
				moveVector.x = (nearestHitXFraction - collisionOffset) * moveInput.x;
			//If move in y is big enough, move vertically
			else if (nearestHitYFraction * speed > collisionOffset)
				moveVector.y = (nearestHitYFraction - collisionOffset) * moveInput.y;
		}

		//Stuck in place --> just move a bit in the opposite direction
		if (nearestHitXFraction == 0 && nearestHitYFraction == 0 && nearestHitDiagFraction == 0)
			moveVector = -0.01f * moveInput;

		return moveVector;
	}

	#endregion MOVEMENT

	#region ROLLING
	void InitiateRoll()
	{
		if (isAttacking || isUsingHealItem)
			return;

		isRolling = true;
		isInvincibleInRoll = true;
		rollStarted = Time.time;

		HUD.Instance.StartRollCDBarRegen(rollDuration);

		animator.SetFloat(animRollingSpeedID, 1 / (rollDuration / 0.333f)); //0.333f is the duration of the original roll animation
		animator.SetTrigger(animRollingTriggerID);

		Vector2 possMove = GetPossibleMovement(currentInputMoveVector, rollDistance);

		rb.DOMove(rb.position + possMove * rollDistance, rollDuration);


		spriteRenderer.color = eyeFrameColorChange;
		spriteRenderer.DOBlendableColor(rollColorChange, eyeFrameDuration).SetLink(gameObject);
	}

	void RollUpdate()
	{
		//If eyeFrames run out -> disable invincibility
		if (isInvincibleInRoll && Time.time - rollStarted >= eyeFrameDuration)
			EndOfRollEyeFrames();
		//If roll duration passed, stop rolling
		if (Time.time - rollStarted >= rollDuration)
			StopRolling();
	}

	void EndOfRollEyeFrames()
	{
		isInvincibleInRoll = false;
		spriteRenderer.DOBlendableColor(origSpriteColor, rollDuration).SetLink(gameObject);
	}

	void StopRolling()
	{
		isRolling = false;
		animator.SetBool(animRollingTriggerID, false);

	}
	#endregion ROLLING

	#region COMBAT

	void InitiateAttack()
	{
		if (isRolling || isUsingHealItem)
			return;

		if (Time.time - attackLastUsed < attackCooldown)
			return;

		//Set attack direction (based on movement input or mouse position)
		Vector2 attackDir = currentInputMoveVector.normalized;

		if (cameraFollow != null)
			attackDir = cameraFollow.currentMouseOffset.normalized;

		if (playerSword != null)
		{
			playerSword.gameObject.SetActive(true);
			playerSword.AnimateAttack(1 / (attackDuration / 0.417f), attackDir);
		}

		//Set animator parameters
		animator.SetFloat(animAttackDirXID, attackDir.x);
		animator.SetFloat(animAttackDirYID, attackDir.y);
		animator.SetFloat(animAttackSpeedID, 1 / (attackDuration / 0.417f)); //0.417f is the duration of the original attack animation
		animator.SetTrigger(animAttackingTriggerID);

		isAttacking = true;
		attackStarted = Time.time;

		HUD.Instance.StartAttackCDBarRegen(attackCooldown);
	}

	void AttackUpdate()
	{
		if (Time.time - attackStarted >= attackDuration)
			AttackEnd();
	}

	public void EnemyHit(EnemyController enemy)
	{
		bool killed = enemy.GetHit(attackDamage);

		if (isRallyActive)
		{
			Heal(rallyHealAmount);
		}

		//If the enemy was killed, add experience and possibly get heal item if that is the current approach
		if (killed)
		{
			AddExperience(enemy.experienceOnDeath);
			
			if (GameManager.Instance.CurHPRegenApproach == HPRegenApproach.BloodborneItems)
				if (Random.Range(0f, 1f) <= enemy.healItemDropChance)
					AddHealItem();
		}
	}

	void AttackEnd()
	{
		if (playerSword != null)
			playerSword.gameObject.SetActive(false);

		isAttacking = false;
		attackLastUsed = Time.time;
	}


	public void GetHit(int damage)
	{
		if (isInvincibleInRoll)
			return;

		currentHP -= damage;

		if (currentHP <= 0)
		{
			Die();
			return;
		}

		if (GameManager.Instance.CurHPRegenApproach == HPRegenApproach.BloodBorneRally)
			StartRally();

		HUD.Instance.UpdateHealthBar(currentHP, maxHP);

		spriteRenderer.DOKill();
		spriteRenderer.color = Color.red;
		spriteRenderer.DOBlendableColor(origSpriteColor, 0.1f).SetLink(gameObject);

		UIFlashingElements.ShowFlashingText(transform, $"-{damage.ToString()}", Color.red);
	}

	void Die()
	{
		FullHeal();
		RestoreHealItems();
		rb.position = respawnLocation;
		GameManager.Instance.RespawnAllEnemies();

		spriteRenderer.color = spriteRenderer.color.WithAlpha(0);
		spriteRenderer.DOColor(origSpriteColor.WithAlpha(1), .8f);
	}

	#endregion COMBAT

	#region LEVELING

	public void AddExperience(int exp)
	{
		experiencePoints += exp;

		if (currentLevel < ExperienceLevelThresholds.thresholds.Length)
			HUD.Instance.UpdateXPBar(experiencePoints / (float)ExperienceLevelThresholds.thresholds[currentLevel]);

		//Only show the exp fly if not leveled up
		UIFlashingElements.ShowFlashingText(transform, $"+{exp}xp", Color.yellow);

	}

	public bool CanLevelUp()
	{
		if (currentLevel >= ExperienceLevelThresholds.thresholds.Length)
			return false;
		int newLevelThreshold = ExperienceLevelThresholds.thresholds[currentLevel];
		return experiencePoints >= newLevelThreshold;
	}

	public bool LevelUpIfPossible()
	{
		if (currentLevel >= ExperienceLevelThresholds.thresholds.Length)
			return false;
		int newLevelThreshold = ExperienceLevelThresholds.thresholds[currentLevel];
		if (experiencePoints >= newLevelThreshold)
		{
			LevelUp();
			return true;
		}
		return false;
	}

	void LevelUp()
	{
		//Add all the level up benefits
		currentLevel++;
		maxHP += HPPerLevel;
		currentHP = Mathf.Min(currentHP + HPPerLevel, maxHP);
		attackDamage += attackDamagePerLevel;
		moveSpeed += moveSpeedPerLevel;

		LevelUpUpdateHUD();

		LevelUpEffect();

	}

	void LevelUpUpdateHUD()
	{
		HUD.Instance.UpdateHealthBar(currentHP, maxHP);
		if (currentLevel < ExperienceLevelThresholds.thresholds.Length)
		{
			HUD.Instance.UpdateXPBar(experiencePoints / (float)ExperienceLevelThresholds.thresholds[currentLevel]);
			if (CanLevelUp())
				HUD.Instance.ShowControlsPopUp(HUD.ControlsPopUpType.LevelUp);
			else
				HUD.Instance.HideControlsPopUp(HUD.ControlsPopUpType.LevelUp);
		}
		else
			HUD.Instance.HideControlsPopUp(HUD.ControlsPopUpType.LevelUp);
		HUD.Instance.UpdateLevelLabel(currentLevel+1);
	}

	void LevelUpEffect()
	{
		//Flash yellow
		spriteRenderer.DOKill();
		spriteRenderer.color = Color.yellow;
		spriteRenderer.DOBlendableColor(origSpriteColor, 0.5f).SetLink(gameObject);

		//Scale up and back down
		transform.DOKill();
		transform.DOScale(1.5f, 0.25f).SetLink(gameObject)
			.OnComplete(() =>
			{
				transform.DOScale(1f, 0.25f).SetLink(gameObject);
			});

		//Show level up number
		UIFlashingElements.ShowFlashingText(transform, $"Level {currentLevel + 1}", Color.yellow, Vector2.up * .5f, Vector2.up * 2, 10, .1f, 1.5f);

		//Show hp increase number
		this.CallWithDelay(() =>
		{
			UIFlashingElements.ShowFlashingText(transform, $"+{HPPerLevel} maxHP", Color.green, Vector2.right * .5f, Vector2.up * 1f, 5, .1f, 1f);
		}
		, .3f);

		//Show attack damage increase number
		this.CallWithDelay(() =>
		{
			UIFlashingElements.ShowFlashingText(transform, $"+{attackDamagePerLevel} Dmg", new Color(1, .5f, 0), Vector2.left * .5f, Vector2.up * 1f, 5, .1f, 1f);
		}, .6f);

		//Show move speed increase number
		this.CallWithDelay(() =>
		{
			UIFlashingElements.ShowFlashingText(transform, $"+{moveSpeedPerLevel} Speed", Color.cyan, Vector2.down * .5f, Vector2.up * 1f, 5, .1f, 1f);
		}, .9f);
	}

	#endregion

	#region HEALING

	public void Heal(float healAmount, bool silent = false)
	{
		currentHP = Mathf.Min(currentHP + healAmount, maxHP);
		HUD.Instance.UpdateHealthBar(currentHP, maxHP);
		//Show heal amount
		if (!silent)
			UIFlashingElements.ShowFlashingText(transform, $"+{healAmount}", Color.green);
	}

	public void FullHeal()
	{
		currentHP = maxHP;
		HUD.Instance.UpdateHealthBar(currentHP, maxHP);
	}

	public void RestAtCheckpoint()
	{
		FullHeal();

		if (GameManager.Instance.CurHPRegenApproach == HPRegenApproach.DarkSoulsItems)
			RestoreHealItems();
		
		respawnLocation = transform.position;
	}

	public bool CanUseHealItem()
	{
		//Is already doing another action
		if (isRolling || isAttacking || isUsingHealItem)
			return false;

		//Not using an approach with heal items
		if (GameManager.Instance.CurHPRegenApproach != HPRegenApproach.BloodborneItems &&
			GameManager.Instance.CurHPRegenApproach != HPRegenApproach.DarkSoulsItems)
			return false;

		return currentHealItemCount > 0;
	}

	public void UseHealItem()
	{
		if (!CanUseHealItem())
			return;

		if (currentHealItemCount <= 0)
			return;
		currentHealItemCount--;
		HUD.Instance.RemoveHealItem();
		Heal(healAmountFromHealItem);

		isUsingHealItem = true;
		TweenUseHealItem();
	}

	void TweenUseHealItem()
	{
		animator.enabled = false;
		spriteRenderer.sprite = usingHealItemSprite;
		spriteRenderer.DOKill();
		spriteRenderer.color = Color.green;
		spriteRenderer.DOBlendableColor(origSpriteColor, healItemUseDuration)
			.SetLink(gameObject)
			.SetEase(Ease.OutFlash);
		this.CallWithDelay(() =>
		{
			isUsingHealItem = false;
			animator.enabled = true;
		}, 
		healItemUseDuration);
	}

	public void AddHealItem()
	{
		if (currentHealItemCount >= maxHealItemCount)
			return;
		currentHealItemCount++;
		HUD.Instance.AddHealItem();

		UIFlashingElements.ShowFlashingSprite(transform, HUD.Instance.CurHealItemSprite, Vector2.zero, Vector2.up * 1.5f, Vector3.one * 0.75f);
	}

	public void RestoreHealItems()
	{
		currentHealItemCount = maxHealItemCount;
		HUD.Instance.UpdateHealItemCount(maxHealItemCount);
	}

	void StartRally()
	{
		rallyStarted = Time.time;
		HUD.Instance.ShowRallyIcon();
		isRallyActive = true;
	}

	void UpdateRally()
	{

		float elapsed = Time.time - rallyStarted;
		if (elapsed >= rallyDuration)
		{
			EndRally();
		}
		else
		{
			float alpha = 1 - .33f*(elapsed / rallyDuration);
			HUD.Instance.UpdateRallyIcon(alpha);
		}
	}

	void EndRally()
	{
		rallyStarted = 0f;
		HUD.Instance.HideRallyIcon();
		isRallyActive = false;
	}

	#endregion
	#region Environment interaction
	public void DestructibleObjectDestroyed(DestructibleScript destroyedObject)
	{
		if (GameManager.Instance.CurHPRegenApproach != HPRegenApproach.BloodborneItems)
			return;

		float randomFloat = Random.Range(0f, 1f);
		if (randomFloat <= destroyedObject.chanceToDropHealItem)
		{
			AddHealItem();
		}
	}
	#endregion

	public PlayerStats GetPlayerStats()
	{
		return new PlayerStats(
			curHealth: Mathf.FloorToInt(currentHP),
			maxHealth: Mathf.FloorToInt(maxHP),
			curExp: experiencePoints,
			goalExp: currentLevel < ExperienceLevelThresholds.thresholds.Length ? ExperienceLevelThresholds.thresholds[currentLevel] : experiencePoints,
			attack: attackDamage,
			moveSpeed: moveSpeed
			);
	}
}
