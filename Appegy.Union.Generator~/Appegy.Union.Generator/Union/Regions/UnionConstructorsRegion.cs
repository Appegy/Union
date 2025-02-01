using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis;

namespace Appegy.Union.Generator;

public class UnionConstructorsRegion : TypePartRegion<UnionAttributePartInput>
{
    public override string Name => "Constructors";

    public override void Generate(IndentedTextWriter codeWriter, UnionAttributePartInput input)
    {
        var (syntax, types) = input;

        foreach (var type in types)
        {
            var typeName = type.Name;
            var fieldName = "_" + typeName.ToCamelCase();

            codeWriter.Write("public ");
            codeWriter.Write(syntax.Identifier.Text);
            codeWriter.Write("(");
            codeWriter.Write(type.ToDisplayString());
            codeWriter.WriteLine(" value)");
            codeWriter.WriteLine('{');
            codeWriter.Indent++;
            codeWriter.Write("_type = Kind.");
            codeWriter.Write(typeName);
            codeWriter.WriteLine(";");
            foreach (var otherType in types)
            {
                if (SymbolEqualityComparer.Default.Equals(otherType, type))
                {
                    continue;
                }
                codeWriter.Write("_");
                codeWriter.Write(otherType.Name.ToCamelCase());
                codeWriter.WriteLine(" = default;");
            }
            codeWriter.Write(fieldName);
            codeWriter.WriteLine(" = value;");
            codeWriter.Indent--;
            codeWriter.WriteLine('}');
            codeWriter.WriteLine();
        }
    }
}