using Raylib_cs;
using System.Numerics;
using Game;
using static Raylib_cs.Raylib;

InitWindow(1920, 1080, "Farm Game");
ToggleFullscreen();
SetTargetFPS(60);

// Player & UI
RunTime.PlayerRun = LoadTexture("resources/assets/player/player_run.png");
RunTime.Hotbar = LoadTexture("resources/assets/hotbar/hotbar.png");

// Seed icons
RunTime.WheatSeedIcon = LoadTexture("resources/assets/hotbar/wheet_seed.png");
RunTime.CarrotSeedIcon = LoadTexture("resources/assets/hotbar/carrot_seed.png");
RunTime.BeetrootSeedIcon = LoadTexture("resources/assets/hotbar/beetroot_seed.png");

// Planted spritesheets
RunTime.WheatPlanted = LoadTexture("resources/assets/crops/wheet_planted.png");
RunTime.CarrotPlanted = LoadTexture("resources/assets/crops/carrot_planted.png");
RunTime.BeetrootPlanted = LoadTexture("resources/assets/crops/beetroot_planted.png");

// Harvested icons
RunTime.WheatHarvestedIcon = LoadTexture("resources/assets/hotbar/wheet.png");
RunTime.CarrotHarvestedIcon = LoadTexture("resources/assets/hotbar/carrot.png");
RunTime.BeetrootHarvestedIcon = LoadTexture("resources/assets/hotbar/beetroot.png");

// Define harvested items first (crops reference them)
RunTime.WheatHarvestedItem = new Item("Wheat", RunTime.WheatHarvestedIcon);
RunTime.CarrotHarvestedItem = new Item("Carrot", RunTime.CarrotHarvestedIcon);
RunTime.BeetrootHarvestedItem = new Item("Beetroot", RunTime.BeetrootHarvestedIcon);

// Define seed items (linked to their crop type)
Item wheatSeed = new Item("Wheat Seed", RunTime.WheatSeedIcon, createCrop: () => new WheatCrop());
Item carrotSeed = new Item("Carrot Seed", RunTime.CarrotSeedIcon, createCrop: () => new CarrotCrop());
Item beetrootSeed = new Item("Beetroot Seed", RunTime.BeetrootSeedIcon, createCrop: () => new BeetrootCrop());

Map map = new Map("resources/assets/map/map.png");
Player player = new Player(new Vector2(400, 300));
Camera camera = new Camera(player.Position, new Vector2(1920, 1080));
Farm farm = new Farm(656, 1088);

Inventory inventory = new Inventory();
inventory.Add(wheatSeed, 10);
inventory.Add(carrotSeed, 10);
inventory.Add(beetrootSeed, 10);

Hotbar hotbar = new Hotbar(inventory);

while (!WindowShouldClose())
{
    float dt = GetFrameTime();
    player.Update(dt, map.Width, map.Height);
    camera.Follow(player.Position, map.Width, map.Height);
    hotbar.Update();
    farm.Update(dt);

    Vector2 playerFeet = new Vector2(
        player.Hitbox.X + player.Hitbox.Width / 2f,
        player.Hitbox.Y + player.Hitbox.Height / 2f
    );

    if (IsKeyPressed(KeyboardKey.E))
    {
        var tile = farm.GetTileAt(playerFeet);
        if (tile.HasValue)
        {
            // Try harvest first
            Item? harvested = farm.TryHarvest(tile.Value.col, tile.Value.row);
            if (harvested != null)
            {
                inventory.Add(harvested, 1);
            }
            else
            {
                // Otherwise try plant
                Slot selected = inventory.Selected;
                if (selected.Item != null && selected.Item.IsPlantable)
                {
                    if (farm.TryPlant(tile.Value.col, tile.Value.row, selected.Item))
                    {
                        inventory.RemoveOne(inventory.SelectedIndex);
                    }
                }
            }
        }
    }

    BeginDrawing();
    ClearBackground(Color.RayWhite);

    BeginMode2D(camera.Raw);
        map.Draw();
        farm.Draw(playerFeet);
        player.Draw();
    EndMode2D();

    hotbar.Draw(1920, 1080);

    EndDrawing();
}

UnloadTexture(RunTime.PlayerRun);
UnloadTexture(RunTime.Hotbar);
UnloadTexture(RunTime.WheatSeedIcon);
UnloadTexture(RunTime.CarrotSeedIcon);
UnloadTexture(RunTime.BeetrootSeedIcon);
UnloadTexture(RunTime.WheatPlanted);
UnloadTexture(RunTime.CarrotPlanted);
UnloadTexture(RunTime.BeetrootPlanted);
UnloadTexture(RunTime.WheatHarvestedIcon);
UnloadTexture(RunTime.CarrotHarvestedIcon);
UnloadTexture(RunTime.BeetrootHarvestedIcon);
map.Unload();
CloseWindow();