using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using SadConsole;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<
    SadConsole.Analyzers.ColoredGlyphDecoratorSetAnalyzer,
    SadConsole.Analyzers.ColoredGlyphDecoratorNullCodeFix,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace SadConsole.Analyzers.Tests;

public class ColoredGlyphDecoratorTests
{
    [Fact]
    public async Task ColoredGlyphDecorator_Null()
    {
        const string text = @"
public class Program
{
    public void Main()
    {
        SadConsole.ColoredGlyph cell = new();
        cell.Decorators = null;
    }
}
";
        const string newText = @"
public class Program
{
    public void Main()
    {
        SadConsole.ColoredGlyph cell = new();
        SadConsole.CellDecoratorHelpers.RemoveAllDecorators(cell);
    }
}
";
        var test =
            new Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<ColoredGlyphDecoratorSetAnalyzer, ColoredGlyphDecoratorNullCodeFix,
                DefaultVerifier>();

        test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80.AddPackages([new PackageIdentity("SadConsole", "10.5.0")]);
        test.TestCode = text;
        test.FixedCode = newText;
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(SadConsole.Analyzers.ColoredGlyphDecoratorSetAnalyzer.RuleNull)
                .WithLocation(7, 9)
                .WithArguments("SadConsole.CellDecoratorHelpers")
        );

        //var expected = Verifier.Diagnostic(SadConsole.Analyzers.ColoredGlyphDecoratorSetAnalyzer.RuleNull);
            //.WithLocation(8, 9);
        //await Verifier.VerifyCodeFixAsync(text, expected, newText);

        await test.RunAsync();
    }
}
