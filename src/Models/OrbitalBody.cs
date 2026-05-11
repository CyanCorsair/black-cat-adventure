using System;
using System.Collections.Generic;
using BlackCatAdventure.Constants;
using BlackCatAdventure.Interfaces;
using BlackCatAdventure.Services;
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
    public double Eccentricity { get; set; } = 0;

    public double PhysicalRadius { get; set; } = 0;
    public double Density { get; set; } = 0;
    public double Mass { get; set; } = 0;

    // Asteroid belts only
    public double InnerRadius { get; set; }
    public double OuterRadius { get; set; }

    public void SetOrbitalBodyPhysicsProperties(SolarSystemGenerationContext context, int index)
    {
        SetBasePhysicsProperties(context.Random);
        SetOrbitalProperties(context, index);

        GD.Print($"Physical radius: {PhysicalRadius} \n\n" +
                 $"Density: {Density} \n\n" +
                 $"Mass: {Mass} \n\n");
    }

    private void SetBasePhysicsProperties(Random randomSeed)
    {
        PhysicalRadius = PhysicalRadius > 0 // If set, is central star/main planet; use provided val
            ? PhysicalRadius
            : randomSeed.NextDouble() * (
                PhysicsConstants.MaxRadius - PhysicsConstants.MinRadius) + PhysicsConstants.MinRadius;
        Density = Density > 0 // If set, is central star/main planet; use provided val
            ? Density
            : randomSeed.NextDouble() * (
                PhysicsConstants.MaxDensity - PhysicsConstants.MinDensity) + PhysicsConstants.MinDensity;
        Mass = Mass > 0 // If set, use provided val; just keep it consistent
            ? Mass
            : Density * (4.0 / 3.0) * Math.PI * Math.Pow(PhysicalRadius, 3);
        Eccentricity = randomSeed.NextDouble() * PhysicsConstants.MaxEccentricity;
    }

    private void SetOrbitalProperties(SolarSystemGenerationContext context, int index)
    {
        if (OrbitalRadius == 0)
        {
            OrbitalRadius = Math.Clamp(
                context.BaseDistance * Math.Pow(context.SpacingFactor, index),
                PhysicsConstants.MinOrbitalRadius,
                PhysicsConstants.MaxOrbitalRadius);
            OrbitalSpeed = Math.Sqrt(context.StarMass / OrbitalRadius) * PhysicsConstants.GravitationalConstant;
            InitialAngle = (2 * Math.PI / context.PlanetCount) * index +
                           (context.Random.NextDouble() * PhysicsConstants.SpreadVariance);
        }
    }
}

public class SolarSystem : IGameJsonSerializable
{
    public string Id { get; set; }
    public string Name { get; set; }
    public OrbitalBody Star { get; set; }
    public List<OrbitalBody> OrbitalBodies { get; set; } = new();
}