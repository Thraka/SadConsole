using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace SadConsole.Analyzers.Tests;

public class ConfigureWindowConflictTests
{
    [Fact]
    public async Task SetWindowSizeInCells_WithConfigureWindow_ReportsDiagnostic()
    {
        const string text = @"
using SadConsole.Configuration;
using SadConsole;
using Builder = SadConsole.Configuration.Builder;

public class Test
{
    public void Run()
    {
        Builder
            .GetBuilder()
            .SetWindowSizeInCells(80, 25)
            .ConfigureWindow((config, builder, host) =>
            {
                config.Fullscreen = true;
            })
            .Run();
    }
}
";

        var test =
            new Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<ConfigureWindowConflictAnalyzer,
                DefaultVerifier>();

        test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80.AddPackages([new PackageIdentity("SadConsole", "10.9.0"), new PackageIdentity("SadConsole.Host.MonoGame", "10.9.0")]);
        test.TestCode = text;
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(ConfigureWindowConflictAnalyzer.Rule)
                .WithSpan(12, 14, 12, 34)
                .WithArguments("SetWindowSizeInCells", "SetWindowSizeInCells")
        );

        await test.RunAsync();
    }

    [Fact]
    public async Task SetWindowSizeInPixels_WithConfigureWindow_ReportsDiagnostic()
    {
        const string text = @"
using SadConsole.Configuration;
using SadConsole;
using Builder = SadConsole.Configuration.Builder;

public class Test
{
    public void Run()
    {
        Builder
            .GetBuilder()
            .SetWindowSizeInPixels(800, 600)
            .ConfigureWindow((config, builder, host) =>
            {
                config.Fullscreen = true;
            })
            .Run();
    }
}
";

        var test =
            new Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<ConfigureWindowConflictAnalyzer,
                DefaultVerifier>();

        test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80.AddPackages([new PackageIdentity("SadConsole", "10.9.0"), new PackageIdentity("SadConsole.Host.MonoGame", "10.9.0")]);
        test.TestCode = text;
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(ConfigureWindowConflictAnalyzer.Rule)
                .WithSpan(12, 14, 12, 35)
                .WithArguments("SetWindowSizeInPixels", "SetWindowSizeInPixels")
        );

        await test.RunAsync();
    }

    [Fact]
    public async Task SetWindowSizeInCells_WithoutConfigureWindow_NoDiagnostic()
    {
        const string text = @"
using SadConsole.Configuration;
using SadConsole;
using Builder = SadConsole.Configuration.Builder;

public class Test
{
    public void Run()
    {
        Builder
            .GetBuilder()
            .SetWindowSizeInCells(80, 25)
            .Run();
    }
}
";

        var test =
            new Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<ConfigureWindowConflictAnalyzer,
                DefaultVerifier>();

        test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80.AddPackages([new PackageIdentity("SadConsole", "10.9.0"), new PackageIdentity("SadConsole.Host.MonoGame", "10.9.0")]);
        test.TestCode = text;

        await test.RunAsync();
    }

    [Fact]
    public async Task ConfigureWindow_BeforeSetWindowSizeInCells_ReportsDiagnostic()
    {
        const string text = @"
using SadConsole.Configuration;
using SadConsole;
using Builder = SadConsole.Configuration.Builder;

public class Test
{
    public void Run()
    {
        Builder
            .GetBuilder()
            .ConfigureWindow((config, builder, host) =>
            {
                config.Fullscreen = true;
            })
            .SetWindowSizeInCells(80, 25)
            .Run();
    }
}
";

        var test =
            new Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<ConfigureWindowConflictAnalyzer,
                DefaultVerifier>();

        test.ReferenceAssemblies = ReferenceAssemblies.Net.Net80.AddPackages([new PackageIdentity("SadConsole", "10.9.0"), new PackageIdentity("SadConsole.Host.MonoGame", "10.9.0")]);
        test.TestCode = text;
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult(ConfigureWindowConflictAnalyzer.Rule)
                .WithSpan(16, 14, 16, 34)
                .WithArguments("SetWindowSizeInCells", "SetWindowSizeInCells")
        );

        await test.RunAsync();
    }
}
