using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using Arch.Core;
using Isotope.Core.Components;

namespace Isotope.Core.Serialization
{
    public static class SceneSerializer
    {
        public class EntityData
        {
            public string Name { get; set; }
            public TransformComponent? Transform { get; set; }
            public SpriteComponent? Sprite { get; set; }
            public LightSource? Light { get; set; }
        }

        public static void SaveScene(World world, string path)
        {
            var entitiesToSave = new List<EntityData>();
            var query = new QueryDescription().WithAll<TransformComponent>();

            world.Query(in query, (Entity e, ref TransformComponent t) =>
            {
                var data = new EntityData();
                data.Transform = t;

                if (world.Has<NameComponent>(e)) data.Name = world.Get<NameComponent>(e).Name;
                if (world.Has<SpriteComponent>(e)) data.Sprite = world.Get<SpriteComponent>(e);
                if (world.Has<LightSource>(e)) data.Light = world.Get<LightSource>(e);

                entitiesToSave.Add(data);
            });

            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(entitiesToSave, options);

            // Ensure directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, json);

            Console.WriteLine($"Scene saved to {path} ({entitiesToSave.Count} entities)");
        }

        public static void LoadScene(World world, string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine($"[ERROR] Scene file not found: {path}");
                return;
            }

            string json = File.ReadAllText(path);
            var loadedEntities = JsonSerializer.Deserialize<List<EntityData>>(json);

            // Clear existing entities before loading
            world.Clear();

            foreach (var data in loadedEntities)
            {
                var e = world.Create();

                if (data.Transform.HasValue) world.Add(e, data.Transform.Value);
                if (data.Sprite.HasValue) world.Add(e, data.Sprite.Value);
                if (data.Light.HasValue) world.Add(e, data.Light.Value);

                string name = data.Name ?? "Unnamed";
                world.Add(e, new NameComponent { Name = name });
            }

            Console.WriteLine($"Scene loaded from {path} ({loadedEntities.Count} entities)");
        }
    }
}
