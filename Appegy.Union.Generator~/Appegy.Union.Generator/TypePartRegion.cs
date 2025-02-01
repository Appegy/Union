using System.CodeDom.Compiler;

namespace Appegy.Union.Generator;

public abstract class TypePartRegion<T>
{
    public abstract string Name { get; }

    public abstract void Generate(IndentedTextWriter codeWriter, T input);
}