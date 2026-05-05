using Raylib_cs;
using System.Collections.Generic;
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
RunTime.WheatHarvestedIcon = LoadTexture("resources/assets/hotbar/wheet_harvested.png");
RunTime.CarrotHarvestedIcon = LoadTexture("resources/assets/hotbar/carrot_harvested.png");
RunTime.BeetrootHarvestedIcon = LoadTexture("resources/assets/hotbar/beetroot_harvested.png");

// Animal farm + products
RunTime.ChickenHouse = LoadTexture("resources/assets/animal farms/chicken_house.png");
RunTime.EggIcon = LoadTexture("resources/assets/animal products/egg.png");

// Define item objects
RunTime.WheatHarvestedItem = new Item("Wheat", RunTime.WheatHarvestedIcon);
RunTime.CarrotHarvestedItem = new Item("Carrot", RunTime.CarrotHarvestedIcon);
RunTime.BeetrootHarvestedItem = new Item("Beetroot", RunTime.BeetrootHarvestedIcon);
RunTime.EggItem = new Item("Egg", RunTime.EggIcon);

Item wheatSeed = new Item("Wheat Seed", RunTime.WheatSeedIcon, createCrop: () => new WheatCrop());
Item carrotSeed = new Item("Carrot Seed", RunTime.CarrotSeedIcon, createCrop: () => new CarrotCrop());
Item beetrootSeed = new Item("Beetroot Seed", RunTime.BeetrootSeedIcon, createCrop: () => new BeetrootCrop());

Map map = new Map("resources/assets/map/map.png");
Player player = new Player(new Vector2(400, 300));
Camera camera = new Camera(player.Position, new Vector2(1920, 1080));
Farm farm = new Farm(656, 1088);
ChickenHouse chickenHouse = new ChickenHouse(new Vector2(420, 740));

Inventory inventory = new Inventory();
inventory.Add(wheatSeed, 10);
inventory.Add(carrotSeed, 10);
inventory.Add(beetrootSeed, 10);
inventory.Add(RunTime.WheatHarvestedItem, 5);  // some wheat to feed chickens with

Hotbar hotbar = new Hotbar(inventory);

while (!WindowShouldClose())
{
    float dt = GetFrameTime();

    // Always update animal farms (even when popup open, so timer ticks)
    chickenHouse.Update(dt);

    // Player + camera + hotbar input only when no popup open
    bool anyPopupOpen = chickenHouse.IsPopupOpen;

    if (!anyPopupOpen)
    {
        List<Rectangle> obstacles = new List<Rectangle> { chickenHouse.Hitbox };
        player.Update(dt, map.Width, map.Height, obstacles);
        camera.Follow(player.Position, map.Width, map.Height);
        hotbar.Update();

        // E to interact with chicken house if close enough
        if (IsKeyPressed(KeyboardKey.E))
        {
            if (chickenHouse.CanPlayerInteract(player.Hitbox))
            {
                chickenHouse.OpenPopup();
            }
            else
            {
                // Otherwise treat E as plant/harvest on farm tile (existing logic)
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
        chickenHouse.Draw();
        player.Draw();
    EndMode2D();

    hotbar.Draw(1920, 1080);

    // Popup drawn on top of everything
    chickenHouse.DrawPopup(1920, 1080, inventory, RunTime.WheatHarvestedItem);

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
UnloadTexture(RunTime.ChickenHouse);
UnloadTexture(RunTime.EggIcon);
map.Unload();
CloseWindow();