using System.CodeDom.Compiler;

namespace Appegy.Union.Generator;

public class ExposeDeclarationPart : GeneratorPart<ExposeAttributePartInput>
{
    public override void Generate(IndentedTextWriter codeWriter, ExposeAttributePartInput input)
    {
        var (syntax, _, interfaces) = input;

        codeWriter.Write("public partial struct ");
        codeWriter.Write(syntax.Identifier.Text);

        codeWriter.WriteLine(" :");
        codeWriter.Indent++;
        for (var i = 0; i < interfaces.Count; i++)
        {
            var @interface = interfaces[i];
            codeWriter.Write(@interface.Name);
            if (i != interfaces.Count - 1)
            {
                codeWriter.WriteLine(",");
            }
        }
        codeWriter.Indent--;
        codeWriter.WriteLine();
    }
}