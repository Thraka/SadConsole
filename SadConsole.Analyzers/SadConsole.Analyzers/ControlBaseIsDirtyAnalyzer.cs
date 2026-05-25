using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static SadConsole.Analyzers.DiagnosticHelpers;

namespace SadConsole.Analyzers;

/// <summary>
/// Reports when IsDirty is set to false inside an override of ControlBase.UpdateAndRedraw.
/// The renderer is responsible for resetting the dirty flag.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ControlBaseIsDirtyAnalyzer : DiagnosticAnalyzer
{
    private const string ControlBaseClassName = "SadConsole.UI.Controls.ControlBase";
    private const string MethodName = "UpdateAndRedraw";
    private const string PropertyName = "IsDirty";

    internal static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIDs.ControlBaseIsDirtyFalse,
        GenerateString(nameof(Resources.ControlBaseIsDirtyFalseTitle)),
        GenerateString(nameof(Resources.ControlBaseIsDirtyFalseMessageFormat)),
        description: GenerateString(nameof(Resources.ControlBaseIsDirtyFalseDescription)),
        category: DiagnosticCategories.Usage,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterOperationAction(AnalyzeOperation, OperationKind.SimpleAssignment);
    }

    private void AnalyzeOperation(OperationAnalysisContext context)
    {
        if (context.Operation is not IAssignmentOperation assignmentOperation ||
            context.Operation.Syntax is not AssignmentExpressionSyntax expressionSyntax)
            return;

        // Check if assigning to IsDirty
        if (assignmentOperation.Target is not IPropertyReferenceOperation propertyRef ||
            propertyRef.Property.Name != PropertyName)
            return;

        // Check if the value being assigned is false
        if (assignmentOperation.Value.ConstantValue.HasValue &&
            assignmentOperation.Value.ConstantValue.Value is bool boolValue &&
            boolValue == false)
        {
            // Check if we're inside an override of UpdateAndRedraw on a type deriving from ControlBase
            var containingMethod = GetContainingMethod(expressionSyntax);
            if (containingMethod == null)
                return;

            var semanticModel = context.Operation.SemanticModel;
            if (semanticModel == null)
                return;

            var methodSymbol = semanticModel.GetDeclaredSymbol(containingMethod) as IMethodSymbol;
            if (methodSymbol == null ||
                methodSymbol.Name != MethodName ||
                !methodSymbol.IsOverride)
                return;

            // Check if the containing type derives from ControlBase
            var controlBaseType = context.Compilation.GetTypeByMetadataName(ControlBaseClassName);
            if (controlBaseType == null)
                return;

            var containingType = methodSymbol.ContainingType;
            if (!DerivesFrom(containingType, controlBaseType))
                return;

            // Check that the property belongs to the type hierarchy (not some unrelated IsDirty)
            var propertyContainingType = propertyRef.Property.ContainingType;
            if (!DerivesFrom(containingType, propertyContainingType) &&
                !SymbolEqualityComparer.Default.Equals(containingType, propertyContainingType))
                return;

            var diagnostic = Diagnostic.Create(Rule, expressionSyntax.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static MethodDeclarationSyntax? GetContainingMethod(SyntaxNode node)
    {
        var current = node.Parent;
        while (current != null)
        {
            if (current is MethodDeclarationSyntax method)
                return method;
            current = current.Parent;
        }
        return null;
    }

    private static bool DerivesFrom(INamedTypeSymbol? type, INamedTypeSymbol baseType)
    {
        var current = type?.BaseType;
        while (current != null)
        {
            if (SymbolEqualityComparer.Default.Equals(current, baseType))
                return true;
            current = current.BaseType;
        }
        return false;
    }
}
