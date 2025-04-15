using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace Appegy.Union.Generator;

public class ScopedPart<T>(IReadOnlyList<GeneratorPart<T>> innerParts) : GeneratorPart<T>
    where T : struct
{
    protected readonly IReadOnlyList<GeneratorPart<T>> InnerParts = innerParts;

    public override bool NeedNewLine => false;

    public override void Generate(IndentedTextWriter codeWriter, T input)
    {
        codeWriter.WriteLine('{');
        codeWriter.Indent++;

        codeWriter.AppendParts(InnerParts, input);

        codeWriter.Indent--;
        codeWriter.WriteLine('}');
    }
}