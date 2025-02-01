using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Appegy.Union.Generator;

public class UnionParentScopedPart(IReadOnlyList<GeneratorPart<UnionAttributePartInput>> innerParts) : ScopedGeneratorPart<UnionAttributePartInput>(innerParts)
{
    public override string Description => "";

    public override void Generate(IndentedTextWriter codeWriter, UnionAttributePartInput input)
    {
        var (syntax, _) = input;

        if (syntax.Parent is NamespaceDeclarationSyntax namespaceDeclaration)
        {
            codeWriter.Write("namespace ");
            codeWriter.WriteLine(namespaceDeclaration.Name);
            base.Generate(codeWriter, input);
        }
    }
}