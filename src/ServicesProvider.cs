using System;
using BlackCatAdventure.models;
using BlackCatAdventure.Services;
using Godot;
using Microsoft.Extensions.DependencyInjection;

namespace BlackCatAdventure;

public partial class ServicesProvider : Node
{
    private static ServiceProvider _serviceProvider;
    private static IServiceCollection _services;
    
    public static ServicesProvider Instance { get; private set; }
    public bool IsInGame = false;
    public string DesiredSaveGame = String.Empty;
    public SolarSystem CurrentSolarSystem = null;
    

    public override void _Ready()
    {
        Instance = this;
        
        _services = new ServiceCollection();
        var eventBus = new BasicEventBus();

        _services.AddSingleton(eventBus);
        _services.AddSingleton<SaveLoadService>();
        
        _serviceProvider = _services.BuildServiceProvider();
        _serviceProvider.GetService<SaveLoadService>(); // eagerly instantiate so event subscriptions are registered
    }

    public T GetService<T>()
    {
        if (_serviceProvider == null)
        {
            GD.PrintErr("ServiceProvider not initialized");
            return default;
        }
        
        return _serviceProvider.GetService<T>();
    }
}