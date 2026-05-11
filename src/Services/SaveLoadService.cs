using System;
using BlackCatAdventure.Interfaces;
using BlackCatAdventure.models;
using Godot;

namespace BlackCatAdventure.Services;

public class SaveLoadService
{
    private readonly BasicEventBus _eventBus;

    public SaveLoadService()
    {
        _eventBus = ServicesProvider.Instance.GetService<BasicEventBus>();
        _eventBus.Subscribe<GameEvents.SaveGameEvent>(saveGameEvent => SaveGame(
            saveGameEvent.SolarSystem, saveGameEvent.SaveGameTitle));
        _eventBus.Subscribe<GameEvents.LoadGameEvent>(loadGameEvent => LoadGame(
            loadGameEvent.LoadGameTitle));
    }

    private void SaveGame(SolarSystem systemState, string saveName)
    {
        var saveGameEndEvent = new GameEvents.SaveGameEndEvent()
        {
            SaveGameTitle = saveName
        };

        try
        {
            IGameJsonSerializable.ToJson($"user://gameSave_{saveName}", systemState);
            saveGameEndEvent.Status = SaveGameResultStates.SUCCEEDED;
        }
        catch (Exception exception)
        {
            GD.PrintErr(exception);
            saveGameEndEvent.Status = SaveGameResultStates.FAILED_ERRORED;
        }
        
        _eventBus.Publish(saveGameEndEvent);
    }

    private void LoadGame(string saveName)
    {
        var loadGameEndEvent = new GameEvents.LoadGameEndEvent()
        {
            LoadGameTitle = saveName
        };

        try
        {
            SolarSystem saveData = IGameJsonSerializable.FromJson<SolarSystem>($"user://gameSave_{saveName}");
            loadGameEndEvent.SolarSystem = saveData;
            loadGameEndEvent.Status = LoadGameResultStates.SUCCEEDED;
        }
        catch (Exception exception)
        {
            GD.PrintErr(exception);
            loadGameEndEvent.Status = LoadGameResultStates.FAILED_ERRORED;
        }
        
        _eventBus.Publish(loadGameEndEvent);
    }
}