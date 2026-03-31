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

        // Phase 1: Load assemblies and instantiate IEditorAddin implementations
        foreach (string dllPath in Directory.GetFiles(addinsDir, "*.dll"))
        {
            try
            {
                Assembly assembly = AssemblyLoadContext.Default
                    .LoadFromAssemblyPath(Path.GetFullPath(dllPath));

                var attr = assembly.GetCustomAttribute<EditorAddinAttribute>();
                if (attr is null) continue;

                var addin = (IEditorAddin)Activator.CreateInstance(attr.AddinType)!;
                addin.Initialize();
                _loadedAddins.Add(addin);

                System.Diagnostics.Debug.WriteLine(
                    $"[Addin] Loaded: {addin.Name} v{addin.Version} by {addin.Author}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[Addin] Failed to load {Path.GetFileName(dllPath)}: {ex.Message}");
            }
        }

        // Phase 2: Register contributions into editor state
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
