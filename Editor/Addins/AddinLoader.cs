using System.Reflection;
using System.Runtime.Loader;

namespace SadConsole.Editor.Addins;

public static class AddinLoader
{
    private static readonly List<IEditorAddin> _loadedAddins = new();

    /// <summary>All successfully loaded addins.</summary>
    public static IReadOnlyList<IEditorAddin> LoadedAddins => _loadedAddins;

    /// <summary>
    /// Discovers, loads, and registers all addins from the addins/ directory.
    /// Call once during startup after Core.Start() completes.
    /// </summary>
    public static void LoadAndRegisterAddins()
    {
        string addinsDir = Path.Combine(AppContext.BaseDirectory, "addins");

        if (!Directory.Exists(addinsDir))
        {
            Directory.CreateDirectory(addinsDir);
            return;
        }

        // Phase 1: Discover all addin attributes across every assembly
        List<(int Order, Type AddinType, string DllPath)> discovered = [];

        foreach (string dllPath in Directory.GetFiles(addinsDir, "*.dll"))
        {
            try
            {
                Assembly assembly = AssemblyLoadContext.Default
                    .LoadFromAssemblyPath(Path.GetFullPath(dllPath));

                foreach (var attr in assembly.GetCustomAttributes<EditorAddinAttribute>())
                    discovered.Add((attr.Order, attr.AddinType, dllPath));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[Addin] Failed to load {Path.GetFileName(dllPath)}: {ex.Message}");
            }
        }

        // Phase 2: Instantiate and initialize in Order (lower first)
        discovered.Sort((a, b) => a.Order.CompareTo(b.Order));

        foreach (var (order, addinType, dllPath) in discovered)
        {
            try
            {
                var addin = (IEditorAddin)Activator.CreateInstance(addinType)!;
                addin.Initialize();
                _loadedAddins.Add(addin);

                System.Diagnostics.Debug.WriteLine(
                    $"[Addin] Loaded (order {order}): {addin.Name} v{addin.Version} by {addin.Author}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[Addin] Failed to initialize {addinType.FullName} from {Path.GetFileName(dllPath)}: {ex.Message}");
            }
        }

        // Phase 3: Register contributions into editor state
        foreach (var addin in _loadedAddins)
        {
            try
            {
                foreach (var builder in addin.GetDocumentBuilders())
                    Core.State.DocumentBuilders.Objects.Add(builder);

                foreach (var panel in addin.GetGuiPanels())
                    Core.ImGuiComponent.ImGuiRenderer.UIObjects.Add(panel);

                foreach (var menuItem in addin.GetMenuItems())
                    Core.State.AddinMenuItems.Add(menuItem);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[Addin] Failed to register {addin.Name}: {ex.Message}");
            }
        }

        if (_loadedAddins.Count > 0)
            System.Diagnostics.Debug.WriteLine(
                $"[Addin] {_loadedAddins.Count} addin(s) loaded successfully.");
    }
}
