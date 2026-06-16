using System.CodeDom.Compiler;

namespace Appegy.Union.Generator;

public class UnionFieldsPart : GeneratorPart<UnionAttributePartInput>
{
    public override void Generate(IndentedTextWriter codeWriter, UnionAttributePartInput input)
    {
        var types = input.Types;
        var explicitLayout = input.ExplicitLayout;

        if (explicitLayout)
        {
            codeWriter.WriteLine("[global::System.Runtime.InteropServices.FieldOffset(0)]");
        }
        codeWriter.WriteLine("private Kind _type;");

        foreach (var type in types)
        {
            if (explicitLayout)
            {
                codeWriter.WriteLine("[global::System.Runtime.InteropServices.FieldOffset(1)]");
            }
            codeWriter.Write("private ");
            codeWriter.Write(type.FullName);
            codeWriter.Write(' ');
            codeWriter.Write(type.FieldName);
            codeWriter.WriteLine(';');
        }
    }
}