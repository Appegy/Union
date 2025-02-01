using System.CodeDom.Compiler;

namespace Appegy.Union.Generator;

public abstract class GeneratorPart<T> where T : struct
{
    public abstract string Description { get; }
    public virtual bool NewLineAtStart => true;
    public virtual bool NewLineAtEnd => true;
    public virtual bool InRegion { get; } = false;

    public abstract void Generate(IndentedTextWriter codeWriter, T input);
}