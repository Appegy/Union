using System.CodeDom.Compiler;

namespace Appegy.Union.Generator;

public class UnionFieldsRegion : GeneratorPart<UnionAttributePartInput>
{
    public override string Description => "Private fields";

    public override void Generate(IndentedTextWriter codeWriter, UnionAttributePartInput input)
    {
        var ( _, types) = input;

        codeWriter.WriteLine("[FieldOffset(0)]");
        codeWriter.WriteLine("private Kind _type;");

        foreach (var type in types)
        {
            codeWriter.WriteLine("[FieldOffset(1)]");
            codeWriter.Write("private ");
            codeWriter.Write(type.ToDisplayString());
            codeWriter.Write(" _");
            codeWriter.Write(type.Name.ToCamelCase());
            codeWriter.WriteLine(";");
        }
    }
}