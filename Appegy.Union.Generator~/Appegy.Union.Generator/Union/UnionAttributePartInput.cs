using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Appegy.Union.Generator;

public record struct UnionAttributePartInput(StructDeclarationSyntax Syntax, ImmutableList<INamedTypeSymbol> Types);