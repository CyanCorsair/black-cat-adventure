using System.IO;
using System.Text.Json;

namespace BlackCatAdventure.Interfaces;

public interface IGameJsonSerializable
{
    public static T FromJson<T>(string inputPath) where T : IGameJsonSerializable
    {
        var resourceDefinition = File.ReadAllText(inputPath);
        return JsonSerializer.Deserialize<T>(resourceDefinition)!;
    }

    public static void ToJson<T>(string inputPath, T targetData) where T : IGameJsonSerializable
    {
        var json = JsonSerializer.Serialize(targetData, new JsonSerializerOptions() {WriteIndented = true});
        File.WriteAllText(inputPath, json);
    }
}