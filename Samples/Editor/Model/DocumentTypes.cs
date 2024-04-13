using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Editor.Model;

public enum DocumentTypes
{
    Surface,
    Scene,
    Animation
}

public static class DocumentTypeNames
{
    public const string Surface = "Surface";
    public const string Scene = "Scene";
    public const string Animation = "Animation";

    private static string[] _names = { Surface, Scene, Animation };

    public static string[] Names => _names;
}
