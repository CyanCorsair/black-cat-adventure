using System;
using BlackCatAdventure.Constants;
using BlackCatAdventure.models;
using Godot;

namespace BlackCatAdventure.Services;

public class SolarSystemGenerationContext
{
    public Random Random { get; init; }
    public double StarMass { get; init; }
    public int PlanetCount { get; init; }
    public double BaseDistance { get; init; }
    public double SpacingFactor { get; init; }
    public int? WorldSeed { get; init; }
}

public class SolarSystemGenerationService
{
    private readonly double _baseDistance = 100.0;
    private readonly double _spacingFactor = 1.6;

    private OrbitalBody _centralStar;
    
    public const int PlanetCount = 12;

    private SolarSystemGenerationContext _context;
    
    public SolarSystem GenerateSolarSystem(int? seed = null)
    {
        var randomSeed = seed.HasValue ? new Random(seed.Value) : new Random();
        SolarSystem solarSystem = new()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Solar System",
        };
        solarSystem.Star = GenerateSystemStar(randomSeed);
        _centralStar = solarSystem.Star;
        
        _context = new SolarSystemGenerationContext()
        {
            Random = randomSeed,
            BaseDistance = _baseDistance,
            SpacingFactor =  _spacingFactor,
            StarMass = _centralStar.Mass,
            PlanetCount = PlanetCount,
            WorldSeed = seed
        };
        
        // Generate central planet
        var centralPlanet = GenerateCentralPlanet(randomSeed);
        solarSystem.OrbitalBodies.Add(centralPlanet);

        for (int i = 1; i < PlanetCount; i++)
        {
            solarSystem.OrbitalBodies.Add(
                GenerateOrbitalBody(randomSeed, i));
        }
        
        return solarSystem;
    }

    private OrbitalBody GenerateSystemStar(Random random)
    {
        OrbitalBody centralStar = new OrbitalBody()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Central Star",
            PhysicalRadius = random.NextDouble() * (
                    PhysicsConstants.MaxStarRadius - PhysicsConstants.MinStarRadius
                ) + PhysicsConstants.MinStarRadius,
            Density = random.NextDouble() * (
                  PhysicsConstants.MaxStarDensity - PhysicsConstants.MinStarDensity) +
              PhysicsConstants.MinStarDensity,
            OrbitalRadius = -1,
            OrbitalSpeed = -1,
            InitialAngle = -1
        };

        centralStar.SetOrbitalBodyPhysicsProperties(
            _context,
            0);

        centralStar.IconPath = GameplayConstants.StationIconPath;
        centralStar.Type = OrbitalBodyType.Star;
        return centralStar;
    }

    private OrbitalBody GenerateCentralPlanet(Random random)
    {
        var centralPlanet = new OrbitalBody()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Planet",
            PhysicalRadius =
                random.NextDouble() * (PhysicsConstants.MaxHabitableRadius - PhysicsConstants.MinHabitableRadius) +
                PhysicsConstants.MinHabitableRadius,
            Density = random.NextDouble() * (
                          PhysicsConstants.MaxHabitableDensity - PhysicsConstants.MinHabitableDensity) +
                      PhysicsConstants.MinHabitableDensity,
            ParentId = _centralStar.Id,
        };
        centralPlanet.SetOrbitalBodyPhysicsProperties(
            _context,
            1);
        
        centralPlanet.IconPath = GameplayConstants.PlanetIconPath;
        centralPlanet.Type = OrbitalBodyType.CentralPlanet;
        
        return centralPlanet;
    }
    

    private OrbitalBody GenerateOrbitalBody(Random random, int index)
    {
        OrbitalBody newPlanet = new OrbitalBody()
        {
            Id = Guid.NewGuid().ToString(),
            Name = $"New Planet #{index}",
            Type = OrbitalBodyType.Planet,
        };
            
        newPlanet.SetOrbitalBodyPhysicsProperties(
            _context,
            index);

        newPlanet.IconPath = GameplayConstants.PlanetIconPath;
        
        if (newPlanet.Mass < PhysicsConstants.MediumPlanetMassBreakpoint)
        {
            newPlanet.Type = OrbitalBodyType.Moon;
            newPlanet.IconPath = GameplayConstants.MoonIconPath;
            newPlanet.OrbitalRadius = Math.Clamp(
                newPlanet.OrbitalRadius,
                PhysicsConstants.MinMoonOrbitalRadius,
                PhysicsConstants.MaxMoonOrbitalRadius);
        }
        else if (newPlanet.Mass >= PhysicsConstants.MediumPlanetMassBreakpoint &&
                 newPlanet.Mass < PhysicsConstants.LargePlanetMassBreakpoint)
        {
            newPlanet.Type = OrbitalBodyType.DwarfPlanet;
        }
        else if (newPlanet.Mass >= PhysicsConstants.LargePlanetMassBreakpoint)
        {
            newPlanet.Type = OrbitalBodyType.Planet;
        }
        
        return newPlanet;
    }
}