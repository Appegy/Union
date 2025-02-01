using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Appegy.Union.Generator;

public class UnionParentStartRegion : GeneratorPart<UnionAttributePartInput>
{
    public override string Description => "";
    public override bool NewLineAtEnd => false;

    public override void Generate(IndentedTextWriter codeWriter, UnionAttributePartInput input)
    {
        var (syntax, _) = input;

        if (syntax.Parent is NamespaceDeclarationSyntax namespaceDeclaration)
        {
            codeWriter.Write("namespace ");
            codeWriter.WriteLine(namespaceDeclaration.Name);
            codeWriter.WriteLine('{');
            codeWriter.Indent++;
        }
    }
}