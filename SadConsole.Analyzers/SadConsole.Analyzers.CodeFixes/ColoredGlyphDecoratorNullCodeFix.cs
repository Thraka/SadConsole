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
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Rename;

namespace SadConsole.Analyzers;

/// <summary>
/// A sample code fix provider that renames classes with the company name in their definition.
/// All code fixes must  be linked to specific analyzers.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ColoredGlyphDecoratorNullCodeFix)), Shared]
public class ColoredGlyphDecoratorNullCodeFix : CodeFixProvider
{
    private const string CommonName = "Common";

    // Specify the diagnostic IDs of analyzers that are expected to be linked.
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(DiagnosticIDs.CellDecoratorNull);

    // If you don't need the 'fix all' behaviour, return null.
    public override FixAllProvider? GetFixAllProvider() => null;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        // We link only one diagnostic and assume there is only one diagnostic in the context.
        var diagnostic = context.Diagnostics.Single();

        // 'SourceSpan' of 'Location' is the highlighted area. We're going to use this area to find the 'SyntaxNode' to rename.
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        // Get the root of Syntax Tree that contains the highlighted diagnostic.
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        // Find SyntaxNode corresponding to the diagnostic.
        var diagnosticNode = root?.FindNode(diagnosticSpan);

        // To get the required metadata, we should match the Node to the specific type: 'ClassDeclarationSyntax'.
        if (diagnosticNode is not AssignmentExpressionSyntax declaration)
            return;

        // Register a code action that will invoke the fix.
        context.RegisterCodeFix(
            CodeAction.Create(
                title: Resources.SADCON0002_RemoveCellDecoratorsTitle,
                createChangedDocument: c => ConvertToHelpersAsync(context, declaration, c),
                equivalenceKey: nameof(Resources.SADCON0002_RemoveCellDecoratorsTitle)),
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

        //((MemberAccessExpressionSyntax)assignmentExpressionSyntax.Left).Expression.TryGetInferredMemberName()

        if (memberSyntax.Expression.TryGetInferredMemberName() is not string memberName)
            return document;

        var compilation = await context.Document.Project.GetCompilationAsync(cancellationToken);

        if (compilation.GetTypeByMetadataName("SadConsole.CellDecoratorHelpers") is not ITypeSymbol helpersSymbol)
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

        // Create the new invocation expression.
        var helperCall = isNamespaceImported
            ? $"CellDecoratorHelpers.RemoveAllDecorators({memberName})"
            : $"SadConsole.CellDecoratorHelpers.RemoveAllDecorators({memberName})";
        ExpressionSyntax methodCall2 = SyntaxFactory.ParseExpression(helperCall)
                                       .WithLeadingTrivia(assignmentExpressionSyntax.GetLeadingTrivia());

        // Replace the old assignment with the new invocation.
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var newRoot = root.ReplaceNode(assignmentExpressionSyntax, methodCall2);

        return document.WithSyntaxRoot(newRoot);

        //editor.ReplaceNode(assignmentExpressionSyntax, methodCall2);

        // var formattedRoot = Formatter.Format(editor.GetChangedRoot(), editor.OriginalDocument.Project.Solution.Workspace);
        // return document.WithSyntaxRoot(formattedRoot);

        //return editor.GetChangedDocument();
    }
}
