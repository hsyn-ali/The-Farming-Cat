using Raylib_cs;

namespace Game;

public static class RunTime
{
    // Player
    public static Texture2D PlayerRun;

    // Hotbar
    public static Texture2D Hotbar;

    // Seed icons
    public static Texture2D WheatSeedIcon;
    public static Texture2D CarrotSeedIcon;
    public static Texture2D BeetrootSeedIcon;

    // Planted spritesheets
    public static Texture2D WheatPlanted;
    public static Texture2D CarrotPlanted;
    public static Texture2D BeetrootPlanted;

    // Harvested icons
    public static Texture2D WheatHarvestedIcon;
    public static Texture2D CarrotHarvestedIcon;
    public static Texture2D BeetrootHarvestedIcon;

    // Animal farms
    public static Texture2D ChickenHouse;
    public static Texture2D CowHouse;
    public static Texture2D SheepHouse;

    // Animal products (icons)
    public static Texture2D EggIcon;
    public static Texture2D MilkIcon;
    public static Texture2D WoolIcon;

    // Item definitions
    public static Item WheatHarvestedItem = null!;
    public static Item CarrotHarvestedItem = null!;
    public static Item BeetrootHarvestedItem = null!;

    public static Item EggItem = null!;
    public static Item MilkItem = null!;
    public static Item WoolItem = null!;
}