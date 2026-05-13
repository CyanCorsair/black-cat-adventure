using Godot;
using System;
using BlackCatAdventure.models;
using BlackCatAdventure.Services;

namespace BlackCatAdventure.UI;

public partial class MainMenuButtonsController : BoxContainer
{
    private Button _newGameButton;
    private Button _saveGameButton;
    private Button _loadGameButton;
    private Button _optionsButton;
    private Button _exitToMenuGameButton;
    private Button _exitToDesktopGameButton;

    private BasicEventBus _eventBus;

    public override void _Ready()
    {
        _newGameButton = GetNode<Button>("Button_NewGame");
        _saveGameButton = GetNode<Button>("Button_SaveGame");
        _loadGameButton = GetNode<Button>("Button_LoadGame");
        _optionsButton = GetNode<Button>("Button_Options");
        _exitToMenuGameButton = GetNode<Button>("Button_ExitToMenu");
        _exitToDesktopGameButton = GetNode<Button>("Button_ExitToDesktop");
        
        _newGameButton.Pressed += OnNewGameButtonPressed;
        _saveGameButton.Pressed += OnSaveGameButtonPressed;
        _loadGameButton.Pressed += OnLoadGameButtonPressed;
        _optionsButton.Pressed += OnOptionsButtonPressed;
        _exitToMenuGameButton.Pressed += OnExitGameToMenuButtonPressed;
        _exitToDesktopGameButton.Pressed += OnExitGameToDesktopButtonPressed;

        if (!ServicesProvider.Instance.IsInGame)
        {
            _saveGameButton.Visible = false;
            _exitToMenuGameButton.Visible = false;
        }
        else
        {
            _saveGameButton.Visible = true;
            _exitToMenuGameButton.Visible = true;
        }
        
        _eventBus = ServicesProvider.Instance.GetService<BasicEventBus>();
    }

    private void OnNewGameButtonPressed()
    {
        GD.Print("Opening GameWorld2d");
        PackedScene gameWorld2D = GD.Load<PackedScene>("res://scenes/game_world2d.tscn");
        GameWorld2D sceneInstance = gameWorld2D.Instantiate<GameWorld2D>();
        FindParent("GameRoot").AddChild(sceneInstance);
        GD.Print("Added gameworld2d to root");
        
        GD.Print("Removing main menu from scene");
        MainMenu mainMenuInstance = GetTree().Root.GetNode<MainMenu>("GameRoot/MainMenu");
        mainMenuInstance.Visible = false;
        mainMenuInstance.QueueFree();
        GD.Print("Removed main menu from scene");
    }
    
    private void OnSaveGameButtonPressed()
    {
        var solarSystem = ServicesProvider.Instance.CurrentSolarSystem;
        if (solarSystem is null)
        {
            GD.PrintErr("Cannot save: no active solar system");
            return;
        }
        _eventBus.Publish(new GameEvents.SaveGameEvent()
        {
            SaveGameTitle = ServicesProvider.Instance.DesiredSaveGame,
            SolarSystem = solarSystem
        });
    }

    private void OnLoadGameButtonPressed()
    {
        GD.Print("Saved games list");
    }

    private void OnOptionsButtonPressed()
    {
        GD.Print("Options");
    }
    
    private void OnExitGameToMenuButtonPressed()
    {
        GD.Print("Opening MainMenu");
        PackedScene gameWorld2D = GD.Load<PackedScene>("res://scenes/main_menu.tscn");
        MainMenu sceneInstance = gameWorld2D.Instantiate<MainMenu>();
        GetTree().Root.AddChild(sceneInstance);
        GD.Print("Added MainMenu to root");
        
        GD.Print("Removing gameworld2d from scene");
        GameWorld2D mainMenuInstance = GetTree().Root.GetNode<GameWorld2D>("GameRoot/GameWorld");
        mainMenuInstance.Visible = false;
        mainMenuInstance.QueueFree();
        GD.Print("Removed gameworld2d from scene");
    }

    private void OnExitGameToDesktopButtonPressed()
    {
        GD.Print("Exiting Game");
        GetTree().Quit();
    }
}
