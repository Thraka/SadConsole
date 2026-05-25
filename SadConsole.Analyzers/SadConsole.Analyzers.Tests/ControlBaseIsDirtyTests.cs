using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace SadConsole.Analyzers.Tests;

public class ControlBaseIsDirtyTests
{
    [Fact]
    public async Task IsDirtyFalse_InUpdateAndRedraw_ReportsDiagnostic()
    {
        const string text = @"
using SadConsole.UI.Controls;

public class MyControl : ControlBase
{
    public MyControl(int width, int height) : base(width, height) { }

    public override void UpdateAndRedraw(System.TimeSpan time)
    {
        IsDirty = false;
    }
}
";
        const string fixedText = @"
using SadConsole.UI.Controls;

public class MyControl : ControlBase
{
    public MyControl(int width, int height) : base(width, height) { }

    public override void UpdateAndRedraw(System.TimeSpan time)
    {
    }
}
";
        var test =
            new Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<ControlBaseIsDirtyAnalyzer, ControlBaseIsDirtyCodeFix,
                DefaultVerifier>();

        test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80.AddPackages([new PackageIdentity("SadConsole", "10.5.0")]);
        test.TestCode = text;
        test.FixedCode = fixedText;
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(ControlBaseIsDirtyAnalyzer.Rule)
                .WithLocation(10, 9)
        );

        await test.RunAsync();
    }

    [Fact]
    public async Task IsDirtyFalse_OutsideUpdateAndRedraw_NoDiagnostic()
    {
        const string text = @"
using SadConsole.UI.Controls;

public class MyControl : ControlBase
{
    public MyControl(int width, int height) : base(width, height) { }

    public override void UpdateAndRedraw(System.TimeSpan time) { }

    public void SomeOtherMethod()
    {
        IsDirty = false;
    }
}
";
        var test =
            new Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<ControlBaseIsDirtyAnalyzer,
                DefaultVerifier>();

        test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80.AddPackages([new PackageIdentity("SadConsole", "10.5.0")]);
        test.TestCode = text;

        await test.RunAsync();
    }
}
