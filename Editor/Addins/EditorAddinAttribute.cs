namespace SadConsole.Editor.Addins;

/// <summary>
/// Marks an assembly as containing an editor addin. Place this on the assembly
/// to enable fast discovery without scanning all types via reflection.
/// </summary>
/// <example>
/// <code>[assembly: EditorAddin(typeof(MyAddin))]</code>
/// </example>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public sealed class EditorAddinAttribute : Attribute
{
    /// <summary>The concrete type that implements <see cref="IEditorAddin"/>.</summary>
    public Type AddinType { get; }

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
