using System.CodeDom.Compiler;

namespace Appegy.Union.Generator;

public class UnionMatchPart : GeneratorPart<UnionAttributePartInput>
{
    public override void Generate(IndentedTextWriter codeWriter, UnionAttributePartInput input)
    {
        var types = input.Types;

        codeWriter.WriteLine("public void Match(");

        using (codeWriter.IndentScope())
        {
            for (var i = 0; i < types.Count; i++)
            {
                var type = types[i];
                codeWriter.Write("global::System.Action<");
                codeWriter.Write(type.FullName);
                codeWriter.Write('>');
                codeWriter.Write(' ');
                codeWriter.Write(type.ParamName);
                codeWriter.WriteLine(i != types.Count - 1 ? "," : ")");
            }
        }

        codeWriter.WriteLine("{");
        using (codeWriter.IndentScope())
        {
            codeWriter.WriteLine("switch (Type)");
            codeWriter.WriteLine("{");
            using (codeWriter.IndentScope())
            {
                foreach (var type in types)
                {
                    codeWriter.Write("case Kind.");
                    codeWriter.Write(type.Name);
                    codeWriter.Write(": ");
                    codeWriter.Write(type.ParamName);
                    codeWriter.Write("(");
                    codeWriter.Write(type.FieldName);
                    codeWriter.Write("); break;");
                    codeWriter.WriteLine();
                }
                codeWriter.WriteLine("default: throw new global::System.InvalidOperationException($\"Unknown type of union: {_type}\");");
            }
            codeWriter.WriteLine("}");
        }
        codeWriter.WriteLine("}");
    }
}