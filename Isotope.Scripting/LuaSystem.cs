using Isotope.Core.Scripting;
using NLua;
using System;
using System.IO;

namespace Isotope.Scripting.Lua;

/// <summary>
/// A system that manages the Lua scripting environment.
/// </summary>
public class LuaSystem
{
    private Lua _lua;
    private EngineApi _api;

    /// <summary>
    /// Initializes a new instance of the <see cref="LuaSystem"/> class.
    /// </summary>
    /// <param name="api">The engine API to expose to Lua scripts.</param>
    public LuaSystem(EngineApi api)
    {
        _api = api;
        _lua = new Lua();
        _lua.LoadCLRPackage();
        _lua["Game"] = _api;
    }

    /// <summary>
    /// Loads and executes all Lua scripts from the specified directory.
    /// </summary>
    /// <param name="path">The path to the directory containing the Lua scripts.</param>
    public void LoadScriptsFromDirectory(string path)
    {
        foreach (var file in Directory.EnumerateFiles(path, "*.lua", SearchOption.AllDirectories))
        {
            try
            {
                var scriptContent = File.ReadAllText(file);
                _lua.DoString(scriptContent);
                Console.WriteLine($"[Lua] Executed script '{Path.GetFileName(file)}'");
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Lua ERROR] Failed to execute {file}: {e.Message}");
            }
        }
    }
}
