using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Game;

public class HUD
{
    private const int BarWidth = 300;
    private const int BarHeight = 20;
    private const int Margin = 20;
    private const int FontSize = 22;

    public void Draw(int screenWidth, int screenHeight)
    {
        // Position: top-right corner
        int barX = screenWidth - BarWidth - Margin;
        int barY = Margin;

        DrawLevelAndXP(barX, barY);
        DrawCoins(barX, barY + BarHeight + 30);
    }

    private void DrawLevelAndXP(int x, int y)
    {
        // Level label
        string levelText = PlayerStats.IsMaxLevel
            ? "MAX LEVEL"
            : $"Level {PlayerStats.Level}";

        DrawText(levelText, x, y, FontSize, Color.White);

        int labelWidth = MeasureText(levelText, FontSize);

        // XP bar background
        int barStartX = x + labelWidth + 12;
        int barStartY = y + 2;
        int remainingBarWidth = BarWidth - labelWidth - 12;

        DrawRectangle(barStartX, barStartY, remainingBarWidth, BarHeight, new Color(50, 50, 50, 200));

        // XP bar fill
        if (!PlayerStats.IsMaxLevel)
        {
            int xpMin = PlayerStats.XpForCurrentLevel;
            int xpMax = PlayerStats.XpForNextLevel;
            float progress = (float)(PlayerStats.CurrentXP - xpMin) / (xpMax - xpMin);
            progress = System.Math.Clamp(progress, 0f, 1f);

            int fillWidth = (int)(remainingBarWidth * progress);
            DrawRectangle(barStartX, barStartY, fillWidth, BarHeight, new Color(80, 180, 80, 255));
        }

        // XP bar outline
        DrawRectangleLines(barStartX, barStartY, remainingBarWidth, BarHeight, Color.White);

        // XP numbers inside bar
        if (!PlayerStats.IsMaxLevel)
        {
            string xpText = $"{PlayerStats.CurrentXP} / {PlayerStats.XpForNextLevel} XP";
            int xpTextWidth = MeasureText(xpText, 16);
            DrawText(
                xpText,
                barStartX + (remainingBarWidth - xpTextWidth) / 2,
                barStartY + 2,
                16,
                Color.White
            );
        }
    }

    private void DrawCoins(int x, int y)
    {
        string coinText = $"Coins: {PlayerStats.Coins}";
        DrawText(coinText, x, y, FontSize, Color.Yellow);
    }
}