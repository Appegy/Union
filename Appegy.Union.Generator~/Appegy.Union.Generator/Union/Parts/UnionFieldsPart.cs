using System.CodeDom.Compiler;

namespace Appegy.Union.Generator;

public class UnionFieldsPart : GeneratorPart<UnionAttributePartInput>
{
    public override void Generate(IndentedTextWriter codeWriter, UnionAttributePartInput input)
    {
        var ( _, types) = input;

        codeWriter.WriteLine("[FieldOffset(0)]");
        codeWriter.WriteLine("private Kind _type;");

        foreach (var type in types)
        {
            codeWriter.WriteLine("[FieldOffset(1)]");
            codeWriter.Write("private ");
            codeWriter.Write(type.Name);
            codeWriter.Write(' ');
            codeWriter.WriteFieldName(type);
            codeWriter.WriteLine(';');
        }
    }
}