using Godot;
using System;
using System.Collections.Generic;
using BlackCatAdventure.Services;

namespace BlackCatAdventure;

public partial class GameHud : Control
{
    private PackedScene _notificationTemplate;
    private PackedScene _objectInfoTemplate;

    private const int MaxOpenNotifications = 3;
    private const int MaxOpenInfoWindows = 5;
    private List<Control> _openNotifications;
    private List<Control> _openInfoWindows;
    
    private GuiEventBus GuiEventBus { get; set; }

    public override void _Ready()
    {
        GuiEventBus = ServicesProvider.Instance.GetService<GuiEventBus>();
    }
    
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Close"))
        {
            GD.Print("Open game menu");
        }
    }
}
