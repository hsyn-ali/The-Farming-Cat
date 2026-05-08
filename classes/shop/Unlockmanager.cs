namespace Game;

public static class UnlockManager
{
    public const int ShopLevel = 2;
    public const int CarrotSeedLevel = 3;
    public const int ChickenLevel = 4;
    public const int BeetrootSeedLevel = 5;
    public const int CowLevel = 6;
    public const int SheepLevel = 7;

    public static bool IsShopUnlocked => PlayerStats.Level >= ShopLevel;

    public static bool IsCarrotSeedUnlocked => PlayerStats.Level >= CarrotSeedLevel;
    public static bool IsBeetrootSeedUnlocked => PlayerStats.Level >= BeetrootSeedLevel;

    public static bool IsChickenAvailable => PlayerStats.Level >= ChickenLevel;
    public static bool IsCowAvailable => PlayerStats.Level >= CowLevel;
    public static bool IsSheepAvailable => PlayerStats.Level >= SheepLevel;
}