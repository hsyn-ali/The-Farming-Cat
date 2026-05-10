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
    public static Texture2D GoldenCarrotSeedIcon;

    // Planted spritesheets
    public static Texture2D WheatPlanted;
    public static Texture2D CarrotPlanted;
    public static Texture2D BeetrootPlanted;
    public static Texture2D GoldenCarrotPlanted;

    // Harvested icons
    public static Texture2D WheatHarvestedIcon;
    public static Texture2D CarrotHarvestedIcon;
    public static Texture2D BeetrootHarvestedIcon;
    public static Texture2D GoldenCarrotHarvestedIcon;

    // Animal farms
    public static Texture2D ChickenHouse;
    public static Texture2D CowHouse;
    public static Texture2D SheepHouse;

    // Animal product icons
    public static Texture2D EggIcon;
    public static Texture2D MilkIcon;
    public static Texture2D WoolIcon;

    // Animal feed icons
    public static Texture2D ChickenFeedIcon;
    public static Texture2D CowFeedIcon;
    public static Texture2D SheepFeedIcon;

    // Item definitions — harvested crops
    public static Item WheatHarvestedItem = null!;
    public static Item CarrotHarvestedItem = null!;
    public static Item BeetrootHarvestedItem = null!;
    public static Item GoldenCarrotHarvestedItem = null!;

    //// Item definitions — harvested crops
    public static Item WheatSeedItem;
    public static Item CarrotSeedItem;
    public static Item BeetrootSeedItem;
    public static Item GoldenCarrotSeedItem;

    // Item definitions — animal products
    public static Item EggItem = null!;
    public static Item MilkItem = null!;
    public static Item WoolItem = null!;

    // Item definitions — animal feeds
    public static Item ChickenFeedItem = null!;
    public static Item CowFeedItem = null!;
    public static Item SheepFeedItem = null!;
    
    // Factories
    public static Texture2D ChickenFactory;
    public static Texture2D CowFactory;
    public static Texture2D SheepFactory;

    //Shop
    public static Texture2D ShopBuilding;

    //Chest
    public static Texture2D ChestSprite;

    //NoticeBoard
    public static Texture2D NoticeBoardSprite;

    //Menu
    public static Texture2D MenuBackground;

    //sounds
    public static Sound PopupSound;
    public static Sound ClickSound;
    public static Sound PlantSound;
    public static Sound HarvestSound;
    public static Sound BuySellSound;
    public static Sound ChickenSound;
    public static Sound CowSound;
    public static Sound SheepSound;
    public static Music BackgroundMusic;
}