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
/// Code fix that removes the IsDirty = false assignment from UpdateAndRedraw overrides.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ControlBaseIsDirtyCodeFix)), Shared]
public class ControlBaseIsDirtyCodeFix : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(DiagnosticIDs.ControlBaseIsDirtyFalse);

    public override FixAllProvider? GetFixAllProvider() => null;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.Single();
        var diagnosticSpan = diagnostic.Location.SourceSpan;
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var diagnosticNode = root?.FindNode(diagnosticSpan);

        if (diagnosticNode is not AssignmentExpressionSyntax)
            return;

        context.RegisterCodeFix(
            CodeAction.Create(
                title: Resources.SADCON0003_RemoveIsDirtyFalseTitle,
                createChangedDocument: c => RemoveStatementAsync(context.Document, diagnosticNode, c),
                equivalenceKey: nameof(Resources.SADCON0003_RemoveIsDirtyFalseTitle)),
            diagnostic);
    }

    private async Task<Document> RemoveStatementAsync(Document document, SyntaxNode node, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root == null)
            return document;

        // The assignment expression is inside an ExpressionStatementSyntax
        var statement = node.FirstAncestorOrSelf<ExpressionStatementSyntax>();
        if (statement == null)
            return document;

        var newRoot = root.RemoveNode(statement, SyntaxRemoveOptions.KeepNoTrivia);
        if (newRoot == null)
            return document;

        return document.WithSyntaxRoot(newRoot);
    }
}
