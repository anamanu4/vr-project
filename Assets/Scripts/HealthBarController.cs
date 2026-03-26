using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [Header("Barras")]
    public Image playerBar;
    public Image aiBar;

    private void Start()
    {
        GameManager.Instance.health.onPlayerHPChanged.AddListener(OnPlayerHP);
        GameManager.Instance.health.onAIHPChanged.AddListener(OnAIHP);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.health.onPlayerHPChanged.RemoveListener(OnPlayerHP);
        GameManager.Instance.health.onAIHPChanged.RemoveListener(OnAIHP);
    }

    private void OnPlayerHP(int hp)
    {
        float ratio = (float)hp / GameManager.Instance.health.playerMaxHP;
        playerBar.fillAmount = ratio;
    }

    private void OnAIHP(int hp)
    {
        float ratio = (float)hp / GameManager.Instance.health.aiMaxHP;
        aiBar.fillAmount = ratio;
    }
}
