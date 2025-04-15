using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Appegy.Union.Generator;

public record struct UnionAttributePartInput(StructDeclarationSyntax Syntax, IReadOnlyList<UnionTypeInfo> Types);

public readonly struct UnionTypeInfo(INamedTypeSymbol symbol)
{
    public string Name => Symbol.Name;
    public readonly INamedTypeSymbol Symbol = symbol;
    public readonly string FullName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    public readonly string FieldName = GetFieldName(symbol);

    private static string GetFieldName(INamedTypeSymbol symbol)
    {
        var name = symbol.Name;

        if (string.IsNullOrEmpty(name)) return string.Empty;

        return name.Length > 1
            ? "_" + char.ToLower(name[0]) + name.Substring(1)
            : "_" + char.ToLower(name[0]);
    }
}