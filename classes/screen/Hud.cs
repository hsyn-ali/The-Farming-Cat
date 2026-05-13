using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Game;

public class HUD
{
    private const int BarWidth = 300;
    private const int BarHeight = 20;
    private const int Margin = 20;
    private const int FontSize = 22;

    // Cached values updated only when events fire (Observer pattern)
    private int _cachedLevel;
    private int _cachedXP;
    private int _cachedCoins;
    private string? _levelUpMessage;
    private float _levelUpTimer;

    public HUD()
    {
        // Subscribe to PlayerStats events
        PlayerStats.OnXPChanged += HandleXPChanged;
        PlayerStats.OnCoinsChanged += HandleCoinsChanged;
        PlayerStats.OnLevelUp += HandleLevelUp;

        // Initialize cache
        _cachedLevel = PlayerStats.Level;
        _cachedXP = PlayerStats.CurrentXP;
        _cachedCoins = PlayerStats.Coins;
    }

    private void HandleXPChanged()
    {
        _cachedXP = PlayerStats.CurrentXP;
        _cachedLevel = PlayerStats.Level;
    }

    private void HandleCoinsChanged()
    {
        _cachedCoins = PlayerStats.Coins;
    }

    private void HandleLevelUp(int newLevel)
    {
        _levelUpMessage = $"Level Up! You are now Level {newLevel}";
        _levelUpTimer = 3f;  // Show for 3 seconds
    }

    public void Update(float dt)
    {
        if (_levelUpTimer > 0)
            _levelUpTimer -= dt;
    }

    public void Draw(int screenWidth, int screenHeight)
    {
        int barX = screenWidth - BarWidth - Margin;
        int barY = Margin;

        DrawLevelAndXP(barX, barY);
        DrawCoins(barX, barY + BarHeight + 30);
        DrawLevelUpMessage(screenWidth, screenHeight);
    }

    private void DrawLevelAndXP(int x, int y)
    {
        string levelText = PlayerStats.IsMaxLevel
            ? "MAX LEVEL"
            : $"Level {_cachedLevel}";

        DrawText(levelText, x, y, FontSize, Color.White);

        int labelWidth = MeasureText(levelText, FontSize);

        int barStartX = x + labelWidth + 12;
        int barStartY = y + 2;
        int remainingBarWidth = BarWidth - labelWidth - 12;

        DrawRectangle(barStartX, barStartY, remainingBarWidth, BarHeight, new Color(50, 50, 50, 200));

        if (!PlayerStats.IsMaxLevel)
        {
            int xpMin = PlayerStats.XpForCurrentLevel;
            int xpMax = PlayerStats.XpForNextLevel;
            float progress = (float)(_cachedXP - xpMin) / (xpMax - xpMin);
            progress = System.Math.Clamp(progress, 0f, 1f);

            int fillWidth = (int)(remainingBarWidth * progress);
            DrawRectangle(barStartX, barStartY, fillWidth, BarHeight, new Color(80, 180, 80, 255));
        }

        DrawRectangleLines(barStartX, barStartY, remainingBarWidth, BarHeight, Color.White);

        if (!PlayerStats.IsMaxLevel)
        {
            string xpText = $"{_cachedXP} / {PlayerStats.XpForNextLevel} XP";
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
        string coinText = $"Coins: {_cachedCoins}";
        DrawText(coinText, x, y, FontSize, Color.Yellow);
    }

    private void DrawLevelUpMessage(int screenW, int screenH)
    {
        if (_levelUpTimer <= 0 || _levelUpMessage == null) return;

        int fontSize = 40;
        int textWidth = MeasureText(_levelUpMessage, fontSize);
        int x = (screenW - textWidth) / 2;
        int y = screenH / 4;

        // Fade out near the end
        byte alpha = _levelUpTimer < 1f
            ? (byte)(255 * _levelUpTimer)
            : (byte)255;

        DrawRectangle(x - 20, y - 10, textWidth + 40, fontSize + 20, new Color((byte)0, (byte)0, (byte)0, (byte)(alpha / 2)));
        DrawText(_levelUpMessage, x, y, fontSize, new Color((byte)255, (byte)215, (byte)0, alpha));
    }
}