﻿using System.CodeDom.Compiler;
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
public class UnionAttributeGenerator : IIncrementalGenerator
{
    private static ImmutableArray<GeneratorPart<UnionAttributePartInput>> Regions { get; } =
        ImmutableArray.Create<GeneratorPart<UnionAttributePartInput>>(
            new UnionUsingsRegion(),
            new UnionParentStartRegion(),
            new UnionDeclarationRegion(),
            new UnionScopeStartRegion(),
            new UnionTypeEnumRegion(),
            new UnionFieldsRegion(),
            new UnionPropertiesRegion(),
            new UnionConstructorsRegion(),
            new UnionToStringRegion(),
            new UnionGetHashCodeRegion(),
            new UnionEqualsRegion(),
            new UnionOperatorsRegion(),
            new UnionComparisonRegion(),
            new UnionScopeEndRegion(),
            new UnionParentEndRegion());

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var sources = context
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

        context.RegisterSourceOutput(sources, static (ctx, input) =>
        {
            var (syntax, types) = input;

            if (!syntax.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                return;
            }

            if (types.Count == 0)
            {
                return;
            }

            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);
            using var codeWriter = new IndentedTextWriter(streamWriter, "    ");

            codeWriter.AppendParts(Regions, new UnionAttributePartInput(syntax, types));

            streamWriter.Flush();
            ctx.AddSource($"{syntax.Identifier.Text}_Union.g.cs", SourceText.From(memoryStream, Encoding.UTF8, canBeEmbedded: true));
        });
    }
}