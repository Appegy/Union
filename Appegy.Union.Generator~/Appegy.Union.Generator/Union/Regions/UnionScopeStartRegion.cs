using System.CodeDom.Compiler;

namespace Appegy.Union.Generator;

public class UnionScopeStartRegion : GeneratorPart<UnionAttributePartInput>
{
    public override string Description => "";
    public override bool NewLineAtEnd => false;

    public override void Generate(IndentedTextWriter codeWriter, UnionAttributePartInput input)
    {
        codeWriter.WriteLine('{');
        codeWriter.Indent++;
    }
}