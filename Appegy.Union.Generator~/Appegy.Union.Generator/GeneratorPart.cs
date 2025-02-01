using System.CodeDom.Compiler;

namespace Appegy.Union.Generator;

public abstract class GeneratorPart<T> where T : struct
{
    public virtual bool NeedNewLine { get; } = true;
    public abstract void Generate(IndentedTextWriter codeWriter, T input);
}