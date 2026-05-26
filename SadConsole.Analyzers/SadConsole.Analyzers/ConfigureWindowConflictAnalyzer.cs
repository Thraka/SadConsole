using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static SadConsole.Analyzers.DiagnosticHelpers;

namespace SadConsole.Analyzers;

/// <summary>
/// Reports an error when SetWindowSizeInCells or SetWindowSizeInPixels is used alongside ConfigureWindow
/// on the same builder chain. The former methods internally call ConfigureWindow, so they will be overwritten.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ConfigureWindowConflictAnalyzer : DiagnosticAnalyzer
{
    private const string ExtensionsClassName = "SadConsole.Configuration.Extensions";
    private const string ConfigureWindowMethodName = "ConfigureWindow";
    private const string SetWindowSizeInCellsMethodName = "SetWindowSizeInCells";
    private const string SetWindowSizeInPixelsMethodName = "SetWindowSizeInPixels";

    internal static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIDs.ConfigureWindowConflict,
        GenerateString(nameof(Resources.ConfigureWindowConflictTitle)),
        GenerateString(nameof(Resources.ConfigureWindowConflictMessageFormat)),
        description: GenerateString(nameof(Resources.ConfigureWindowConflictDescription)),
        category: DiagnosticCategories.Usage,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        // We only care about SetWindowSizeInCells or SetWindowSizeInPixels calls
        string methodName = GetMethodName(invocation);
        if (methodName != SetWindowSizeInCellsMethodName && methodName != SetWindowSizeInPixelsMethodName)
            return;

        // Verify it's the right method via semantic model
        var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken);
        if (symbolInfo.Symbol is not IMethodSymbol methodSymbol)
            return;

        if (methodSymbol.ContainingType?.ToDisplayString() != ExtensionsClassName)
            return;

        // Walk the fluent chain to see if ConfigureWindow is also called
        var chain = GetFluentChainRoot(invocation);
        if (chain == null)
            return;

        bool hasConflict = ChainContainsMethod(chain, ConfigureWindowMethodName, context);

        // If not found in fluent chain, check sibling statements on the same variable
        if (!hasConflict)
        {
            hasConflict = SiblingStatementsContainMethod(invocation, ConfigureWindowMethodName, context);
        }

        if (hasConflict)
        {
            string configMethodName = methodName == SetWindowSizeInCellsMethodName
                ? "SetWindowSizeInCells"
                : "SetWindowSizeInPixels";

            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;
            var diagnostic = Diagnostic.Create(Rule, memberAccess.Name.GetLocation(), methodName, configMethodName);
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static string GetMethodName(InvocationExpressionSyntax invocation)
    {
        return invocation.Expression switch
        {
            MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.Text,
            _ => string.Empty
        };
    }

    /// <summary>
    /// Gets the root expression of a fluent method chain.
    /// </summary>
    private static ExpressionSyntax GetFluentChainRoot(InvocationExpressionSyntax invocation)
    {
        SyntaxNode current = invocation;

        // Walk up: InvocationExpression -> MemberAccessExpression -> InvocationExpression ...
        while (current.Parent is MemberAccessExpressionSyntax memberAccess &&
               memberAccess.Parent is InvocationExpressionSyntax parentInvocation)
        {
            current = parentInvocation;
        }

        // Also walk down from the original invocation to find the full chain
        return current as ExpressionSyntax ?? invocation;
    }

    /// <summary>
    /// Checks if the fluent chain (rooted at the top-level invocation) contains a call to the given method.
    /// </summary>
    private static bool ChainContainsMethod(ExpressionSyntax chainRoot, string targetMethodName, SyntaxNodeAnalysisContext context)
    {
        // Collect all invocations in the chain
        foreach (var node in chainRoot.DescendantNodesAndSelf())
        {
            if (node is InvocationExpressionSyntax inv)
            {
                string name = GetMethodName(inv);
                if (name == targetMethodName)
                {
                    // Verify it's the right class
                    var symbol = context.SemanticModel.GetSymbolInfo(inv, context.CancellationToken).Symbol as IMethodSymbol;
                    if (symbol?.ContainingType?.ToDisplayString() == ExtensionsClassName)
                        return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// For non-fluent usage (separate statements on the same variable), checks if any sibling
    /// statement in the same block calls the target method on the same receiver variable.
    /// </summary>
    private static bool SiblingStatementsContainMethod(InvocationExpressionSyntax invocation, string targetMethodName, SyntaxNodeAnalysisContext context)
    {
        // Get the receiver identifier (e.g., "build" in "build.SetWindowSizeInPixels(...)")
        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
            return false;

        string? receiverName = GetIdentifierName(memberAccess.Expression);
        if (receiverName == null)
            return false;

        // Find the containing block (method body, etc.)
        var containingBlock = invocation.FirstAncestorOrSelf<BlockSyntax>();
        if (containingBlock == null)
            return false;

        // Search all invocations in the block for the target method on the same receiver
        foreach (var statement in containingBlock.Statements)
        {
            foreach (var inv in statement.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>())
            {
                if (inv == invocation)
                    continue;

                if (inv.Expression is MemberAccessExpressionSyntax ma &&
                    ma.Name.Identifier.Text == targetMethodName)
                {
                    string? otherReceiver = GetIdentifierName(ma.Expression);
                    if (otherReceiver == receiverName)
                    {
                        // Verify it's the right class
                        var symbol = context.SemanticModel.GetSymbolInfo(inv, context.CancellationToken).Symbol as IMethodSymbol;
                        if (symbol?.ContainingType?.ToDisplayString() == ExtensionsClassName)
                            return true;
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Gets the identifier name from an expression, walking through fluent chains to find the root identifier.
    /// </summary>
    private static string? GetIdentifierName(ExpressionSyntax expression)
    {
        switch (expression)
        {
            case IdentifierNameSyntax identifier:
                return identifier.Identifier.Text;
            case InvocationExpressionSyntax inv:
                // e.g., build.Something().SetWindowSize(...) - walk down
                if (inv.Expression is MemberAccessExpressionSyntax ma)
                    return GetIdentifierName(ma.Expression);
                return null;
            case MemberAccessExpressionSyntax memberAccess:
                return GetIdentifierName(memberAccess.Expression);
            default:
                return null;
        }
    }
}
