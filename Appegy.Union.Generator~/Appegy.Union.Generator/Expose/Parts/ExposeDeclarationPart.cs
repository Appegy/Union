using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Appegy.Union.Generator;

public class ExposeDeclarationPart : GeneratorPart<ExposeAttributePartInput>
{
    public override void Generate(IndentedTextWriter codeWriter, ExposeAttributePartInput input)
    {
        var (syntax, _, interfaces) = input;

        codeWriter.Write("partial ");

        switch (syntax)
        {
            case StructDeclarationSyntax:
                codeWriter.Write("struct ");
                break;
            case ClassDeclarationSyntax:
                codeWriter.Write("class ");
                break;
        }

        codeWriter.Write(syntax.Identifier.Text);

        codeWriter.WriteLine(" :");
        codeWriter.Indent++;
        for (var i = 0; i < interfaces.Count; i++)
        {
            var @interface = interfaces[i];
            codeWriter.Write(@interface.FullName);
            if (i != interfaces.Count - 1)
            {
                codeWriter.WriteLine(",");
            }
        }
        codeWriter.Indent--;
        codeWriter.WriteLine();
    }
}