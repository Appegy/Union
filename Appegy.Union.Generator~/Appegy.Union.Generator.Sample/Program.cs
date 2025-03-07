using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Appegy.Union.Generator.Sample;

public static class Program
{
    public static async Task Main()
    {
        var assembly = typeof(Program).Assembly;

        var resourceNames = assembly.GetManifestResourceNames();

        var sourceCodes = resourceNames.Select(resourceName =>
        {
            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream!);
            return reader.ReadToEnd();
        });

        var syntaxTrees = sourceCodes.Select(code => CSharpSyntaxTree.ParseText(code)).ToArray();

        var compilation = CSharpCompilation.Create("UnionDebuggingApp",
            syntaxTrees,
            references:
            [
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Attribute).Assembly.Location),
            ],
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        ImmutableArray<DiagnosticAnalyzer> analyzers =
        [
            new UnionAttributeAnalyzer(),
            new ExposeAttributeAnalyzer()
        ];

        var compilationWithAnalyzers = compilation.WithAnalyzers(analyzers);
        var diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();

        foreach (var diagnostic in diagnostics)
        {
            Console.WriteLine(diagnostic);
        }

        IIncrementalGenerator[] generators =
        [
            new UnionAttributeGenerator(),
            new ExposeAttributeGenerator()
        ];

        var driver = CSharpGeneratorDriver.Create(generators);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out _);

        foreach (var tree in updatedCompilation.SyntaxTrees.Except(compilation.SyntaxTrees))
        {
            Console.WriteLine($"Generated: {tree.FilePath}");
            Console.WriteLine(await tree.GetTextAsync());
        }
    }
}