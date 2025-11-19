using Isotope.Core.Scripting;
using NLua;
using System;
using System.IO;

namespace Isotope.Scripting.Lua;

public class LuaSystem
{
    private Lua _lua;
    private EngineApi _api;

    public LuaSystem(EngineApi api)
    {
        _api = api;
        _lua = new Lua();
        _lua.LoadCLRPackage();
        _lua["Game"] = _api;
    }

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
