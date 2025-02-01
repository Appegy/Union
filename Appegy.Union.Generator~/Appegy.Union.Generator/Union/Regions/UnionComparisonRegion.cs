using System.CodeDom.Compiler;

namespace Appegy.Union.Generator;

public class UnionComparisonRegion : TypePartRegion<UnionAttributePartInput>
{
    public override string Name => "Override .GetHashCode";

    public override void Generate(IndentedTextWriter codeWriter, UnionAttributePartInput input)
    {
        var (syntax, types) = input;

        codeWriter.Write("public static bool operator ==(");
        codeWriter.Write(syntax.Identifier.Text);
        codeWriter.Write(" a, ");
        codeWriter.Write(syntax.Identifier.Text);
        codeWriter.WriteLine(" b) => a.Equals(b);");

        codeWriter.Write("public static bool operator !=(");
        codeWriter.Write(syntax.Identifier.Text);
        codeWriter.Write(" a, ");
        codeWriter.Write(syntax.Identifier.Text);
        codeWriter.WriteLine(" b) => !a.Equals(b);");
        codeWriter.WriteLine();

        for (var i = 0; i < types.Count; i++)
        {
            var type = types[i];
            codeWriter.Write("public static bool operator ==(");
            codeWriter.Write(syntax.Identifier.Text);
            codeWriter.Write(" a, ");
            codeWriter.Write(type.ToDisplayString());
            codeWriter.WriteLine(" b) => a.Equals(b);");

            codeWriter.Write("public static bool operator !=(");
            codeWriter.Write(syntax.Identifier.Text);
            codeWriter.Write(" a, ");
            codeWriter.Write(type.ToDisplayString());
            codeWriter.WriteLine(" b) => !a.Equals(b);");
            if (i < types.Count - 1)
            {
                codeWriter.WriteLine();
            }
        }
    }
}