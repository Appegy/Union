using System.CodeDom.Compiler;

namespace Appegy.Union.Generator;

public class UnionTypeEnumPart : GeneratorPart<UnionAttributePartInput>
{
    public override void Generate(IndentedTextWriter codeWriter, UnionAttributePartInput input)
    {
        var types = input.Types;

        codeWriter.WriteLine("[global::System.Serializable]");
        codeWriter.WriteLine("public enum Kind : global::System.Byte");
        codeWriter.WriteLine('{');
        codeWriter.Indent++;
        foreach (var type in types)
        {
            codeWriter.Write(type.Name);
            codeWriter.WriteLine(",");
        }
        codeWriter.Indent--;
        codeWriter.WriteLine('}');
    }
}