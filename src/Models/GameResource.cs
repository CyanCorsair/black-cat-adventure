using System;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using BlackCatAdventure.Interfaces;

namespace BlackCatAdventure.models;

public interface IGameResource
{
    string Id { get; }
    string DisplayName { get; }
    GameResourceCategory Category { get; }
}

public enum GameResourceCategory { Raw, Processed, Manufactured }

public class GameResourceDefinition : IGameResource, IGameJsonSerializable
{
    public string Id { get; set; }
    public string DisplayName { get; set; }
    public GameResourceCategory Category { get; set; }
    public Dictionary<string, double> Requirements { get; set; } = new();
}