using SadConsole.ImGuiSystem;

namespace SadConsole.ImGuiTypes;

public class ConnectedLineStyleType : ITitle
{
    public static ConnectedLineStyleType[] AllConnectedLineStyles { get; } =
    [
        new ConnectedLineStyleType("Empty", ICellSurface.ConnectedLineEmpty),
        new ConnectedLineStyleType("Thin", ICellSurface.ConnectedLineThin),
        new ConnectedLineStyleType("Thin Extended", ICellSurface.ConnectedLineThinExtended),
        new ConnectedLineStyleType("Thick", ICellSurface.ConnectedLineThick),
        new ConnectedLineStyleType("3D", ICellSurface.Connected3dBox),
    ];

    public string Title { get; }

    public int[] ConnectedLineStyle { get; }

    public ConnectedLineStyleType(string title, int[] connectedLineStyle) =>
        (Title, ConnectedLineStyle) = (title, connectedLineStyle);

    public override string ToString() =>
        Title;
}
