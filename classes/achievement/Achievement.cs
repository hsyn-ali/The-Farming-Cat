namespace Game;

public class Achievement
{
    public string Title { get; }
    public string Description { get; }
    public bool IsUnlocked { get; private set; }

    public Achievement(string title, string description)
    {
        Title = title;
        Description = description;
        IsUnlocked = false;
    }

    public void Unlock()
    {
        IsUnlocked = true;
    }
}