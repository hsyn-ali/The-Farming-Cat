namespace Game;

public static class PlayerStats
{
    // XP and leveling
    public static int Level { get; private set; } = 1;
    public static int CurrentXP { get; private set; } = 0;
    public static int Coins { get; private set; } = 0;

    // XP required to reach each level (index = level number)
    private static readonly int[] XpThresholds = { 0, 100, 250, 500, 850, 1300, 1850, 2500 };

    public static int XpForCurrentLevel => XpThresholds[Level - 1];
    public static int XpForNextLevel => Level < XpThresholds.Length ? XpThresholds[Level] : XpThresholds[^1];
    public static bool IsMaxLevel => Level >= XpThresholds.Length;

    public static void AddXP(int amount)
    {
        if (IsMaxLevel) return;

        CurrentXP += amount;

        // Check for level up (could level up multiple times at once)
        while (!IsMaxLevel && CurrentXP >= XpForNextLevel)
            Level++;
    }

    public static void AddCoins(int amount) => Coins += amount;

    public static bool SpendCoins(int amount)
    {
        if (Coins < amount) return false;
        Coins -= amount;
        return true;
    }
}