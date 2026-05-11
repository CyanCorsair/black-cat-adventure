using System.IO;
using System.Text.Json;
using Godot;
using FileAccess = Godot.FileAccess;

namespace BlackCatAdventure.Interfaces;

public interface IGameJsonSerializable
{
    public static T FromJson<T>(string inputPath) where T : IGameJsonSerializable
    {
        using var saveFile = FileAccess.Open(inputPath, FileAccess.ModeFlags.Read);
        return JsonSerializer.Deserialize<T>(saveFile.GetLine())!;
    }

    public static void ToJson<T>(string inputPath, T targetData) where T : IGameJsonSerializable
    {
        try
        {
            var json = JsonSerializer.Serialize(targetData, new JsonSerializerOptions() { WriteIndented = true });
            using var saveFile = FileAccess.Open(inputPath, FileAccess.ModeFlags.WriteRead);
            saveFile.StoreLine(json);
        }
        catch
        {
            GD.PrintErr("Could not write to file: " + inputPath);
            throw;
        }
    }
}