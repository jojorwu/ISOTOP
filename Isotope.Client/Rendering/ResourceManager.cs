using System.Collections.Generic;
using Raylib_cs;

namespace Isotope.Client.Rendering
{
    /// <summary>
    /// A static class for managing and caching game resources, like textures.
    /// </summary>
    public static class ResourceManager
    {
        private static Dictionary<string, Texture2D> _textureCache = new();

        /// <summary>
        /// Retrieves a texture from a given path, loading it from disk if it's not already in the cache.
        /// </summary>
        /// <param name="path">The file path of the texture.</param>
        /// <returns>The loaded Texture2D.</returns>
        public static Texture2D GetTexture(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return GetDefaultTexture();
            }

            if (_textureCache.TryGetValue(path, out var texture))
            {
                return texture;
            }

            if (System.IO.File.Exists(path))
            {
                var newTexture = Raylib.LoadTexture(path);
                _textureCache[path] = newTexture;
                return newTexture;
            }

            // Return a default texture if the path is invalid to avoid crashes
            return GetDefaultTexture();
        }

        /// <summary>
        /// Gets a default placeholder texture.
        /// </summary>
        private static Texture2D GetDefaultTexture()
        {
            // A simple magenta texture to indicate a missing asset
            if (_textureCache.TryGetValue("!default", out var defaultTex))
            {
                return defaultTex;
            }

            var image = Raylib.GenImageColor(32, 32, Color.Magenta);
            var newDefault = Raylib.LoadTextureFromImage(image);
            Raylib.UnloadImage(image);
            _textureCache["!default"] = newDefault;
            return newDefault;
        }

        /// <summary>
        /// Unloads all cached textures from memory.
        /// </summary>
        public static void UnloadAll()
        {
            foreach (var texture in _textureCache.Values)
            {
                Raylib.UnloadTexture(texture);
            }
            _textureCache.Clear();
        }
    }
}
