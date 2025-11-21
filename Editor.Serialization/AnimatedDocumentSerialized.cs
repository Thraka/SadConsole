using SadConsole.SerializedTypes;
using SadRogue.Primitives;

namespace SadConsole.Editor.Serialization;

public class AnimatedDocumentSerialized
{
    public string Title;
    public AnimatedScreenObject Animation;
    public FontSerialized SurfaceFont;
    public Point SurfaceFontSize;
    public Point EditorFontSize;
}
