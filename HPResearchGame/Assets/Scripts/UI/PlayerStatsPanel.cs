using TMPro;
using UnityEngine;

public class PlayerStatsPanel : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI healthLabel;
    [SerializeField] TextMeshProUGUI expLabel;
    [SerializeField] TextMeshProUGUI attackLabel;
    [SerializeField] TextMeshProUGUI moveSpeedLabel;
    [SerializeField] PlayerController targetPlayer;
    string healthStringPrefix = "Health: ";
    string expStringPrefix = "Exp: ";
    string attackStringPrefix = "Attack: ";
    string speedStringPrefix = "MoveSpeed: ";



	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MenuShown()
    {
        UpdateAllLabels(targetPlayer.GetPlayerStats());
	}

	void UpdateAllLabels(PlayerStats stats)
    {
        healthLabel.text = healthStringPrefix + $"{stats.CurHealth}/{stats.MaxHealth}";
        expLabel.text = expStringPrefix + $"{stats.CurExp}/{stats.GoalExp}";
        attackLabel.text = attackStringPrefix + stats.Attack.ToString();
        moveSpeedLabel.text = speedStringPrefix + stats.MoveSpeed.ToString();
	}
}

public struct PlayerStats
{
    public float CurHealth;
    public float MaxHealth;
    public int CurExp;
    public int GoalExp;
    public int Attack;
    public float MoveSpeed;
    public PlayerStats(float curHealth, float maxHealth, int curExp, int goalExp, int attack, float moveSpeed)
    {
        CurHealth = curHealth;
        MaxHealth = maxHealth;
        CurExp = curExp;
        GoalExp = goalExp;
        Attack = attack;
        MoveSpeed = moveSpeed;
    }
}