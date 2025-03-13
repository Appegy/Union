using System.CodeDom.Compiler;

namespace Appegy.Union.Generator;

public class UnionPropertiesPart : GeneratorPart<UnionAttributePartInput>
{
    public override void Generate(IndentedTextWriter codeWriter, UnionAttributePartInput input)
    {
        var (_, types) = input;

        codeWriter.WriteLine("public Kind Type => _type;");
        codeWriter.WriteLine();

        for (var i = 0; i < types.Count; i++)
        {
            var type = types[i];
            var typeName = type.Name;

            codeWriter.Write("public ");
            codeWriter.Write(type.Name);
            codeWriter.Write(" ");
            codeWriter.WriteLine(typeName);
            codeWriter.WriteLine('{');
            codeWriter.Indent++;
            codeWriter.Write("get => Type != Kind.");
            codeWriter.Write(typeName);
            codeWriter.Write(" ? throw new Exception($\"Can't get ");
            codeWriter.Write(typeName);
            codeWriter.Write(" because current type is '{Type}'.\") : ");
            codeWriter.WriteFieldName(type);
            codeWriter.WriteLine(";");
            codeWriter.WriteLine("set");
            codeWriter.WriteLine('{');
            codeWriter.Indent++;
            codeWriter.Write("_type = Kind.");
            codeWriter.Write(typeName);
            codeWriter.WriteLine(";");
            codeWriter.WriteFieldName(type);
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