using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis;

namespace Appegy.Union.Generator;

public class UnionConstructorsPart : GeneratorPart<UnionAttributePartInput>
{
    public override void Generate(IndentedTextWriter codeWriter, UnionAttributePartInput input)
    {
        var (syntax, types) = input;

        for (var i = 0; i < types.Count; i++)
        {
            var type = types[i];
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

            if (i < types.Count - 1)
            {
                codeWriter.WriteLine();
            }
        }
    }
}