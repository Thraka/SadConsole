using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace SadConsole.Analyzers;

/// <summary>
/// A sample analyzer that reports invalid values being used for the 'speed' parameter of the 'SetSpeed' function.
/// To make sure that we analyze the method of the specific class, we use semantic analysis instead of the syntax tree, so this analyzer will not work if the project is not compilable.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ColoredGlyphDecoratorSetAnalyzer : DiagnosticAnalyzer
{
    private static LocalizableResourceString GenerateString(string name) =>
        new LocalizableResourceString(name, Resources.ResourceManager, typeof(Resources));

    private const string CommonApiClassName = "SadConsole.ColoredGlyphBase";
    private const string CommonApiPropertyName = "Decorators";

    internal static readonly DiagnosticDescriptor RuleNew = new(
        DiagnosticIDs.CellDecoratorNew,
        GenerateString(nameof(Resources.ColoredGlyphDecoratorNewAccessTitle)),
        GenerateString(nameof(Resources.ColoredGlyphDecoratorNewAccessMessageFormat)),
        description: GenerateString(nameof(Resources.ColoredGlyphDecoratorNewAccessDescription)),
        category: DiagnosticCategories.Usage,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor RuleNull = new(
        DiagnosticIDs.CellDecoratorNull,
        GenerateString(nameof(Resources.ColoredGlyphDecoratorNewAccessTitle)),
        GenerateString(nameof(Resources.ColoredGlyphDecoratorNewAccessMessageFormat)),
        description: GenerateString(nameof(Resources.ColoredGlyphDecoratorNewAccessDescription)),
        category: DiagnosticCategories.Usage,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    // Keep in mind: you have to list your rules here.
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(RuleNew, RuleNull);

    public override void Initialize(AnalysisContext context)
    {
        // You must call this method to avoid analyzing generated code.
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        // You must call this method to enable the Concurrent Execution.
        context.EnableConcurrentExecution();

        // Subscribe to semantic (compile time) action invocation, e.g. method invocation.
        context.RegisterOperationAction(AnalyzeOperation, OperationKind.SimpleAssignment);

        // Check other 'context.Register...' methods that might be helpful for your purposes.
    }

    /// <summary>
    /// Executed on the completion of the semantic analysis associated with the Invocation operation.
    /// </summary>
    /// <param name="context">Operation context.</param>
    private void AnalyzeOperation(OperationAnalysisContext context)
    {
        if (context.Operation is not IAssignmentOperation assignmentOperation ||
            context.Operation.Syntax is not AssignmentExpressionSyntax expressionSyntax)
            return;

        var baseType = context.Compilation.GetTypeByMetadataName(CommonApiClassName);

        // Check if we're talking to a property and that property belongs to ColoredGlyph
        if (assignmentOperation.Target is not IPropertyReferenceOperation propertyRef ||
            propertyRef.Property.Name != CommonApiPropertyName ||
            !SymbolEqualityComparer.Default.Equals(propertyRef.Property.ContainingType, baseType))
            return;

        // Check if decorators is being assigned a new list, or an array expression
        if (expressionSyntax.Right.IsKind(SyntaxKind.NullLiteralExpression))
        {
            var diagnostic = Diagnostic.Create(RuleNull, expressionSyntax.GetLocation(), "SadConsole.CellDecoratorHelpers");

            // Reporting a diagnostic is the primary outcome of analyzers.
            context.ReportDiagnostic(diagnostic);
        }

        else if (expressionSyntax.Right.IsKind(SyntaxKind.ObjectCreationExpression) ||
            expressionSyntax.Right.IsKind(SyntaxKind.ImplicitObjectCreationExpression) ||
            expressionSyntax.Right.IsKind(SyntaxKind.CollectionExpression) ||
            expressionSyntax.Right.IsKind(SyntaxKind.NullLiteralExpression))
        {
            var diagnostic = Diagnostic.Create(RuleNew, expressionSyntax.GetLocation(), "SadConsole.CellDecoratorHelpers");

            // Reporting a diagnostic is the primary outcome of analyzers.
            context.ReportDiagnostic(diagnostic);
        }
    }
}
