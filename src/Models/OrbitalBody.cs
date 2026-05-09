using System.Collections.Generic;
using BlackCatAdventure.Interfaces;

namespace BlackCatAdventure.models;

public enum OrbitalBodyType { Star, Planet, Moon, DwarfPlanet, AsteroidBelt }

public class OrbitalBody : IGameJsonSerializable
{
    public string Id { get; set; }
    public string Name { get; set; }
    public OrbitalBodyType Type { get; set; }
    public double OrbitalRadius { get; set; }
    public double OrbitalSpeed { get; set; }
    public double InitialAngle { get; set; }
    public string? ParentId { get; set; }

    // Asteroid belts only
    public double InnerRadius { get; set; }
    public double OuterRadius { get; set; }
}

public class SolarSystem : IGameJsonSerializable
{
    public string Id { get; set; }
    public string Name { get; set; }
    public OrbitalBody Star { get; set; }
    public List<OrbitalBody> OrbitalBodies { get; set; } = new();
}