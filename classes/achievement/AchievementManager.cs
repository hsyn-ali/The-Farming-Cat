using System;
using System.Collections.Generic;
using System.Linq;

namespace Game;

public static class AchievementManager
{
    private static List<Achievement> _achievements = new();

    public static event Action<Achievement>? OnAchievementUnlocked;

    public static IReadOnlyList<Achievement> All => _achievements;

    public static void Initialize()
    {
        _achievements.Clear();
        _achievements.Add(new Achievement("First Steps",   "Reach Level 2"));
        _achievements.Add(new Achievement("Halfway There", "Reach Level 5"));
        _achievements.Add(new Achievement("Maxed Out",     "Reach the maximum level"));
        _achievements.Add(new Achievement("Penny Pincher", "Hold 500 coins"));
        _achievements.Add(new Achievement("Coin Magnate",  "Hold 2000 coins"));

        PlayerStats.OnLevelUp += HandleLevelUp;
        PlayerStats.OnCoinsChanged += HandleCoinsChanged;
    }

    private static void HandleLevelUp(int newLevel)
    {
        if (newLevel >= 2) TryUnlock("First Steps");
        if (newLevel >= 5) TryUnlock("Halfway There");
        if (PlayerStats.IsMaxLevel) TryUnlock("Maxed Out");
    }

    private static void HandleCoinsChanged()
    {
        if (PlayerStats.Coins >= 500) TryUnlock("Penny Pincher");
        if (PlayerStats.Coins >= 2000) TryUnlock("Coin Magnate");
    }

    private static void TryUnlock(string title)
    {
        Achievement? a = _achievements.FirstOrDefault(x => x.Title == title);
        if (a != null && !a.IsUnlocked)
        {
            a.Unlock();
            OnAchievementUnlocked?.Invoke(a);
        }
    }
}