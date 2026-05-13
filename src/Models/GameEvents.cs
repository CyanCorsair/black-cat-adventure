namespace BlackCatAdventure.models;

public enum SaveGameResultStates
{
    Succeeded,
    FailedErrored
}

public enum LoadGameResultStates
{
    Succeeded,
    FailedErrored
}

public static class GameEvents
{
    public class SaveGameEvent
    {
        public string SaveGameTitle;
        public SolarSystem SolarSystem;
    };

    public class SaveGameEndEvent: SaveGameEvent
    {
        public SaveGameResultStates Status;
    }

    public class LoadGameEvent
    {
        public string LoadGameTitle;
    };

    public class LoadGameEndEvent : LoadGameEvent
    {
        public LoadGameResultStates Status;
        public SolarSystem SolarSystem;
    }
}