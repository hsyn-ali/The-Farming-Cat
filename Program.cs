using Raylib_cs;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Game;
using static Raylib_cs.Raylib;

InitWindow(1920, 1080, "Farm Game");
ToggleFullscreen();
SetTargetFPS(144);

List<Texture2D> textures = new List<Texture2D>();

Texture2D LoadTex(string path)
{
    Texture2D tex = LoadTexture(path);
    textures.Add(tex);
    return tex;
}

// Player & UI
RunTime.PlayerRun = LoadTex("resources/assets/player/player_run.png");
RunTime.Hotbar = LoadTex("resources/assets/hotbar/hotbar.png");

// Seed icons
RunTime.WheatSeedIcon = LoadTex("resources/assets/crops/wheet_seed.png");
RunTime.CarrotSeedIcon = LoadTex("resources/assets/crops/carrot_seed.png");
RunTime.BeetrootSeedIcon = LoadTex("resources/assets/crops/beetroot_seed.png");

// Planted spritesheets
RunTime.WheatPlanted = LoadTex("resources/assets/crops/wheet_planted.png");
RunTime.CarrotPlanted = LoadTex("resources/assets/crops/carrot_planted.png");
RunTime.BeetrootPlanted = LoadTex("resources/assets/crops/beetroot_planted.png");

// Harvested icons
RunTime.WheatHarvestedIcon = LoadTex("resources/assets/crops/wheet_harvested.png");
RunTime.CarrotHarvestedIcon = LoadTex("resources/assets/crops/carrot_harvested.png");
RunTime.BeetrootHarvestedIcon = LoadTex("resources/assets/crops/beetroot_harvested.png");

// Animal farms
RunTime.ChickenHouse = LoadTex("resources/assets/animal farms/chicken_house.png");
RunTime.CowHouse = LoadTex("resources/assets/animal farms/cow_house.png");
RunTime.SheepHouse = LoadTex("resources/assets/animal farms/sheep_house.png");

// Animal product icons
RunTime.EggIcon = LoadTex("resources/assets/animal products/egg.png");
RunTime.MilkIcon = LoadTex("resources/assets/animal products/milk.png");
RunTime.WoolIcon = LoadTex("resources/assets/animal products/wool.png");

// Animal feed icons
RunTime.ChickenFeedIcon = LoadTex("resources/assets/animal products/chicken_feed.png");
RunTime.CowFeedIcon = LoadTex("resources/assets/animal products/cow_feed.png");
RunTime.SheepFeedIcon = LoadTex("resources/assets/animal products/sheep_feed.png");

// factories
RunTime.ChickenFactory = LoadTex("resources/assets/factories/chicken_factory.png");
RunTime.CowFactory = LoadTex("resources/assets/factories/cow_factory.png");
RunTime.SheepFactory = LoadTex("resources/assets/factories/sheep_factory.png");

//Shop
RunTime.ShopBuilding = LoadTex("resources/assets/shop/shop.png");

// Define harvested + animal product + feed items
RunTime.WheatHarvestedItem = new Item("Wheat", RunTime.WheatHarvestedIcon, xpReward: 5);
RunTime.CarrotHarvestedItem = new Item("Carrot", RunTime.CarrotHarvestedIcon, xpReward: 8);
RunTime.BeetrootHarvestedItem = new Item("Beetroot", RunTime.BeetrootHarvestedIcon, xpReward: 10);

RunTime.EggItem = new Item("Egg", RunTime.EggIcon, xpReward: 6);
RunTime.MilkItem = new Item("Milk", RunTime.MilkIcon, xpReward: 15);
RunTime.WoolItem = new Item("Wool", RunTime.WoolIcon, xpReward: 12);

RunTime.ChickenFeedItem = new Item("Chicken Feed", RunTime.ChickenFeedIcon, xpReward: 2);
RunTime.CowFeedItem = new Item("Cow Feed", RunTime.CowFeedIcon, xpReward: 2);
RunTime.SheepFeedItem = new Item("Sheep Feed", RunTime.SheepFeedIcon, xpReward: 2);

// Define seed items
RunTime.WheatSeedItem = new Item("Wheat Seed", RunTime.WheatSeedIcon, createCrop: () => new WheatCrop());
RunTime.CarrotSeedItem = new Item("Carrot Seed", RunTime.CarrotSeedIcon, createCrop: () => new CarrotCrop());
RunTime.BeetrootSeedItem = new Item("Beetroot Seed", RunTime.BeetrootSeedIcon, createCrop: () => new BeetrootCrop());

// World
Map map = new Map("resources/assets/map/map.png");
Player player = new Player(new Vector2(1140, 870));
Camera camera = new Camera(player.Position, new Vector2(1920, 1080));
Farm farm = new Farm(656, 1088);

// Animal farms
ChickenHouse chickenHouse = new ChickenHouse(new Vector2(1430, 575));
CowHouse cowHouse = new CowHouse(new Vector2(1640, 575));
SheepHouse sheepHouse = new SheepHouse(new Vector2(1870, 575));

// factories
ChickenFactory chickenFactory = new ChickenFactory(new Vector2(150, 135));
CowFactory cowFactory = new CowFactory(new Vector2(315, 650));
SheepFactory sheepFactory = new SheepFactory(new Vector2(550, 650));

//shop
Shop shop = new Shop(new Vector2(400, 1110)); 

List<Factory> factories = new List<Factory> { chickenFactory, cowFactory, sheepFactory };
List<AnimalFarm> animalFarms = new List<AnimalFarm> { chickenHouse, cowHouse, sheepHouse };
List<InteractableBuilding> shops = new List<InteractableBuilding> { shop };

// Inventory — start with wheat seeds only, plus a small coin pool
Inventory inventory = new Inventory();
inventory.Add(RunTime.WheatSeedItem, 10);
PlayerStats.AddCoins(50);

Hotbar hotbar = new Hotbar(inventory);
HUD hud = new HUD();

while (!WindowShouldClose())
{
    float dt = GetFrameTime();

    // DEBUG keys — remove before submission
    if (IsKeyPressed(KeyboardKey.F1)) PlayerStats.AddXP(100);
    if (IsKeyPressed(KeyboardKey.F2)) PlayerStats.AddCoins(100);

    foreach (var af in animalFarms) af.Update(dt);
    foreach (var f in factories) f.Update(dt);
    foreach (var s in shops)s.Update(dt);
    
    bool anyPopupOpen =
        animalFarms.Any(af => af.IsPopupOpen) ||
        factories.Any(f => f.IsPopupOpen) ||
        shops.Any(s => s.IsPopupOpen);

    if (!anyPopupOpen)
    {
        List<Rectangle> obstacles = new List<Rectangle>();

        foreach (var af in animalFarms)
            obstacles.Add(af.Hitbox);

        foreach (var f in factories)
            obstacles.Add(f.Hitbox);

        foreach (var s in shops)
            obstacles.Add(s.Hitbox);

        player.Update(dt, map.Width, map.Height, obstacles);
        camera.Follow(player.Position, map.Width, map.Height);
        hotbar.Update();

        if (IsKeyPressed(KeyboardKey.E))
        {
            bool opened = false;

            // ANIMAL FARMS
            foreach (var af in animalFarms)
            {
                if (af.CanPlayerInteract(player.Hitbox))
                {
                    af.OpenPopup();
                    opened = true;
                    break;
                }
            }

            // FACTORIES
            if (!opened)
            {
                foreach (var f in factories)
                {
                    if (f.CanPlayerInteract(player.Hitbox))
                    {
                        f.OpenPopup();
                        opened = true;
                        break;
                    }
                }
            }

            // SHOPS
            if (!opened)
            {
                foreach (var s in shops)
                {
                    if (s.CanPlayerInteract(player.Hitbox))
                    {
                        s.OpenPopup();
                        opened = true;
                        break;
                    }
                }
            }

            // FARM TILE LOGIC (only if nothing opened)
            if (!opened)
            {
                Vector2 feet = new Vector2(
                    player.Hitbox.X + player.Hitbox.Width / 2f,
                    player.Hitbox.Y + player.Hitbox.Height / 2f
                );

                var tile = farm.GetTileAt(feet);
                if (tile.HasValue)
                {
                    Item? harvested = farm.TryHarvest(tile.Value.col, tile.Value.row);
                    if (harvested != null)
                    {
                        inventory.Add(harvested, 1);
                    }
                    else
                    {
                        Slot selected = inventory.Selected;
                        if (selected.Item != null && selected.Item.IsPlantable)
                        {
                            if (farm.TryPlant(tile.Value.col, tile.Value.row, selected.Item))
                                inventory.RemoveOne(inventory.SelectedIndex);
                        }
                    }
                }
            }
        }
    }

    farm.Update(dt);

    Vector2 playerFeet = new Vector2(
        player.Hitbox.X + player.Hitbox.Width / 2f,
        player.Hitbox.Y + player.Hitbox.Height / 2f
    );

    BeginDrawing();
    ClearBackground(Color.RayWhite);

    BeginMode2D(camera.Raw);
        map.Draw();
        farm.Draw(playerFeet);
        foreach (var af in animalFarms) af.Draw();
        foreach (var f in factories) f.Draw();
        foreach (var s in shops) s.Draw();
        player.Draw();
    EndMode2D();

    hotbar.Draw(1920, 1080);
    hud.Draw(1920, 1080);

    foreach (var af in animalFarms)
    {
        af.DrawPopup(1920, 1080, inventory);
    }

    foreach (var f in factories)
    {
        f.DrawPopup(1920, 1080, inventory);
    }
    foreach (var s in shops)
    {
        s.DrawPopup(1920, 1080, inventory);
    }
    EndDrawing();
}

foreach (var tex in textures)
{
    UnloadTexture(tex);
}
textures.Clear();

map.Unload();
CloseWindow();