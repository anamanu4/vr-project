using System.Collections;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;

public enum GameState { WaitingForRemoval, WaitingForPlacement, ResolvingTurn, GameOver, SettlingPiece}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Referencias")]
    public TowerGenerator        towerGenerator;
    public TowerCollapseDetector collapseDetector;
    public TopPlacementZone      placementZone;
    public AIOpponent            ai;
    public HealthManager         health;

    [Header("Timing")]
    [Tooltip("Segundos de espera tras colocar la pieza antes de resolver (la física se asienta)")]
    public float settleDelay = 1.5f;
    public float resolveDisplayDuration = 2.5f;

    // (BlockType jugador, BlockType IA, TurnOutcome resultado)
    public UnityEvent<BlockType, BlockType, TurnOutcome> onTurnResolved;
    public UnityEvent onTowerCollapsed;
    public UnityEvent<GameState> onStateChanged;

    public GameState State { get; private set; }

    private JengaPiece _removedPiece;
    
    

    // ─── Unity ───────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        towerGenerator.GenerateTower();
        collapseDetector.Initialize(towerGenerator.AllPieces);

        health.onPlayerDied.AddListener(() => SetState(GameState.GameOver));
        health.onAIDied.AddListener(() => SetState(GameState.GameOver));
        collapseDetector.onCollapse.AddListener(HandleCollapse);

        SetState(GameState.WaitingForRemoval);
    }

    // ─── Callbacks desde JengaPiece ──────────────────────────

    public void OnPieceRemoved(JengaPiece piece)
    {
        if (State != GameState.WaitingForRemoval) return;
        _removedPiece = piece;
        SetState(GameState.WaitingForPlacement);
    }

    public void OnPieceReleased(JengaPiece piece)
    {
        if (State != GameState.WaitingForPlacement) return;
        if (piece != _removedPiece) return;

        if (placementZone.TryConsumePlacement(piece, out JengaPiece placed))
            StartCoroutine(SettleAndResolve(placed));
        // Si soltó fuera de la zona, puede volver a agarrarla
    }

    // ─── Lógica de turno ─────────────────────────────────────

    private IEnumerator SettleAndResolve(JengaPiece placed)
    {
        SetState(GameState.SettlingPiece);   
        SetInteractability(false);  
        yield return new WaitForSeconds(settleDelay);

        if (collapseDetector.HasCollapsed)
        {
            HandleCollapse();
            yield break;
        }
        
        SetState(GameState.ResolvingTurn);


        BlockType playerType = placed.blockType;
        BlockType aiType     = ai.PickRandom();
        TurnOutcome outcome  = TurnResolver.Resolve(playerType, aiType);

        switch (outcome)
        {
            case TurnOutcome.PlayerWins: health.DamageAI();     break;
            case TurnOutcome.AIWins:     health.DamagePlayer(); break;
            // Draw: sin cambios
        }

        onTurnResolved.Invoke(playerType, aiType, outcome);

        if (State == GameState.GameOver) yield break;
        yield return new WaitForSeconds(resolveDisplayDuration);

        // Subir la zona de colocación (una capa = pieceHeight)
        placementZone.transform.position += Vector3.up * towerGenerator.pieceHeight;

        _removedPiece = null;
        
        SetInteractability(true); 
        SetState(GameState.WaitingForRemoval);
    }

    private void HandleCollapse()
    {
        SetInteractability(false); 
        onTowerCollapsed.Invoke();
        SetState(GameState.GameOver);
    }

    private void SetState(GameState next)
    {
        State = next;
        onStateChanged.Invoke(next);
    }
    
    private void SetInteractability(bool enabled)
    {
        foreach (var piece in towerGenerator.AllPieces)
        {
            if (piece == null) continue;
            var grab = piece.GetComponentInChildren<Grabbable>();
            if (grab != null) grab.enabled = enabled;
        }
    }
}