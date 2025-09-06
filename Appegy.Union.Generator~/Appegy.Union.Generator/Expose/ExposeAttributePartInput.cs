using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Appegy.Union.Generator;

public record struct ExposeAttributePartInput(TypeDeclarationSyntax Syntax, IReadOnlyList<ExposeTypeInfo> Types, IReadOnlyList<ExposeInterfaceInfo> Interfaces);

public readonly struct ExposeTypeInfo(INamedTypeSymbol symbol)
{
    public string Name => Symbol.Name;
    public readonly INamedTypeSymbol Symbol = symbol;
    public readonly string FieldName = symbol.GetFieldName();
}

public readonly struct ExposeInterfaceInfo(INamedTypeSymbol symbol)
{
    public readonly INamedTypeSymbol Symbol = symbol;
    public readonly string FullName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
}