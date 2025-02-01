using System.CodeDom.Compiler;

namespace Appegy.Union.Generator;

public class UnionToStringRegion : GeneratorPart<UnionAttributePartInput>
{
    public override string Description => "Override .ToString";

    public override void Generate(IndentedTextWriter codeWriter, UnionAttributePartInput input)
    {
        var (_, types) = input;

        codeWriter.WriteLine("public override string ToString() => _type switch");
        codeWriter.WriteLine('{');
        codeWriter.Indent++;
        foreach (var type in types)
        {
            codeWriter.Write("Kind.");
            codeWriter.Write(type.Name);
            codeWriter.Write(" => _");
            codeWriter.Write(type.Name.ToCamelCase());
            codeWriter.WriteLine(".ToString(),");
        }
        codeWriter.WriteLine("_ => throw new InvalidOperationException($\"Unknown type of union: {_type}\")");
        codeWriter.Indent--;
        codeWriter.WriteLine("};");
    }
}