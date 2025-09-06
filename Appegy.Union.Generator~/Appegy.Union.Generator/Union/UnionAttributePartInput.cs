using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Appegy.Union.Generator;

public record struct UnionAttributePartInput(TypeDeclarationSyntax Syntax, IReadOnlyList<UnionTypeInfo> Types, bool ExplicitLayout);

public readonly struct UnionTypeInfo(INamedTypeSymbol symbol)
{
    public string Name => Symbol.Name;
    public readonly INamedTypeSymbol Symbol = symbol;
    public readonly string FullName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    public readonly string FieldName = symbol.GetFieldName();
    public readonly string ParamName = symbol.GetParamName();
}