using UnityEngine;
using TMPro;

public class TVDisplay : MonoBehaviour
{
    [Header("Referencia")]
    public TMP_Text displayText;

    private void Start()
    {
        GameManager.Instance.onStateChanged.AddListener(OnStateChanged);
        GameManager.Instance.onTurnResolved.AddListener(OnTurnResolved);
        GameManager.Instance.onTowerCollapsed.AddListener(OnTowerCollapsed);
        GameManager.Instance.health.onPlayerHPChanged.AddListener(OnPlayerHP);
        GameManager.Instance.health.onAIHPChanged.AddListener(OnAIHP);

        _playerHP = GameManager.Instance.health.PlayerHP;
        _aiHP     = GameManager.Instance.health.AIHP;

        UpdateHP();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.onStateChanged.RemoveListener(OnStateChanged);
        GameManager.Instance.onTurnResolved.RemoveListener(OnTurnResolved);
        GameManager.Instance.onTowerCollapsed.RemoveListener(OnTowerCollapsed);
        GameManager.Instance.health.onPlayerHPChanged.RemoveListener(OnPlayerHP);
        GameManager.Instance.health.onAIHPChanged.RemoveListener(OnAIHP);
    }

    // ─── HP ──────────────────────────────────────────────────

    private int _playerHP;
    private int _aiHP;

    private void OnPlayerHP(int hp) { _playerHP = hp; UpdateHP(); }
    private void OnAIHP(int hp)     { _aiHP     = hp; UpdateHP(); }

    private void UpdateHP()
    {
        displayText.text = $"Jugador: {_playerHP} HP     IA: {_aiHP} HP";
    }

    // ─── Estado ──────────────────────────────────────────────

    private void OnStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.WaitingForRemoval:
                displayText.text = $"Jugador: {_playerHP} HP     IA: {_aiHP} HP\n\nTu turno\nSaca una pieza";
                break;
            case GameState.WaitingForPlacement:
                displayText.text = $"Jugador: {_playerHP} HP     IA: {_aiHP} HP\n\nColoca la pieza\nen la cima";
                break;
            case GameState.SettlingPiece:
                displayText.text = $"Jugador: {_playerHP} HP     IA: {_aiHP} HP\n\nEvaluando...";
                break;
            case GameState.ResolvingTurn:
                // Este se sobreescribe inmediatamente por OnTurnResolved
                break;
            case GameState.GameOver:
                // Se maneja en OnTowerCollapsed o en los HP events
                break;
        }
    }

    // ─── Resultado de turno ──────────────────────────────────

    private void OnTurnResolved(BlockType player, BlockType ai, TurnOutcome outcome)
    {
        string playerSymbol = TypeToSymbol(player);
        string aiSymbol     = TypeToSymbol(ai);

        string resultLine = outcome switch
        {
            TurnOutcome.PlayerWins => "¡Ganaste el turno! (-1 HP a IA)",
            TurnOutcome.AIWins     => "Perdiste el turno (-1 HP a ti)",
            TurnOutcome.Draw       => "Empate",
            _                      => ""
        };

        displayText.text =
            $"Jugador: {_playerHP} HP     IA: {_aiHP} HP\n\n" +
            $"Tú:  {playerSymbol}\n" +
            $"IA:  {aiSymbol}\n\n" +
            $"{resultLine}";
    }

    // ─── Torre caída ─────────────────────────────────────────

    private void OnTowerCollapsed()
    {
        displayText.text = "¡La torre cayó!\nFin del juego";
    }

    // ─── Helper ──────────────────────────────────────────────

    private string TypeToSymbol(BlockType type) => type switch
    {
        BlockType.Rock     => "🪨 Piedra",
        BlockType.Paper    => "📄 Papel",
        BlockType.Scissors => "✂️ Tijera",
        _                  => "?"
    };
}