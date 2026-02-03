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
	public Vector2 currentInputMoveVector= Vector2.zero;

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
    Color rollColorChange = Color.cyan;
    [SerializeField]
    Color eyeFrameColorChange = Color.green;

    [Header("Attack variables")]

    [SerializeField]
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



	Color origSpriteColor;

	//Input actions
	InputAction moveAction;
    InputAction rollAction;
    InputAction attackAction;

	//Cached components
	Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    Animator animator;

    [Header("References to objects")]
    //Cached object references
    [SerializeField]
	PlayerSwordScript playerSword;
    CameraFollowPlayer cameraFollow;

	//Rigidbody cast hits list
	List<RaycastHit2D> moveCastHits = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        rollAction = InputSystem.actions.FindAction("Roll");
        attackAction = InputSystem.actions.FindAction("Attack");

		rb = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        animator = gameObject.GetComponent<Animator>();

        cameraFollow = Camera.main.GetComponent<CameraFollowPlayer>();
        playerSword.gameObject.SetActive(false);

		origSpriteColor = spriteRenderer.color;

        currentHP = maxHP;

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

		//Move every frame -> idle if nothing is pressed
		Move(moveAction.ReadValue<Vector2>());

	}

	#region MOVEMENT

	void Move(Vector2 inputMove)
    {
        currentInputMoveVector = inputMove;

        animator.SetFloat(animXMoveID, inputMove.x);
        animator.SetFloat(animYMoveID, inputMove.y);


        if (isRolling || isAttacking) return;

        Vector2 moveVector = GetPossibleMovement(inputMove, moveSpeed*Time.fixedDeltaTime);

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
        else if (nearestHitDiagFraction*speed > collisionOffset)
			moveVector = (nearestHitDiagFraction - collisionOffset) * moveInput;
		
        else
        {
            //Seperate movement in the axis -> for the situation explained above
            //If the move in x is bigger and big enough, move horizontally
            if (nearestHitXFraction > nearestHitYFraction && nearestHitXFraction*speed > collisionOffset)
				moveVector.x = (nearestHitXFraction - collisionOffset) * moveInput.x;
            //If move in y is big enough, move vertically
            else if (nearestHitYFraction*speed > collisionOffset)
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
        if (isAttacking)
            return;

		isRolling = true;
        isInvincibleInRoll = true;
        rollStarted = Time.time;
		animator.SetFloat(animRollingSpeedID, 1 / (rollDuration / 0.333f)); //0.333f is the duration of the original roll animation
		animator.SetTrigger(animRollingTriggerID);

        Vector2 possMove = GetPossibleMovement(currentInputMoveVector, rollDistance);

        rb.DOMove(rb.position + possMove*rollDistance, rollDuration);


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
        if (isRolling)
            return;

		if (Time.time - attackLastUsed < attackCooldown)
            return;

        Vector2 attackDir = currentInputMoveVector.normalized;
        
        if (cameraFollow != null)
            attackDir = cameraFollow.currentMouseOffset.normalized;

        if (playerSword != null)
        {
            playerSword.gameObject.SetActive(true);
            playerSword.AnimateAttack(1 / (attackDuration / 0.417f), attackDir);
        }

		animator.SetFloat(animAttackDirXID, attackDir.x);
        animator.SetFloat(animAttackDirYID, attackDir.y);
		animator.SetFloat(animAttackSpeedID, 1 / (attackDuration / 0.417f)); //0.417f is the duration of the original attack animation
		animator.SetTrigger(animAttackingTriggerID);

		isAttacking = true;
        attackStarted = Time.time;
	}

    void AttackUpdate()
    {
        if (Time.time - attackStarted >= attackDuration)
            AttackEnd();
	}

    public void EnemyHit(EnemyController enemy) 
    {
        bool killed = enemy.GetHit(attackDamage);

        if (killed)
        {
            UIFlashingNumbers.ShowFlashingNumber(transform, enemy.experienceOnDeath, Color.yellow);
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

        print($"Player got hit for {damage} damage!");

        currentHP -= damage;

        spriteRenderer.DOKill();
        spriteRenderer.color = Color.red;
        spriteRenderer.DOBlendableColor(origSpriteColor, 0.1f).SetLink(gameObject);

        UIFlashingNumbers.ShowFlashingNumber(transform, damage, Color.red);
	}

    #endregion COMBAT
}
