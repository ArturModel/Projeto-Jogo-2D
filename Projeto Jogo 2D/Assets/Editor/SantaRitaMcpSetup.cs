using System;
using System.IO;
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
        ConectarManualmente();
    }

    [MenuItem("Santa Rita/Ferramentas/Conectar Unity MCP local")]
    public static async void ConectarManualmente()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
        try
        {
            ConfigureEnvironment();

#pragma warning disable UAC0005 // Bootstrap de inicialização MCP necessita inspecionar assemblies de pacotes
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
#pragma warning restore UAC0005
            var locator = assemblies.Select(a => a.GetType("MCPForUnity.Editor.Services.MCPServiceLocator")).FirstOrDefault(t => t != null);
            if (locator == null) return;

            var server = locator.GetProperty("Server")?.GetValue(null);
            var bridge = locator.GetProperty("Bridge")?.GetValue(null);
            if (server == null || bridge == null) return;

            bool isReachable = (bool)server.GetType().GetMethod("IsLocalHttpServerReachable").Invoke(server, null);
            if (!isReachable)
            {
                server.GetType().GetMethod("StartLocalHttpServer").Invoke(server, new object[] { true });
                await Task.Delay(1500);
            }

            bool isRunning = (bool)bridge.GetType().GetProperty("IsRunning").GetValue(bridge);
            if (!isRunning)
            {
                await (Task<bool>)bridge.GetType().GetMethod("StartAsync").Invoke(bridge, null);
            }
        }
        catch (Exception error)
        {
            Debug.LogWarning("Santa Rita MCP (manual): " + error.Message);
        }
    }

    public static void ConfigureEnvironment()
    {
        try
        {
            EditorPrefs.SetBool("MCPForUnity.UseHttpTransport", true);
            // Usar porta 8088 para evitar conflitos com aplicações web/Java/Spring Boot locais (que usam 8080)
            EditorPrefs.SetString("MCPForUnity.HttpUrl", "http://127.0.0.1:8088");
            EditorPrefs.SetBool("MCPForUnity.AutoStartOnLoad", true);
            EditorPrefs.SetBool("MCPForUnity.HttpServerLaunchConfirmed", true);

            bool isWindows = Application.platform == RuntimePlatform.WindowsEditor;
            string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            if (isWindows)
            {
                var tools = Path.Combine(home, ".santarita");
                var localBin = Path.Combine(home, ".local", "bin");
                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var wingetUv = Path.Combine(localAppData, @"Microsoft\WinGet\Packages\astral-sh.uv_Microsoft.Winget.Source_8wekyb3d8bbwe");

                string uvxPath = Path.Combine(tools, "bin", "uvx.exe");
                string localUvx = Path.Combine(localBin, "uvx.exe");
                string wingetUvx = Path.Combine(wingetUv, "uvx.exe");

                if (File.Exists(uvxPath))
                    EditorPrefs.SetString("MCPForUnity.UvxPath", uvxPath);
                else if (File.Exists(localUvx))
                    EditorPrefs.SetString("MCPForUnity.UvxPath", localUvx);
                else if (File.Exists(wingetUvx))
                    EditorPrefs.SetString("MCPForUnity.UvxPath", wingetUvx);

                var path = Environment.GetEnvironmentVariable("PATH") ?? "";
                string winPython = Path.Combine(tools, "python", "cpython-3.12.15-windows-x86_64-none");
                string winBin = Path.Combine(tools, "bin");
                string[] searchDirs = { winBin, winPython, localBin, wingetUv };
                foreach (var dir in searchDirs)
                {
                    if (Directory.Exists(dir) && !path.Contains(dir))
                        path = dir + ";" + path;
                }
                Environment.SetEnvironmentVariable("PATH", path);

                if (Directory.Exists(Path.Combine(tools, "python")))
                    Environment.SetEnvironmentVariable("UV_PYTHON_INSTALL_DIR", Path.Combine(tools, "python"));
                if (Directory.Exists(Path.Combine(tools, "uv-cache")))
                    Environment.SetEnvironmentVariable("UV_CACHE_DIR", Path.Combine(tools, "uv-cache"));
            }
            else
            {
                // No Linux/macOS: limpar qualquer chave UvxPath gravada com .exe
                string currentOverride = EditorPrefs.GetString("MCPForUnity.UvxPath", "");
                if (currentOverride.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    EditorPrefs.DeleteKey("MCPForUnity.UvxPath");
                }

                // Garantir ~/.local/bin no PATH caso o usuário utilize uv/uvx local
                string localBin = Path.Combine(home, ".local", "bin");
                string path = Environment.GetEnvironmentVariable("PATH") ?? "";
                if (Directory.Exists(localBin) && !path.Contains(localBin))
                {
                    Environment.SetEnvironmentVariable("PATH", localBin + ":" + path);
                }

                string uvxLocal = Path.Combine(localBin, "uvx");
                if (File.Exists(uvxLocal) && !EditorPrefs.HasKey("MCPForUnity.UvxPath"))
                {
                    EditorPrefs.SetString("MCPForUnity.UvxPath", uvxLocal);
                }
            }

#pragma warning disable UAC0005 // Bootstrap de inicialização MCP necessita inspecionar assemblies de pacotes
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
#pragma warning restore UAC0005
            var cache = assemblies.Select(a => a.GetType("MCPForUnity.Editor.Services.EditorConfigurationCache")).FirstOrDefault(t => t != null);
            if (cache != null)
            {
                var config = cache.GetProperty("Instance")?.GetValue(null);
                if (config != null)
                {
                    cache.GetMethod("Refresh")?.Invoke(config, null);
                }
            }
        }
        catch (Exception error)
        {
            Debug.LogWarning("Santa Rita MCP (config): " + error.Message);
        }
    }
}
