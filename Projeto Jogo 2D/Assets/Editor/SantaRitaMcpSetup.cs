using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class SantaRitaMcpSetup
{
    static SantaRitaMcpSetup()
    {
        if (Application.isBatchMode) return;
        EditorApplication.update += WaitForEditor;
    }

    private static void WaitForEditor()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
        EditorApplication.update -= WaitForEditor;
        Configure();
    }

    [MenuItem("Santa Rita/Ferramentas/Conectar Unity MCP local")]
    public static async void Configure()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
        try
        {
            EditorPrefs.SetBool("MCPForUnity.UseHttpTransport", true);
            EditorPrefs.SetString("MCPForUnity.HttpUrl", "http://127.0.0.1:8080");
            EditorPrefs.SetBool("MCPForUnity.AutoStartOnLoad", true);
            var tools = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.santarita";
            EditorPrefs.SetString("MCPForUnity.UvxPath", tools + "/bin/uvx.exe");
            var path = Environment.GetEnvironmentVariable("PATH") ?? "";
            if (!path.Contains(tools + "/bin"))
                Environment.SetEnvironmentVariable("PATH", tools + "/bin;" + tools + "/python/cpython-3.12.15-windows-x86_64-none;" + path);
            Environment.SetEnvironmentVariable("UV_PYTHON_INSTALL_DIR", tools + "/python");
            Environment.SetEnvironmentVariable("UV_CACHE_DIR", tools + "/uv-cache");
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var cache = assemblies.Select(a => a.GetType("MCPForUnity.Editor.Services.EditorConfigurationCache")).FirstOrDefault(t => t != null);
            var locator = assemblies.Select(a => a.GetType("MCPForUnity.Editor.Services.MCPServiceLocator")).FirstOrDefault(t => t != null);
            if (cache == null || locator == null) return; // Package Manager pode estar resolvendo a instalação.
            var config = cache.GetProperty("Instance").GetValue(null);
            cache.GetMethod("Refresh").Invoke(config, null);
            var bridge = locator.GetProperty("Bridge").GetValue(null);
            if (!(bool)bridge.GetType().GetProperty("IsRunning").GetValue(bridge))
                await (Task<bool>)bridge.GetType().GetMethod("StartAsync").Invoke(bridge, null);
        }
        catch (Exception error) { Debug.LogWarning("Santa Rita MCP: " + error.Message); }
    }
}
