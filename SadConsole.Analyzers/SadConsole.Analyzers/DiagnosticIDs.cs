using Microsoft.CodeAnalysis;

namespace SadConsole.Analyzers;

public static class DiagnosticIDs
{
    public const string CellDecoratorNew = "SADCON0001"; //
    public const string CellDecoratorNull = "SADCON0002"; //
}

public static class DiagnosticCategories
{
    public const string Usage = "Usage";
    public const string Design = "Design";
    public const string Naming = "Naming";

}

public static class DiagnosticHelpers
{
    public static LocalizableResourceString GenerateString(string name) =>
        new(name, Resources.ResourceManager, typeof(Resources));
}
