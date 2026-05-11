namespace BlackCatAdventure.models;

public enum SaveGameResultStates
{
    SUCCEEDED,
    FAILED_ERRORED
}

public enum LoadGameResultStates
{
    SUCCEEDED,
    FAILED_ERRORED
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