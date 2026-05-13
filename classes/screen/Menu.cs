using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Game;

public class Menu
{
    public bool ShouldStartGame { get; private set; } = false;
    public bool ShouldQuit { get; private set; } = false;

    public void Update()
    {
        // Reset each frame, then check buttons
        Rectangle playBtn = GetPlayButton();
        Rectangle quitBtn = GetQuitButton();

        if (CheckCollisionPointRec(GetMousePosition(), playBtn) && IsMouseButtonPressed(MouseButton.Left))
            ShouldStartGame = true;

        if (CheckCollisionPointRec(GetMousePosition(), quitBtn) && IsMouseButtonPressed(MouseButton.Left))
            ShouldQuit = true;
    }

    public void Draw(int screenW, int screenH)
    {
        // Background image stretched to fill screen
        DrawTexturePro(
            RunTime.MenuBackground,
            new Rectangle(0, 0, RunTime.MenuBackground.Width, RunTime.MenuBackground.Height),
            new Rectangle(0, 0, screenW, screenH),
            new System.Numerics.Vector2(0, 0),
            0f,
            Color.White
        );

        // Title
        string title = "The Farming Cat";
        int titleSize = 80;
        int titleWidth = MeasureText(title, titleSize);
        DrawText(title, (screenW - titleWidth) / 2, screenH / 4, titleSize, Color.Black);

        // Buttons
        DrawButton(GetPlayButton(), "Play");
        DrawButton(GetQuitButton(), "Quit");
    }

    private Rectangle GetPlayButton()
    {
        int w = 300;
        int h = 70;
        int x = (1920 - w) / 2;
        int y = 1080 / 2;
        return new Rectangle(x, y, w, h);
    }

    private Rectangle GetQuitButton()
    {
        int w = 300;
        int h = 70;
        int x = (1920 - w) / 2;
        int y = 1080 / 2 + 100;
        return new Rectangle(x, y, w, h);
    }

    private void DrawButton(Rectangle rect, string label)
    {
        bool hovered = CheckCollisionPointRec(GetMousePosition(), rect);
        Color bg = hovered
            ? new Color(120, 90, 60, 255)
            : new Color(70, 55, 40, 240);

        DrawRectangleRec(rect, bg);
        DrawRectangleLinesEx(rect, 3, Color.White);

        int fontSize = 36;
        int textWidth = MeasureText(label, fontSize);
        DrawText(label,
            (int)(rect.X + (rect.Width - textWidth) / 2),
            (int)(rect.Y + (rect.Height - fontSize) / 2),
            fontSize,
            Color.White);
    }
}