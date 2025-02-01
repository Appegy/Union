using System.CodeDom.Compiler;

namespace Appegy.Union.Generator;

public class UnionGetHashCodeRegion : TypePartRegion<UnionAttributePartInput>
{
    public override string Name => "Override .GetHashCode";

    public override void Generate(IndentedTextWriter codeWriter, UnionAttributePartInput input)
    {
        var (_, types) = input;

        codeWriter.WriteLine("public override int GetHashCode() => _type switch");
        codeWriter.WriteLine('{');
        codeWriter.Indent++;
        foreach (var type in types)
        {
            codeWriter.Write("Kind.");
            codeWriter.Write(type.Name);
            codeWriter.Write(" => _");
            codeWriter.Write(type.Name.ToCamelCase());
            codeWriter.WriteLine(".GetHashCode(),");
        }
        codeWriter.WriteLine("_ => throw new InvalidOperationException($\"Unknown type of union: {_type}\")");
        codeWriter.Indent--;
        codeWriter.WriteLine("};");
        codeWriter.WriteLine();
    }
}