using UnityEngine;

public class AIOpponent : MonoBehaviour
{
    public BlockType PickRandom() => (BlockType)Random.Range(0, 3);
}