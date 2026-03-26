using UnityEngine;

/// <summary>
/// Collider trigger posicionado en la cima de la torre.
/// GameManager lo sube manualmente tras cada turno exitoso.
/// </summary>
[RequireComponent(typeof(Collider))]
public class TopPlacementZone : MonoBehaviour
{
    private JengaPiece _pieceInZone;

    private void OnTriggerEnter(Collider other)
    {
        var piece = other.GetComponent<JengaPiece>() 
            ?? other.GetComponentInParent<JengaPiece>();
        if (piece != null && piece.hasBeenRemoved)
            _pieceInZone = piece;
    }

    private void OnTriggerExit(Collider other)
    {
        if (_pieceInZone != null && other.gameObject == _pieceInZone.gameObject)
            _pieceInZone = null;
    }

    /// <returns>true si la pieza soltada estaba dentro de la zona</returns>
    public bool TryConsumePlacement(JengaPiece released, out JengaPiece placed)
    {
        placed = null;
        if (_pieceInZone != released) return false;
        placed = _pieceInZone;
        _pieceInZone = null;
        return true;
    }
}