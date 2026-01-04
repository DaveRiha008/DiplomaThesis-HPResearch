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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        float maxMouseDist = Mathf.Sqrt(Screen.width / 2f * Screen.width / 2f + Screen.height / 2f * Screen.height / 2f);
		//Debug.Log($"Mouse position: {mousePos}");
		//Debug.Log($"Screen center: {screenCenter}");

		if (!followPlayer) return;
        if (playerObject == null)
            Debug.LogError("PlayerObject not set in camera follow");

        Vector2 targetPos = VecConvert.ToVec2(playerObject.transform.position);
        Vector2 curMouseOffset = mousePos - screenCenter;
        float curMouseDistPercentage = Mathf.Min(curMouseOffset.magnitude / maxMouseDist, 1);
        Vector2 fromMousePosOffset =  curMouseOffset.normalized * curMouseDistPercentage * maxMouseBasedOffset;
        Vector2 fromPlayerMovementOffset = playerObject.currentInputMoveVector.normalized * maxMoveBasedOffset;
        targetPos += fromMousePosOffset + fromPlayerMovementOffset;

        transform.DOKill();
        transform.DOMove(VecConvert.ToVec3CustomZ(targetPos, transform.position.z), moveTweenDuration).SetEase(Ease.Linear);
        //transform.position = VecConvert.ToVec3CustomZ(targetPos, transform.position.z);
    }
}
