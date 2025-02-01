using System.CodeDom.Compiler;

namespace Appegy.Union.Generator;

public class UnionTypeEnumRegion : GeneratorPart<UnionAttributePartInput>
{
    public override string Description => "Enum for wrapped types";

    public override void Generate(IndentedTextWriter codeWriter, UnionAttributePartInput input)
    {
        var (_, types) = input;

        codeWriter.WriteLine("[Serializable]");
        codeWriter.WriteLine("public enum Kind : byte");
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