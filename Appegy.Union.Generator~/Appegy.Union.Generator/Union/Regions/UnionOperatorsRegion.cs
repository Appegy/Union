using System.CodeDom.Compiler;

namespace Appegy.Union.Generator;

public class UnionOperatorsRegion : TypePartRegion<UnionAttributePartInput>
{
    public override string Name => "Override .GetHashCode";

    public override void Generate(IndentedTextWriter codeWriter, UnionAttributePartInput input)
    {
        var (syntax, types) = input;

        for (var i = 0; i < types.Count; i++)
        {
            var type = types[i];
            var typeName = type.Name;

            codeWriter.Write("public static implicit operator ");
            codeWriter.Write(type.ToDisplayString());
            codeWriter.Write("(");
            codeWriter.Write(syntax.Identifier.Text);
            codeWriter.Write(" other) => other.");
            codeWriter.Write(typeName);
            codeWriter.WriteLine(";");

            codeWriter.Write("public static implicit operator ");
            codeWriter.Write(syntax.Identifier.Text);
            codeWriter.Write("(");
            codeWriter.Write(type.ToDisplayString());

            codeWriter.Write(" other) => new ");
            codeWriter.Write(syntax.Identifier.Text);
            codeWriter.WriteLine("(other);");
            codeWriter.WriteLine();
        }
    }
}