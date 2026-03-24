using UnityEngine;
using Oculus.Interaction;

[RequireComponent(typeof(Rigidbody))]
public class JengaPiece : MonoBehaviour
{
    [Header("Tipo RPS")]
    public BlockType blockType;

    [HideInInspector] public bool hasBeenRemoved = false;
    [HideInInspector] public int row;

    // El Grabbable puede estar en un hijo (estructura de Bui   ldingBlock)
    private Grabbable _grabbable;

    private void Awake()
    {
        _grabbable = GetComponentInChildren<Grabbable>();

        if (_grabbable == null)
        {
            Debug.LogWarning($"[JengaPiece] No se encontró Grabbable en {gameObject.name} ni en sus hijos.");
            return;
        }

        _grabbable.WhenPointerEventRaised += OnPointerEvent;
    }

    private void OnDestroy()
    {
        if (_grabbable != null)
            _grabbable.WhenPointerEventRaised -= OnPointerEvent;
    }

    private void OnPointerEvent(PointerEvent evt)
    {
        switch (evt.Type)
        {
            case PointerEventType.Select:
                if (hasBeenRemoved) return;
                hasBeenRemoved = true;
                GameManager.Instance.OnPieceRemoved(this);
                break;

            case PointerEventType.Unselect:
                GameManager.Instance.OnPieceReleased(this);
                break;
        }
    }
    
    
}