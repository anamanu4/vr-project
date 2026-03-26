using System.Collections.Generic;
using UnityEngine;

public class TowerGenerator : MonoBehaviour
{
    [Header("Prefab y dimensiones")]
    public GameObject piecePrefab;
    public int rows = 18;

    // Dimensiones estándar de Jenga (metros)
    public float pieceWidth  = 0.075f;
    public float pieceHeight = 0.025f;
    public float pieceLength = 0.225f;
    public float gap         = 0.001f;  // separación entre fichas

    [Header("Distribución de tipos")]
    [Range(0f, 1f)] public float rockChance    = 0.334f;
    [Range(0f, 1f)] public float paperChance   = 0.333f;
    // scissors ocupa el resto

    public List<JengaPiece> AllPieces { get; private set; } = new();

    public void GenerateTower()
    {
        AllPieces.Clear();
        foreach (Transform child in transform) Destroy(child.gameObject);

        for (int row = 0; row < rows; row++)
        {
            bool isEven = (row % 2 == 0);
            float yCenter = pieceHeight * 0.5f + row * pieceHeight;

            for (int col = 0; col < 3; col++)
            {
                float lateralOffset = (col - 1) * (pieceWidth + gap);

                Vector3 pos = isEven
                    ? transform.position + new Vector3(lateralOffset, yCenter, 0f)
                    : transform.position + new Vector3(0f, yCenter, lateralOffset);

                Quaternion rot = isEven
                    ? Quaternion.identity
                    : Quaternion.Euler(0f, 90f, 0f);

                GameObject go = Instantiate(piecePrefab, pos, rot, transform);
                go.name = $"Piece_R{row}_C{col}";

                JengaPiece piece = go.GetComponent<JengaPiece>();
                piece.blockType = RandomType();
                piece.row = row;

                SetColor(go, piece.blockType);

                AllPieces.Add(piece);
            }
        }
    }

    private BlockType RandomType()
    {
        float r = Random.value;
        if (r < rockChance)               return BlockType.Rock;
        if (r < rockChance + paperChance) return BlockType.Paper;
        return BlockType.Scissors;
    }
    private void SetColor(GameObject go, BlockType type)
{
    Renderer renderer = go.GetComponent<Renderer>();

    if (renderer != null)
    {
        // IMPORTANTE: usar material instancia (no sharedMaterial)
        Material mat = renderer.material;

        switch (type)
        {
            case BlockType.Rock:
                mat.color = Color.gray;
                break;

            case BlockType.Paper:
                mat.color = Color.white;
                break;

            case BlockType.Scissors:
                mat.color = Color.red;
                break;
        }
    }
}
}