using DG.Tweening;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public PlayerController playerObject;
    public bool followPlayer = true;

    [SerializeField]
    float moveTweenDuration = 0.3f;

    float maxMoveBasedOffset = 1;
    float maxMouseBasedOffset = 1;

    [HideInInspector]
    public Vector2 currentMouseOffset;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (!followPlayer) return;

        if (playerObject == null)
            Debug.LogError("PlayerObject not set in camera follow");


        Vector2 mousePos = Input.mousePosition;

        //Center of the current screen only considering the game
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        //Maximum distance from the center (any corner, calculated from Pythagoras)
        float maxMouseDist = Mathf.Sqrt(Screen.width / 2f * Screen.width / 2f + Screen.height / 2f * Screen.height / 2f);

		//Debug.Log($"Mouse position: {mousePos}");
		//Debug.Log($"Screen center: {screenCenter}");

        //Primary target is the player
        Vector2 targetPos = VecConvert.ToVec2(playerObject.transform.position);

		//Get the mouse offset from center
		currentMouseOffset = mousePos - screenCenter;
        //Percentage of the offset magnitude out of the max offset
        float curMouseDistPercentage = Mathf.Min(currentMouseOffset.magnitude / maxMouseDist, 1);
        
        //Get the camera offset based on mouse offset
        Vector2 fromMousePosOffset = currentMouseOffset.normalized * curMouseDistPercentage * maxMouseBasedOffset;
        //Get the camera offset based on the player movement
        Vector2 fromPlayerMovementOffset = playerObject.currentInputMoveVector.normalized * maxMoveBasedOffset;
        
        //Add the offset to the target position
        targetPos += fromMousePosOffset + fromPlayerMovementOffset;

        //Always kill the tween from last loop
        transform.DOKill();
        //Tween the position to the new one to make it look more smooth
        transform.DOMove(VecConvert.ToVec3CustomZ(targetPos, transform.position.z), moveTweenDuration).SetEase(Ease.Linear);

    }
}
