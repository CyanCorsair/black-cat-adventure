using Godot;
using System;
using System.Collections.Generic;
using BlackCatAdventure.Constants;
using BlackCatAdventure.GameObjects;
using BlackCatAdventure.Interfaces;
using BlackCatAdventure.models;
using BlackCatAdventure.Services;

namespace BlackCatAdventure;

public partial class GameWorld2d : Node2D
{
    private SolarSystem _solarSystem;
    private PackedScene _defaultScene = GD.Load<PackedScene>("res://scenes/components/default_solar_object.tscn");
    
    private Camera2D _camera;
    private TextureRect _backgroundSprite;

    private BasicEventBus _eventBus;
    
    public override void _Ready()
    {
        _eventBus = ServicesProvider.Instance.GetService<BasicEventBus>();
        _eventBus.Subscribe<GameEvents.SaveGameEndEvent>(saveGameEnd =>
        {
            if (saveGameEnd.Status != SaveGameResultStates.SUCCEEDED)
            {
                GD.PrintErr("Save game failed");
                throw new Exception("Saving game failed");
            }

            GD.Print("Save game succeeded");
        });
        
        _camera = GetNode<Camera2D>("GameCamera");
        var canvasLayer = new CanvasLayer();
        canvasLayer.Layer = -1; // behind everything

        _backgroundSprite = new TextureRect();
        _backgroundSprite.Texture = GD.Load<Texture2D>("res://assets/images/radar_grid_tile.svg");
        _backgroundSprite.StretchMode = TextureRect.StretchModeEnum.Tile;
        _backgroundSprite.TextureRepeat = TextureRepeatEnum.Enabled;
        _backgroundSprite.AnchorRight = 1;
        _backgroundSprite.AnchorBottom = 1; // stretch to fill viewport
        _backgroundSprite.OffsetRight = 0;
        _backgroundSprite.OffsetBottom = 0;

        canvasLayer.AddChild(_backgroundSprite);
        AddChild(canvasLayer);
        
        // Generate solar system
        Random mainRandom = new Random();
        int worldSeed = mainRandom.Next();
        try
        {
            SolarSystemGenerationService solarSystemGenerator = new SolarSystemGenerationService();
            _solarSystem = solarSystemGenerator.GenerateSolarSystem(worldSeed);
            ServicesProvider.Instance.CurrentSolarSystem = _solarSystem;
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            GD.Print($"Autosaving solar system {_solarSystem.Id}_{timestamp}.json");
            _eventBus.Publish(new GameEvents.SaveGameEvent()
            {
                SaveGameTitle = $"{_solarSystem.Id}_{timestamp}.json",
                SolarSystem = _solarSystem
            });
            GD.Print($"Autosave done");
            
            OrbitalBody centralStar = _solarSystem.Star;
            OrbitalBody centralPlanet = _solarSystem.OrbitalBodies.Find((body => body.Type == OrbitalBodyType.CentralPlanet));
            List<OrbitalBody> planets = _solarSystem.OrbitalBodies.FindAll((body => body.Type is OrbitalBodyType.Planet or OrbitalBodyType.DwarfPlanet));
            List<OrbitalBody> moons = _solarSystem.OrbitalBodies.FindAll((body => body.Type == OrbitalBodyType.Moon));
            
            DefaultSolarObject centralStarInstance = CreateNewSolarObject(centralStar);
            AddChild(centralStarInstance);
            centralStarInstance.Position = GetViewport().GetVisibleRect().GetCenter();
            
            DefaultSolarObject centralPlanetInstance = CreateNewSolarObject(centralPlanet);
            centralStarInstance.AddChild(centralPlanetInstance);
            var planetOrbit = centralPlanetInstance.CreateOrbitLine();
            centralStarInstance.AddChild(planetOrbit);

            if (planets is not null && planets.Count > 0)
            {
                int lastCountedMoonIndex = 0;
                
                // Generate regular planets/dwarf planets
                planets.ForEach((planet) =>
                {
                    DefaultSolarObject planetInstance = CreateNewSolarObject(planet);
                    int targetMoonCount = mainRandom.Next(GameplayConstants.MinMoons, GameplayConstants.MaxMoons);
                    centralStarInstance.AddChild(planetInstance);
                    var orbitLine = planetInstance.CreateOrbitLine();
                    centralStarInstance.AddChild(orbitLine);
            
                    // Fetch assigned moons
                    if (moons.Count <= 0) return;
                    var availableMoons = Math.Min(targetMoonCount, moons.Count - lastCountedMoonIndex);
                    if (availableMoons <= 0) return;
                    var assignedMoons = moons.Slice(lastCountedMoonIndex, availableMoons);
                    assignedMoons.ForEach((moon) =>
                    {
                        var moonInstance = CreateNewSolarObject(moon);
                        planetInstance.AddChild(moonInstance);
                        var moonOrbitLine = moonInstance.CreateOrbitLine();
                        planetInstance.AddChild(moonOrbitLine);
                    });
                    lastCountedMoonIndex += targetMoonCount;
                });
            }
            
        }
        catch (Exception exception)
        {
            GD.PrintErr(exception);
            throw;
        }
        
        GD.Print("Add game hud to scene");
        PackedScene gameHud = GD.Load<PackedScene>("res://scenes/game_hud.tscn");
        GameHud gameHudInstance = gameHud.Instantiate<GameHud>();
        AddChild(gameHudInstance);
        GD.Print("Game HUD added");

        ServicesProvider.Instance.IsInGame = true;
    }

    private DefaultSolarObject CreateNewSolarObject(OrbitalBody bodyDefinition)
    {
        if (bodyDefinition is null)
        {
            GD.PrintErr($"Body definition is null");
            return null;
        }

        DefaultSolarObject newSolarObject = _defaultScene.Instantiate<DefaultSolarObject>();
        newSolarObject.OrbitalBodyDefinition = bodyDefinition;
        newSolarObject.Id = bodyDefinition.Id;
        newSolarObject.Name = bodyDefinition.Id;
        
        return newSolarObject;
    }
}
