using System;
using System.Collections.Generic;
using BlackCatAdventure.Constants;
using BlackCatAdventure.Interfaces;
using Godot;

namespace BlackCatAdventure.models;

public enum OrbitalBodyType
{
    Star,
    CentralPlanet,
    Planet,
    Moon,
    DwarfPlanet,
    AsteroidBelt
}

public class OrbitalBody : IGameJsonSerializable
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string IconPath { get; set; }
    public OrbitalBodyType Type { get; set; }
    public double OrbitalRadius { get; set; } = 0;
    public double OrbitalSpeed { get; set; } = 0;
    public double InitialAngle { get; set; } = 0;
    public string? ParentId { get; set; }

    public double PhysicalRadius { get; set; } = 0;
    public double Density { get; set; } = 0;
    public double Mass { get; set; } = 0;

    // Asteroid belts only
    public double InnerRadius { get; set; }
    public double OuterRadius { get; set; }

    public void SetOrbitalBodyPhysicsProperties(
        Random randomSeed,
        int index,
        double baseDistance,
        double spacingFactor,
        double starMass,
        int planetCount)
    {
        PhysicalRadius = PhysicalRadius > 0
            ? PhysicalRadius
            : randomSeed.NextDouble() * (
                PhysicsConstants.MaxRadius - PhysicsConstants.MinRadius) + PhysicsConstants.MinRadius;
        Density = Density > 0
            ? Density
            : randomSeed.NextDouble() * (
                PhysicsConstants.MaxDensity - PhysicsConstants.MinDensity) + PhysicsConstants.MinDensity;
        Mass = Density * (4.0 / 3.0) * Math.PI * Math.Pow(PhysicalRadius, 3);
        Mass = Math.Clamp(Mass, 10, 20);
        
        if (OrbitalRadius == 0)
        {
            OrbitalRadius = baseDistance * Math.Pow(spacingFactor, index);
            OrbitalSpeed = Math.Sqrt(starMass / OrbitalRadius) * PhysicsConstants.GravitationalConstant;
            InitialAngle = (2 * Math.PI / planetCount) * index +
                           (randomSeed.NextDouble() * PhysicsConstants.SpreadVariance);
        }

        // GD.Print($"Physical radius: {PhysicalRadius} \n\n" +
        //          $"Density: {Density} \n\n" +
        //          $"Mass: {Mass} \n\n");
    }
}

public class SolarSystem : IGameJsonSerializable
{
    public string Id { get; set; }
    public string Name { get; set; }
    public OrbitalBody Star { get; set; }
    public List<OrbitalBody> OrbitalBodies { get; set; } = new();
}