using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Appegy.Union.Generator;

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

    public static string GetFieldName(this INamedTypeSymbol symbol)
    {
        var name = symbol.Name;

        if (string.IsNullOrEmpty(name)) return string.Empty;

        return name.Length > 1
            ? "_" + char.ToLower(name[0]) + name.Substring(1)
            : "_" + char.ToLower(name[0]);
    }
}