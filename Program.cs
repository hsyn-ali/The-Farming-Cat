using Raylib_cs;
using System.Numerics;
using Game;
using static Raylib_cs.Raylib;

InitWindow(1920, 1080, "Farm Game");
ToggleFullscreen();
SetTargetFPS(60);

RunTime.PlayerRun = LoadTexture("resources/assets/player/player_run.png");
RunTime.WheatSeedIcon = LoadTexture("resources/assets/hotbar/wheet_seed.png");
RunTime.Hotbar = LoadTexture("resources/assets/hotbar/hotbar.png");

Map map = new Map("resources/assets/map/map.png");
Player player = new Player(new Vector2(400, 300));
Camera camera = new Camera(player.Position, new Vector2(1920, 1080));
Farm farm = new Farm(656, 1088);

Item wheatSeed = new Item("Wheat Seed", RunTime.WheatSeedIcon);
Inventory inventory = new Inventory();
inventory.Add(wheatSeed, 10);

Hotbar hotbar = new Hotbar(inventory);

while (!WindowShouldClose())
{
    float dt = GetFrameTime();
    player.Update(dt, map.Width, map.Height);
    camera.Follow(player.Position, map.Width, map.Height);
    hotbar.Update();

    Vector2 playerFeet = new Vector2(
        player.Hitbox.X + player.Hitbox.Width / 2f,
        player.Hitbox.Y + player.Hitbox.Height / 2f
    );

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
UnloadTexture(RunTime.WheatSeedIcon);
UnloadTexture(RunTime.Hotbar);
map.Unload();
CloseWindow();