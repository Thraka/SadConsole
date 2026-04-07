namespace SadConsole.Editor.Addins;

/// <summary>
/// Marks an assembly as containing an editor addin. Place this on the assembly
/// to enable fast discovery without scanning all types via reflection.
/// </summary>
/// <example>
/// <code>
/// [assembly: EditorAddin(typeof(MyAddin), Order = 0)]
/// [assembly: EditorAddin(typeof(MyOtherAddin), Order = 10)]
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class EditorAddinAttribute : Attribute
{
    /// <summary>The concrete type that implements <see cref="IEditorAddin"/>.</summary>
    public Type AddinType { get; }

    /// <summary>
    /// Controls the initialization order across all discovered addins.
    /// Lower values are loaded first. Defaults to <c>0</c>.
    /// </summary>
    public int Order { get; set; }

    public EditorAddinAttribute(Type addinType)
    {
        ArgumentNullException.ThrowIfNull(addinType);
        if (!typeof(IEditorAddin).IsAssignableFrom(addinType))
            throw new ArgumentException(
                $"Type {addinType.FullName} must implement {nameof(IEditorAddin)}.",
                nameof(addinType));
        AddinType = addinType;
    }
}
