using System.CodeDom.Compiler;

namespace Appegy.Union.Generator;

public class UnionPropertiesRegion : GeneratorPart<UnionAttributePartInput>
{
    public override string Description => "Public properties";

    public override void Generate(IndentedTextWriter codeWriter, UnionAttributePartInput input)
    {
        var (_, types) = input;

        codeWriter.WriteLine("public Kind Type => _type;");
        codeWriter.WriteLine();

        for (var i = 0; i < types.Count; i++)
        {
            var type = types[i];
            var typeName = type.Name;
            var fieldName = "_" + typeName.ToCamelCase();

            codeWriter.Write("public ");
            codeWriter.Write(type.ToDisplayString());
            codeWriter.Write(" ");
            codeWriter.WriteLine(typeName);
            codeWriter.WriteLine('{');
            codeWriter.Indent++;
            codeWriter.Write("get => Type != Kind.");
            codeWriter.Write(typeName);
            codeWriter.Write(" ? throw new Exception($\"Can't get ");
            codeWriter.Write(typeName);
            codeWriter.Write(" because current type is '{Type}'.\") : ");
            codeWriter.Write(fieldName);
            codeWriter.WriteLine(";");
            codeWriter.WriteLine("set");
            codeWriter.WriteLine('{');
            codeWriter.Indent++;
            codeWriter.Write("_type = Kind.");
            codeWriter.Write(typeName);
            codeWriter.WriteLine(";");
            codeWriter.Write(fieldName);
            codeWriter.WriteLine(" = value;");
            codeWriter.Indent--;
            codeWriter.WriteLine('}');
            codeWriter.Indent--;
            codeWriter.WriteLine('}');

            if (i < types.Count - 1)
            {
                codeWriter.WriteLine();
            }
        }
    }
}