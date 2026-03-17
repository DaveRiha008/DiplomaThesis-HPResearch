using UnityEngine;

public class Tutorials : MonoBehaviour
{
    [SerializeField]
    GameObject HeartShow;
    [SerializeField]
    GameObject BarShow;
    [SerializeField]
    GameObject ScreenFXShow;
    [SerializeField]
    GameObject DSRegen;
    [SerializeField]
    GameObject BBRegen;
    [SerializeField]
    GameObject RallyRegen;
    [SerializeField]
    GameObject OverTimeRegen;
    [SerializeField]
    GameObject PickUpRegen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DisableAll();
        gameObject.SetActive(false);
    }
    
    public void ActivateTutorial()
    {
        GameManager.PauseGame();
        gameObject.SetActive(true);

        switch (GameManager.Instance.CurHPShowApproach)
        {
            case HPShowApproach.EldenRing:
                BarShow.SetActive(true);
                break;
            case HPShowApproach.HollowKnight:
                HeartShow.SetActive(true);
                break;
            case HPShowApproach.SilentHill:
                ScreenFXShow.SetActive(true);
                break;
            default:
                break;
        }

        switch (GameManager.Instance.CurHPRegenApproach)
        {
            case HPRegenApproach.OverTime:
                OverTimeRegen.SetActive(true);
                break;
            case HPRegenApproach.PickUp:
                PickUpRegen.SetActive(true);
                break;
            case HPRegenApproach.BloodBorneRally:
                RallyRegen.SetActive(true);
                break;
            case HPRegenApproach.DarkSoulsItems:
                DSRegen.SetActive(true);
                break;
            case HPRegenApproach.BloodborneItems:
                BBRegen.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void DeactivateTutorial() 
    {
        DisableAll();
        gameObject.SetActive(false);
        GameManager.ResumeGame();
    }

    void DisableAll()
    {
        HeartShow.SetActive(false);
        BarShow.SetActive(false);
        ScreenFXShow.SetActive(false);
        DSRegen.SetActive(false);
        BBRegen.SetActive(false);
        OverTimeRegen.SetActive(false);
        RallyRegen.SetActive(false);
        PickUpRegen.SetActive(false);
    }
}
