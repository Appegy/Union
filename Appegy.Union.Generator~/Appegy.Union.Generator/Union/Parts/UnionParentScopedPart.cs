using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Appegy.Union.Generator;

public class UnionParentScopedPart(IReadOnlyList<GeneratorPart<UnionAttributePartInput>> innerParts) : ParentScopedPart<UnionAttributePartInput>(innerParts)
{
    protected override TypeDeclarationSyntax GetTypeSyntax(UnionAttributePartInput input) => input.Syntax;
}