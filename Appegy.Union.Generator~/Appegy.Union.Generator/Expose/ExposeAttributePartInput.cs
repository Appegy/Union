using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Appegy.Union.Generator;

public record struct ExposeAttributePartInput(StructDeclarationSyntax Syntax, IReadOnlyList<INamedTypeSymbol> Types, IReadOnlyList<INamedTypeSymbol> Interfaces);

public record struct ExposeInterfacePartInput(StructDeclarationSyntax Syntax, IReadOnlyList<INamedTypeSymbol> Types, IReadOnlyList<ISymbol> Members);