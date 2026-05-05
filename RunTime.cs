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

    // Harvested item definitions (created at startup, referenced by crops)
    public static Item WheatHarvestedItem = null!;
    public static Item CarrotHarvestedItem = null!;
    public static Item BeetrootHarvestedItem = null!;
    
// Animal farm
public static Texture2D ChickenHouse;

// Animal products
public static Texture2D EggIcon;
public static Item EggItem = null!;
}