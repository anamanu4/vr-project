public enum TurnOutcome { PlayerWins, AIWins, Draw }

public static class TurnResolver
{
    public static TurnOutcome Resolve(BlockType player, BlockType ai)
    {
        if (player == ai) return TurnOutcome.Draw;

        bool playerWins =
            (player == BlockType.Rock     && ai == BlockType.Scissors) ||
            (player == BlockType.Scissors && ai == BlockType.Paper)    ||
            (player == BlockType.Paper    && ai == BlockType.Rock);

        return playerWins ? TurnOutcome.PlayerWins : TurnOutcome.AIWins;
    }
}