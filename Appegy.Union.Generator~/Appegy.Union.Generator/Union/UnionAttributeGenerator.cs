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
using static Appegy.Union.Generator.AttributesSource;
using static Appegy.Union.Generator.AttributesNames;

namespace Appegy.Union.Generator;

[Generator]
public class UnionAttributeGenerator : IIncrementalGenerator
{
    private static IReadOnlyList<GeneratorPart<UnionAttributePartInput>> Parts { get; } =
    [
        new HeaderPart<UnionAttributePartInput>(),
        new UnionParentScopedPart([
            new UnionDeclarationPart(),
            new ScopedPart<UnionAttributePartInput>([
                new UnionTypeEnumPart(),
                new UnionFieldsPart(),
                new UnionPropertiesPart(),
                new UnionConstructorsPart(),
                new UnionToStringPart(),
                new UnionGetHashCodePart(),
                new UnionEqualsPart(),
                new UnionOperatorsPart(),
                new UnionComparisonPart(),
            ])
        ])
    ];

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(c => c.AddSource("UnionAttribute.g.cs", UnionAttribute));

        var sources = context
            .SyntaxProvider
            .ForAttributeWithMetadataName(
                UnionAttributeName,
                predicate: static (syntax, _) => syntax is StructDeclarationSyntax,
                transform: static (ctx, _) =>
                {
                    var syntax = (StructDeclarationSyntax)ctx.TargetNode;
                    var attribute = ctx.Attributes.First();
                    var types = attribute.GetTypesFromConstructor(TypeKind.Struct)
                        .Select(c => new UnionTypeInfo(c))
                        .ToImmutableList();
                    return (syntax, types);
                });

        context.RegisterSourceOutput(sources, static (ctx, input) =>
        {
            var (syntax, types) = input;

            if (!syntax.Modifiers.Any(SyntaxKind.PartialKeyword)) return;
            if (types.Count == 0) return;

            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);
            using var codeWriter = new IndentedTextWriter(streamWriter, "    ");

            codeWriter.AppendParts(Parts, new UnionAttributePartInput(syntax, types));

            streamWriter.Flush();
            ctx.AddSource($"{syntax.Identifier.Text}_Union.g.cs", SourceText.From(memoryStream, Encoding.UTF8, canBeEmbedded: true));
        });
    }
}