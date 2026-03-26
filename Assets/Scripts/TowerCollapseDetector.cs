using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TowerCollapseDetector : MonoBehaviour
{
    [Tooltip("Cuántos metros debe desplazarse una pieza para considerarse caída")]
    public float fallThreshold = 0.5f;

    public UnityEvent onCollapse;
    public bool HasCollapsed { get; private set; }

    private readonly Dictionary<JengaPiece, Vector3> _origins = new();
    private bool _active;

    public void Initialize(List<JengaPiece> pieces)
    {
        _origins.Clear();
        HasCollapsed = false;
        _active = false;

        foreach (var p in pieces)
            _origins[p] = p.transform.position;

        // Pequeño delay para que la física de la torre se asiente antes de monitorear
        Invoke(nameof(Activate), 1.5f);
    }

    private void Activate() => _active = true;

    private void Update()
    {
        if (!_active || HasCollapsed) return;

        foreach (var kvp in _origins)
        {
            JengaPiece piece = kvp.Key;
            if (piece == null || piece.hasBeenRemoved) continue;

            if (Vector3.Distance(piece.transform.position, kvp.Value) > fallThreshold)
            {
                HasCollapsed = true;
                onCollapse.Invoke();
                return;
            }
        }
    }
}