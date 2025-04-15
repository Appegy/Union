using System.CodeDom.Compiler;

namespace Appegy.Union.Generator;

public class UnionEqualsPart : GeneratorPart<UnionAttributePartInput>
{
    public override void Generate(IndentedTextWriter codeWriter, UnionAttributePartInput input)
    {
        var (syntax, types) = input;

        codeWriter.WriteLine("public override bool Equals(object boxed) => boxed switch");
        codeWriter.WriteLine('{');
        codeWriter.Indent++;
        codeWriter.Write(syntax.Identifier.Text);
        codeWriter.WriteLine(" other => Equals(other),");
        foreach (var type in types)
        {
            codeWriter.Write(type.Name);
            codeWriter.WriteLine(" other => Equals(other),");
        }
        codeWriter.WriteLine("_ => throw new InvalidOperationException($\"Unknown type of union: {_type}\")");
        codeWriter.Indent--;
        codeWriter.WriteLine("};");
        codeWriter.WriteLine();

        codeWriter.Write("public bool Equals(");
        codeWriter.Write(syntax.Identifier.Text);
        codeWriter.WriteLine(" other) => _type == other.Type && _type switch");
        codeWriter.WriteLine('{');
        codeWriter.Indent++;
        foreach (var type in types)
        {
            codeWriter.Write("Kind.");
            codeWriter.Write(type.Name);
            codeWriter.Write(" => ");
            codeWriter.WriteFieldName(type);
            codeWriter.Write(".Equals(other.");
            codeWriter.Write(type.Name);
            codeWriter.WriteLine("),");
        }
        codeWriter.WriteLine("_ => throw new InvalidOperationException($\"Unknown type of union: {_type}\")");
        codeWriter.Indent--;
        codeWriter.WriteLine("};");
        codeWriter.WriteLine();

        foreach (var type in types)
        {
            codeWriter.Write("public bool Equals(");
            codeWriter.Write(type.Name);
            codeWriter.Write(" other) => _type == Kind.");
            codeWriter.Write(type.Name);
            codeWriter.Write(" && ");
            codeWriter.WriteFieldName(type);
            codeWriter.WriteLine(".Equals(other);");
        }
    }
}