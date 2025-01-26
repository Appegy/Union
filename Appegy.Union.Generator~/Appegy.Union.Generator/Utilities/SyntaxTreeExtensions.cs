using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Appegy.Union.Generator.Utilities;

public static class SyntaxTreeExtensions
{
    public static ImmutableList<INamedTypeSymbol> GetTypesFromConstructor(this AttributeData attribute, TypeKind? typeKind = null)
    {
        var types = ImmutableList.CreateBuilder<INamedTypeSymbol>();
        var argument = attribute.ConstructorArguments.First();

        foreach (var typedConstant in argument.Values)
        {
            if (typedConstant.Value is not INamedTypeSymbol symbol)
            {
                continue;
            }
            if (typeKind != null && symbol.TypeKind != typeKind.Value)
            {
                continue;
            }
            types.Add(symbol);
        }

        return types.ToImmutable();
    }
}