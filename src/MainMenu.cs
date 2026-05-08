using Godot;
using System;

namespace BlackCatAdventure;

public partial class MainMenu : Control
{
    private Button _newGameButton;
    private Button _loadGameButton;
    private Button _optionsButton;
    private Button _exitGameButton;

    public override void _Ready()
    {
        _newGameButton = GetNode<Button>("Button_List/Button_NewGame");
        _loadGameButton = GetNode<Button>("Button_List/Button_LoadGame");
        _optionsButton = GetNode<Button>("Button_List/Button_Options");
        _exitGameButton = GetNode<Button>("Button_List/Button_Exit");
        
        _newGameButton.Pressed += OnNewGameButtonPressed;
        _loadGameButton.Pressed += OnLoadGameButtonPressed;
        _optionsButton.Pressed += OnOptionsButtonPressed;
        _exitGameButton.Pressed += OnExitGameButtonPressed;
    }

    private void OnNewGameButtonPressed()
    {
        GD.Print("New Game");
    }

    private void OnLoadGameButtonPressed()
    {
        GD.Print("Saved games list");
    }

    private void OnOptionsButtonPressed()
    {
        GD.Print("Options");
    }

    private void OnExitGameButtonPressed()
    {
        GD.Print("Exiting Game");
        GetTree().Quit();
    }
}
