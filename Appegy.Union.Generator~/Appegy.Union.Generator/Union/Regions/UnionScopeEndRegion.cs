using System.CodeDom.Compiler;

namespace Appegy.Union.Generator;

public class UnionScopeEndRegion: GeneratorPart<UnionAttributePartInput>
{
    public override string Description => "";
    public override bool NewLineAtEnd => false;

    public override void Generate(IndentedTextWriter codeWriter, UnionAttributePartInput input)
    {
        codeWriter.Indent--;
        codeWriter.WriteLine('}');
    }
}