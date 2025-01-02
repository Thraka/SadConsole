using Newtonsoft.Json.Linq;
using SadConsole.Editor.GuiObjects;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor;

public static partial class Core
{
    public static void Start()
    {
        Settings.DoFinalDraw = false;

        ImGuiCore.ImGuiComponent = new ImGuiMonoGameComponent(Host.Global.GraphicsDeviceManager, Game.Instance.MonoGameInstance, true);

        var value = ImGuiCore.ImGuiComponent.ImGuiRenderer.AddFontTTF("JetBrains Mono SemiBold Nerd Font Complete.ttf", 18f);
        ImGuiCore.ImGuiComponent.ImGuiRenderer.SetDefaultFont(value);
        Themes.SetModernColors();

        //ImGuiNET.ImGui.GetStyle().ScaleAllSizes(2f);

        ResetUIList();

        Game.Instance.MonoGameInstance.Components.Add(ImGuiCore.ImGuiComponent);

        // Test code
        // ===============
        Documents.DocumentSurface.Builder builder = new();
        builder.ResetBuilder();
        State.Documents.Objects.Add(builder.CreateDocument());
        State.Documents.SelectedItemIndex = 0;
        State.Documents.SelectedItem!.OnSelected();
        State.Tools.SelectedItemIndex = 0;
    }

    public static void ResetUIList()
    {
        ImGuiCore.ImGuiComponent.UIComponents.Clear();
        //ImGuiCore.ImGuiComponent.UIComponents.Add(GuiStartup);
        ImGuiCore.ImGuiComponent.UIComponents.Add(State.GuiTopBar);
        ImGuiCore.ImGuiComponent.UIComponents.Add(State.GuiDockSpace);
        ImGuiCore.ImGuiComponent.UIComponents.Add(new GuiDocumentsList());
        ImGuiCore.ImGuiComponent.UIComponents.Add(new GuiToolsList());
        ImGuiCore.ImGuiComponent.UIComponents.Add(new GuiDocumentsHost());
        ImGuiCore.ImGuiComponent.UIComponents.Add(new GuiFinalDrawDocument());
        //ImGuiCore.ImGuiComponent.UIComponents.Add(GuiToolsWindow);
        //ImGuiCore.ImGuiComponent.UIComponents.Add(GuiDocumentsHost);
        //ImGuiCore.ImGuiComponent.UIComponents.Add(_debuggingTools);
    }

    public static void ShowCreateDocument()
    {
        //NewFile window = new();
        //window.Closed += (s, e) =>
        //{
        //    if (window.DialogResult)
        //        State.OpenDocuments = [.. State.OpenDocuments, window.Document];
        //};
        //window.Show();
    }

    public static void Alert(string message)
    {
        //MessageWindow window = new(message);
        //window.Show();
    }

    public static void Stop()
    {
        Host.Global.SadConsoleComponent.Enabled = true;
        Settings.DoFinalDraw = true;

        //SadConsole.Game.Instance.MonoGameInstance.ClearScreenComponent.Visible = true;
        //SadConsole.Game.Instance.MonoGameInstance.ClearScreenComponent.Enabled = true;

        ImGuiCore.ImGuiComponent.Visible = false;
        ImGuiCore.ImGuiComponent.Enabled = false;

        Settings.Input.DoKeyboard = true;
        Settings.Input.DoMouse = true;
    }
}
