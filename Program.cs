using Raylib_cs;
using System.Numerics;
using Game;
using static Raylib_cs.Raylib;

InitWindow(800, 600, "Farm Game");
SetTargetFPS(60);

RunTime.PlayerRun = LoadTexture("resources/assets/player/player_run.png");

Player player = new Player(new Vector2(400, 300));

while (!WindowShouldClose())
{
    float dt = GetFrameTime();
    player.Update(dt);

    BeginDrawing();
    ClearBackground(Color.RayWhite);
    player.Draw();
    EndDrawing();
}

UnloadTexture(RunTime.PlayerRun);
CloseWindow();