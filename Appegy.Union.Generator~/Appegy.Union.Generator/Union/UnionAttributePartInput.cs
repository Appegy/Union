using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Appegy.Union.Generator;

public record struct UnionAttributePartInput(StructDeclarationSyntax Syntax, IReadOnlyList<INamedTypeSymbol> Types);