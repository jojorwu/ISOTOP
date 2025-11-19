using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Isotope.Core.Prototypes;

/// <summary>
/// Provides functionality for loading entity prototypes from files.
/// </summary>
public static class PrototypeLoader
{
    /// <summary>
    /// Loads all entity prototypes from the specified directory.
    /// </summary>
    /// <param name="path">The path to the directory containing the prototype files.</param>
    public static void LoadPrototypesFromDirectory(string path)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        foreach (var file in Directory.EnumerateFiles(path, "*.yaml", SearchOption.AllDirectories))
        {
            try
            {
                var content = File.ReadAllText(file);
                var prototypes = deserializer.Deserialize<List<EntityPrototype>>(content);

                if (prototypes == null) continue;

                foreach (var prototype in prototypes)
                {
                    PrototypeManager.Register(prototype);
                    Console.WriteLine($"[Prototype] Registered '{prototype.Id}' from {Path.GetFileName(file)}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR] Failed to load prototypes from {file}: {e.Message}");
            }
        }
    }
}
