using Raylib_cs;
using System.Numerics;
using Game;
using static Raylib_cs.Raylib;

InitWindow(1920, 1080, "Farm Game");
SetTargetFPS(60);

RunTime.PlayerRun = LoadTexture("resources/assets/player/player_run.png");
Map map = new Map("resources/assets/map/map.png");

Player player = new Player(new Vector2(400, 300));

Camera2D camera = new Camera2D
{
    Target = player.Position,
    Offset = new Vector2(400, 300),   // half the window size — puts target at screen center
    Rotation = 0f,
    Zoom = 1f
};

while (!WindowShouldClose())
{
    float dt = GetFrameTime();
    player.Update(dt);

    // Camera follows the player every frame
    camera.Target = player.Position;

    BeginDrawing();
    ClearBackground(Color.RayWhite);

    BeginMode2D(camera);
        map.Draw();
        player.Draw();
    EndMode2D();

    EndDrawing();
}

UnloadTexture(RunTime.PlayerRun);
map.Unload();
CloseWindow();