using System;

namespace Game;

public static class PlayerStats
{
    // XP and leveling
    public static int Level { get; private set; } = 1;
    public static int CurrentXP { get; private set; } = 0;
    public static int Coins { get; private set; } = 0;

    // Observer events — fire when state changes
    public static event Action? OnXPChanged;
    public static event Action? OnCoinsChanged;
    public static event Action<int>? OnLevelUp;

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
        {
            Level++;
            OnLevelUp?.Invoke(Level);  // Notify subscribers of level up
        }

        OnXPChanged?.Invoke();  // Notify subscribers of XP change
    }

    public static void AddCoins(int amount)
    {
        Coins += amount;
        OnCoinsChanged?.Invoke();
    }

    public static bool SpendCoins(int amount)
    {
        if (Coins < amount) return false;
        Coins -= amount;
        OnCoinsChanged?.Invoke();
        return true;
    }
}