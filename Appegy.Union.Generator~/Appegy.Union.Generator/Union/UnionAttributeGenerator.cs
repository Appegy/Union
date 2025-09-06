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
        var sources = context
            .SyntaxProvider
            .ForAttributeWithMetadataName(
                UnionAttributeName,
                predicate: static (syntax, _) => syntax is StructDeclarationSyntax,
                transform: static (ctx, _) =>
                {
                    var syntax = (StructDeclarationSyntax)ctx.TargetNode;
                    var symbol = ctx.SemanticModel.GetDeclaredSymbol(syntax);
                    var attribute = ctx.Attributes.First();
                    var types = attribute
                        .GetTypesFromConstructor(TypeKind.Struct)
                        .Select(c => new UnionTypeInfo(c))
                        .ToImmutableList();

                    var explicitLayout = HasExplicitStructLayoutAttribute(symbol);

                    return (syntax, types, explicitLayout);
                });

        context.RegisterSourceOutput(sources, static (ctx, input) =>
        {
            var (syntax, types, explicitLayout) = input;

            if (!syntax.Modifiers.Any(SyntaxKind.PartialKeyword)) return;
            if (types.Count == 0) return;

            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);
            using var codeWriter = new IndentedTextWriter(streamWriter, "    ");

            codeWriter.AppendParts(Parts, new UnionAttributePartInput(syntax, types, explicitLayout));

            streamWriter.Flush();
            ctx.AddSource($"{syntax.Identifier.Text}_Union.g.cs", SourceText.From(memoryStream, Encoding.UTF8, canBeEmbedded: true));
        });
    }

    private static bool HasExplicitStructLayoutAttribute(INamedTypeSymbol? symbol)
    {
        if (symbol == null) return false;

        return symbol.GetAttributes().Any(attr =>
        {
            if (attr.AttributeClass?.Name != "StructLayoutAttribute" ||
                attr.AttributeClass.ContainingNamespace?.ToDisplayString() != "System.Runtime.InteropServices")
            {
                return false;
            }

            if (attr.ConstructorArguments.Length > 0)
            {
                var layoutKindArg = attr.ConstructorArguments[0];

                if (layoutKindArg.Type?.Name == "LayoutKind" && layoutKindArg.Type.ContainingNamespace?.ToDisplayString() == "System.Runtime.InteropServices")
                {
                    return layoutKindArg.Value is 2;
                }
            }

            return false;
        });
    }
}