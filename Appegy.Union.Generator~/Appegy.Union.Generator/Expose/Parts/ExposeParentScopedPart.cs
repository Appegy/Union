using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Appegy.Union.Generator;

public class ExposeParentScopedPart(IReadOnlyList<GeneratorPart<ExposeAttributePartInput>> innerParts) : ParentScopedPart<ExposeAttributePartInput>(innerParts)
{
    protected override TypeDeclarationSyntax GetTypeSyntax(ExposeAttributePartInput input) => input.Syntax;
}