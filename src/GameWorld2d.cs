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
    
    public override void _Ready()
    {
        // Generate solar system
        Random mainRandom = new Random();
        int worldSeed = mainRandom.Next();
        try
        {
            SolarSystemGenerationService solarSystemGenerator = new SolarSystemGenerationService();
            _solarSystem = solarSystemGenerator.GenerateSolarSystem(worldSeed);
            IGameJsonSerializable.ToJson<SolarSystem>("./docs/solar_system.json", _solarSystem);
            OrbitalBody centralStar = _solarSystem.Star;
            OrbitalBody centralPlanet = _solarSystem.OrbitalBodies.Find((body => body.Type == OrbitalBodyType.CentralPlanet));
            List<OrbitalBody> planets = _solarSystem.OrbitalBodies.FindAll((body => body.Type is OrbitalBodyType.Planet or OrbitalBodyType.DwarfPlanet));
            List<OrbitalBody> moons = _solarSystem.OrbitalBodies.FindAll((body => body.Type == OrbitalBodyType.Moon));
            
            DefaultSolarObject centralStarInstance = CreateNewSolarObject(centralStar);
            AddChild(centralStarInstance);
            centralStarInstance.Position = GetViewport().GetVisibleRect().Size / 2;
            
            DefaultSolarObject centralPlanetInstance = CreateNewSolarObject(centralPlanet);
            centralStarInstance.AddChild(centralPlanetInstance);

            int lastCountedMoonIndex = 0;
            // Generate regular planets/dwarf planets
            planets.ForEach((planet) =>
            {
                DefaultSolarObject planetInstance = CreateNewSolarObject(planet);
                int targetMoonCount = mainRandom.Next(GameplayConstants.MinMoons, GameplayConstants.MaxMoons);
                centralStarInstance.AddChild(planetInstance);
            
                // Fetch assigned moons
                if (moons.Count <= 0) return;
                var availableMoons = Math.Min(targetMoonCount, moons.Count - lastCountedMoonIndex);
                var assignedMoons = moons.Slice(lastCountedMoonIndex, availableMoons);
                assignedMoons.ForEach((moon) =>
                {
                    var moonInstance = CreateNewSolarObject(moon);
                    planetInstance.AddChild(moonInstance);
                });
                lastCountedMoonIndex += targetMoonCount;
            });
        }
        catch (Exception exception)
        {
            GD.PrintErr(exception);
            return;
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
