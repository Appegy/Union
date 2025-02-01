using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Appegy.Union.Generator.AttributesNames;

namespace Appegy.Union.Generator;

[Generator]
public class ExposeAttributeGenerator : IIncrementalGenerator
{
    private static IReadOnlyList<GeneratorPart<ExposeAttributePartInput>> Parts { get; } =
    [
        new ExposeUsingsPart(),
        new ExposeParentScopedPart([
            new ExposeDeclarationPart(),
            new ScopedPart<ExposeAttributePartInput>([
                new ExposeInterfacePart([
                    new ExposePropertiesPart()
                ]),
            ])
        ])
    ];

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var exposeSources = context
            .SyntaxProvider
            .ForAttributeWithMetadataName(
                ExposeAttributeName,
                predicate: static (syntax, _) => syntax is StructDeclarationSyntax,
                transform: static (ctx, _) =>
                {
                    var syntax = (StructDeclarationSyntax)ctx.TargetNode;
                    var attribute = ctx.Attributes.First();
                    var interfaces = attribute.GetTypesFromConstructor(TypeKind.Interface);
                    return (syntax, interfaces);
                });

        var unionSources = context
            .SyntaxProvider
            .ForAttributeWithMetadataName(
                UnionAttributeName,
                predicate: static (syntax, _) => syntax is StructDeclarationSyntax,
                transform: static (ctx, _) =>
                {
                    var syntax = (StructDeclarationSyntax)ctx.TargetNode;
                    var attribute = ctx.Attributes.First();
                    var types = attribute.GetTypesFromConstructor(TypeKind.Struct);
                    return (syntax, types);
                });

        var sources = exposeSources.Combine(unionSources.Collect())
            .Select(static (input, _) =>
            {
                var ((syntax, interfaces), unionArray) = input;
                var unionTypesForThis = unionArray
                    .Where(x => x.syntax.Span.Equals(syntax.Span) && x.syntax.SyntaxTree.FilePath == syntax.SyntaxTree.FilePath)
                    .SelectMany(x => x.types)
                    .ToImmutableList();
                return (syntax, interfaces, unionTypesForThis);
            });

        context.RegisterSourceOutput(sources, static (ctx, input) =>
        {
            var (syntax, interfaces, types) = input;

            if (!syntax.Modifiers.Any(SyntaxKind.PartialKeyword)) return;
            if (interfaces.Count == 0) return;

            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);
            using var codeWriter = new IndentedTextWriter(streamWriter, "    ");

            codeWriter.AppendParts(Parts, new ExposeAttributePartInput(syntax, types, interfaces));

            streamWriter.Flush();
            ctx.AddSource($"{syntax.Identifier.Text}_Expose.g.cs", SourceText.From(memoryStream, Encoding.UTF8, canBeEmbedded: true));
        });
    }
}