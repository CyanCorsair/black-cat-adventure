using Godot;
using Microsoft.Extensions.DependencyInjection;

namespace BlackCatAdventure;

public partial class ServicesProvider : Node
{
    private static ServiceProvider _serviceProvider;
    private static IServiceCollection _services;
    
    public static ServicesProvider Instance { get; private set; }
    public bool IsInGame = false;

    public override void _Ready()
    {
        Instance = this;
        
        _services = new ServiceCollection();
        
        // Add services here
        
        _serviceProvider = _services.BuildServiceProvider();
    }

    public static T GetService<T>()
    {
        if (_serviceProvider == null)
        {
            GD.PrintErr("ServiceProvider not initialized");
            return default;
        }
        
        return _serviceProvider.GetService<T>();
    }
}