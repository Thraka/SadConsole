using System.Collections.Generic;
using SadConsole.Entities;
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
    public DocumentOptions Options;
    public Dictionary<string, string> Metadata = new();

    public AnimatedScreenObject GetAnimatedScreenObject()
    {
        Animation.Font = GameHost.Instance.Fonts[SurfaceFont.Name];
        Animation.FontSize = SurfaceFontSize;
        return Animation;
    }

    public AnimatedScreenObject GetAnimatedScreenObjectAndFont()
    {
        Animation.Font = FontSerialized.ToFont(SurfaceFont);
        Animation.FontSize = SurfaceFontSize;
        return Animation;
    }
}
