using UnityEngine;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour
{
    public int playerMaxHP = 5;
    public int aiMaxHP     = 5;

    public int PlayerHP { get; private set; }
    public int AIHP     { get; private set; }

    // (int newHP)
    public UnityEvent<int> onPlayerHPChanged;
    public UnityEvent<int> onAIHPChanged;

    public UnityEvent onPlayerDied;
    public UnityEvent onAIDied;

    private void Awake() => ResetHP();

    public void DamagePlayer(int amount = 1)
    {
        PlayerHP = Mathf.Max(0, PlayerHP - amount);
        onPlayerHPChanged.Invoke(PlayerHP);
        if (PlayerHP == 0) onPlayerDied.Invoke();
    }

    public void DamageAI(int amount = 1)
    {
        AIHP = Mathf.Max(0, AIHP - amount);
        onAIHPChanged.Invoke(AIHP);
        if (AIHP == 0) onAIDied.Invoke();
    }

    public void ResetHP()
    {
        PlayerHP = playerMaxHP;
        AIHP     = aiMaxHP;
        onPlayerHPChanged.Invoke(PlayerHP);
        onAIHPChanged.Invoke(AIHP);
    }
}