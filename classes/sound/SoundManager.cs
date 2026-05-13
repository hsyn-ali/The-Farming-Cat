using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Game;

public static class SoundManager
{
    public static void LoadAll()
    {
        InitAudioDevice();

        RunTime.PopupSound = LoadSound("resources/assets/sounds/popup.wav");
        RunTime.ClickSound = LoadSound("resources/assets/sounds/click.wav");
        RunTime.PlantSound = LoadSound("resources/assets/sounds/plant.wav");
        RunTime.HarvestSound = LoadSound("resources/assets/sounds/harvest.wav");
        RunTime.BuySellSound = LoadSound("resources/assets/sounds/buysell.mp3");
        RunTime.ChickenSound = LoadSound("resources/assets/sounds/chicken.wav");
        RunTime.CowSound = LoadSound("resources/assets/sounds/cow.wav");
        RunTime.SheepSound = LoadSound("resources/assets/sounds/sheep.wav");
        RunTime.BackgroundMusic = LoadMusicStream("resources/assets/sounds/music.wav");
        SetMusicVolume(RunTime.BackgroundMusic, 0.4f);
        PlayMusicStream(RunTime.BackgroundMusic);
    }

    public static void Update()
    {
        UpdateMusicStream(RunTime.BackgroundMusic);
    }

    public static void Play(Sound sound)
    {
        PlaySound(sound);
    }

    public static void UnloadAll()
    {
        UnloadSound(RunTime.PopupSound);
        UnloadSound(RunTime.ClickSound);
        UnloadSound(RunTime.PlantSound);
        UnloadSound(RunTime.HarvestSound);
        UnloadSound(RunTime.BuySellSound);
        UnloadSound(RunTime.ChickenSound);
        UnloadSound(RunTime.CowSound);
        UnloadSound(RunTime.SheepSound);
        UnloadMusicStream(RunTime.BackgroundMusic);
        CloseAudioDevice();
    }
}