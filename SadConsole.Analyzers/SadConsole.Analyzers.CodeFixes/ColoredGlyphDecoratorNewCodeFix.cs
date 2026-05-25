using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SadConsole.Analyzers;

/// <summary>
/// A sample code fix provider that renames classes with the company name in their definition.
/// All code fixes must  be linked to specific analyzers.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ColoredGlyphDecoratorNewCodeFix)), Shared]
public class ColoredGlyphDecoratorNewCodeFix : CodeFixProvider
{
    private const string CommonName = "Common";

    // Specify the diagnostic IDs of analyzers that are expected to be linked.
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(DiagnosticIDs.CellDecoratorNew);

    // If you don't need the 'fix all' behaviour, return null.
    public override FixAllProvider? GetFixAllProvider() => null;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.Single();

        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        var diagnosticNode = root?.FindNode(diagnosticSpan);

        if (diagnosticNode is not AssignmentExpressionSyntax declaration)
            return;

        // Register a code action that will invoke the fix.
        context.RegisterCodeFix(
            CodeAction.Create(
                title: Resources.SADCON0001_NewCellDecoratorsTitle,
                createChangedDocument: c => ConvertToHelpersAsync(context, declaration, c),
                equivalenceKey: nameof(Resources.SADCON0001_NewCellDecoratorsTitle)),
            diagnostic);
    }

    /// <summary>
    /// Executed on the quick fix action raised by the user.
    /// </summary>
    /// <param name="document">Affected source file.</param>
    /// <param name="assignmentExpressionSyntax">Highlighted class declaration Syntax Node.</param>
    /// <param name="cancellationToken">Any fix is cancellable by the user, so we should support the cancellation token.</param>
    /// <returns>Clone of the solution with updates: renamed class.</returns>
    private async Task<Document> ConvertToHelpersAsync(CodeFixContext context,
        AssignmentExpressionSyntax assignmentExpressionSyntax, CancellationToken cancellationToken)
    {
        Document document = context.Document;

        if (assignmentExpressionSyntax.Left is not MemberAccessExpressionSyntax memberSyntax)
            return document;

        string memberName = memberSyntax.Expression.ToString();

        var compilation = await context.Document.Project.GetCompilationAsync(cancellationToken);

        if (compilation?.GetTypeByMetadataName("SadConsole.CellDecoratorHelpers") is not ITypeSymbol helpersSymbol)
            return document;


        // Check if the namespace is already imported
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        var namespaceImported = semanticModel.Compilation.GlobalNamespace
            .GetNamespaceMembers()
            .Any(ns => ns.Name == helpersSymbol.ContainingNamespace.ToString());

        ExpressionSyntax cellDecoratorSyntax = SyntaxFactory.IdentifierName(helpersSymbol.Name);

        // Check if the SadConsole namespace is imported and its types are resolvable without qualification.
        var isNamespaceImported = semanticModel.LookupNamespacesAndTypes(assignmentExpressionSyntax.SpanStart, name: "CellDecoratorHelpers")
            .Any(symbol => symbol.ContainingNamespace?.ToDisplayString() == "SadConsole");

        // Determine if the right-hand side has initializer items (non-empty collection).
        string rightSide = assignmentExpressionSyntax.Right.ToString();
        bool hasItems = assignmentExpressionSyntax.Right switch
        {
            ObjectCreationExpressionSyntax creation => creation.Initializer?.Expressions.Count > 0,
            ImplicitObjectCreationExpressionSyntax implicitCreation => implicitCreation.Initializer?.Expressions.Count > 0,
            CollectionExpressionSyntax collection => collection.Elements.Count > 0,
            _ => false
        };

        // Create the new invocation expression.
        string prefix = isNamespaceImported ? "CellDecoratorHelpers" : "SadConsole.CellDecoratorHelpers";
        var helperCall = hasItems
            ? $"{prefix}.SetDecorators({rightSide}, {memberName})"
            : $"{prefix}.RemoveAllDecorators({memberName})";
        ExpressionSyntax methodCall2 = SyntaxFactory.ParseExpression(helperCall)
                                       .WithLeadingTrivia(assignmentExpressionSyntax.GetLeadingTrivia());

        // Replace the old assignment with the new invocation.
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var newRoot = root.ReplaceNode(assignmentExpressionSyntax, methodCall2);

        return document.WithSyntaxRoot(newRoot);
    }
}
