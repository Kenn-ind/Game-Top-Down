public enum GameModeType
{
    Normal,  // Tutorial + gameplay bebas
    Story    // Tutorial + quest cerita utama
}

public static class GameMode
{
    public static GameModeType Current = GameModeType.Normal;
}