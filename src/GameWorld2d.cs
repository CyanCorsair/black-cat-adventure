using Godot;
using System;

namespace BlackCatAdventure;

public partial class GameWorld2d : Node2D
{
    public override void _Ready()
    {
        GD.Print("Adding test map to scene");
        PackedScene testMap = GD.Load<PackedScene>("res://scenes/maps/test_map.tscn");
        Node2D testMapInstance = testMap.Instantiate<Node2D>();
        AddChild(testMapInstance);
        GD.Print("Added test map to scene");
        
        GD.Print("Add game hud to scene");
        PackedScene gameHud = GD.Load<PackedScene>("res://scenes/game_hud.tscn");
        GameHud gameHudInstance = gameHud.Instantiate<GameHud>();
        AddChild(gameHudInstance);
        GD.Print("Game HUD added");

        ServicesProvider.Instance.IsInGame = true;
    }
}
