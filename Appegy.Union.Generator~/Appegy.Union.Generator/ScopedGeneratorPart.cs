using System.CodeDom.Compiler;
using System.Collections.Immutable;

namespace Appegy.Union.Generator;

public class ScopedGeneratorPart<T>(ImmutableArray<GeneratorPart<T>> innerParts) : GeneratorPart<T>
    where T : struct
{
    protected readonly ImmutableArray<GeneratorPart<T>> InnerParts = innerParts;

    public override string Description => "";

    public override void Generate(IndentedTextWriter codeWriter, T input)
    {
        codeWriter.WriteLine('{');
        codeWriter.Indent++;

        codeWriter.AppendParts(InnerParts, input);

        codeWriter.Indent--;
        codeWriter.WriteLine('}');
    }
}