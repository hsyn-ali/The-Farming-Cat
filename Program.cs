using Raylib_cs;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Game;
using static Raylib_cs.Raylib;

InitWindow(1920, 1080, "Farm Game");
ToggleFullscreen();
SetTargetFPS(60);

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

// Define harvested + animal product + feed items
RunTime.WheatHarvestedItem = new Item("Wheat", RunTime.WheatHarvestedIcon);
RunTime.CarrotHarvestedItem = new Item("Carrot", RunTime.CarrotHarvestedIcon);
RunTime.BeetrootHarvestedItem = new Item("Beetroot", RunTime.BeetrootHarvestedIcon);

RunTime.EggItem = new Item("Egg", RunTime.EggIcon);
RunTime.MilkItem = new Item("Milk", RunTime.MilkIcon);
RunTime.WoolItem = new Item("Wool", RunTime.WoolIcon);

RunTime.ChickenFeedItem = new Item("Chicken Feed", RunTime.ChickenFeedIcon);
RunTime.CowFeedItem = new Item("Cow Feed", RunTime.CowFeedIcon);
RunTime.SheepFeedItem = new Item("Sheep Feed", RunTime.SheepFeedIcon);

// Define seed items
Item wheatSeed = new Item("Wheat Seed", RunTime.WheatSeedIcon, createCrop: () => new WheatCrop());
Item carrotSeed = new Item("Carrot Seed", RunTime.CarrotSeedIcon, createCrop: () => new CarrotCrop());
Item beetrootSeed = new Item("Beetroot Seed", RunTime.BeetrootSeedIcon, createCrop: () => new BeetrootCrop());

// World
Map map = new Map("resources/assets/map/map.png");
Player player = new Player(new Vector2(1140, 870));
Camera camera = new Camera(player.Position, new Vector2(1920, 1080));
Farm farm = new Farm(656, 1088);

// Animal farms
ChickenHouse chickenHouse = new ChickenHouse(new Vector2(1430, 340));
CowHouse cowHouse = new CowHouse(new Vector2(1640, 340));
SheepHouse sheepHouse = new SheepHouse(new Vector2(1870, 340));

// factories
ChickenFactory chickenFactory = new ChickenFactory(new Vector2(150, 135));
CowFactory cowFactory = new CowFactory(new Vector2(315, 650));
SheepFactory sheepFactory = new SheepFactory(new Vector2(550, 650));

List<Factory> factories = new List<Factory> { chickenFactory, cowFactory, sheepFactory };
List<AnimalFarm> animalFarms = new List<AnimalFarm> { chickenHouse, cowHouse, sheepHouse };

// Inventory
Inventory inventory = new Inventory();
inventory.Add(wheatSeed, 10);
inventory.Add(carrotSeed, 10);
inventory.Add(beetrootSeed, 10);
inventory.Add(RunTime.ChickenFeedItem, 1);
inventory.Add(RunTime.CowFeedItem, 1);
inventory.Add(RunTime.SheepFeedItem, 1);
inventory.Add(RunTime.WheatHarvestedItem, 10);

Hotbar hotbar = new Hotbar(inventory);

while (!WindowShouldClose())
{
    float dt = GetFrameTime();

    foreach (var af in animalFarms) af.Update(dt);
    foreach (var f in factories) f.Update(dt);

   bool anyPopupOpen =
    animalFarms.Any(af => af.IsPopupOpen) ||
    factories.Any(f => f.IsPopupOpen);

    if (!anyPopupOpen)
    {
        List<Rectangle> obstacles = new List<Rectangle>();

        foreach (var af in animalFarms)
            obstacles.Add(af.Hitbox);

        foreach (var f in factories)
            obstacles.Add(f.Hitbox);
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

    // FACTORIES (MISSING BEFORE)
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
        player.Draw();
    EndMode2D();

    hotbar.Draw(1920, 1080);

    foreach (var af in animalFarms)
    {
        af.DrawPopup(1920, 1080, inventory);
    }

    foreach (var f in factories)
    {
        f.DrawPopup(1920, 1080, inventory);
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