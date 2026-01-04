using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.StandaloneInputModule;

public class PlayerController : MonoBehaviour
{
    public int moveSpeed = 10;
    public float collisionOffset = 0.05f;

    public ContactFilter2D movementFilter;

    public Vector2 currentVelocity = Vector2.zero;
    public Vector2 currentInputMoveVector= Vector2.zero;

    InputAction moveAction;

    Rigidbody2D rb;
    List<RaycastHit2D> moveCastHits = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Move every frame -> idle if nothing is pressed
        Move(moveAction.ReadValue<Vector2>());
    }

    void Move(Vector2 inputMove)
    {
        currentInputMoveVector = inputMove;

        //Check if you can move in horizontal and vertical direction (if there is not an obstacle)
        //Check seperately to ensure movement in a situation where you hold up and right and can't go right, you still go up
        bool canMoveInX = TryToMovePlayer(new Vector2(inputMove.x,0));
        bool canMoveInY = TryToMovePlayer(new Vector2(0,inputMove.y));

        Vector2 moveVector = Vector2.zero;

        //Seperate movement in the axis -> for the situation explained above
        if (canMoveInX)
            moveVector.x = inputMove.x;
        if (canMoveInY)
            moveVector.y = inputMove.y;

        //Actually move the RB
        Vector2 moveVectorFinal = moveVector * moveSpeed * Time.fixedDeltaTime;
		rb.MovePosition(rb.position + moveVectorFinal);

        currentVelocity = moveVectorFinal;


	}

	bool TryToMovePlayer(Vector2 moveVec)
    {
        //Cast in the moveVec
        int collisionCount = rb.Cast(
             moveVec, //Direction
             movementFilter, //Filter -> such as layer mask (set in the inspector)
             moveCastHits, //The output list
             moveSpeed * Time.fixedDeltaTime + collisionOffset //The cast offset
             );

        if ( collisionCount > 0 )
            return false;

        return true;
    }
}
