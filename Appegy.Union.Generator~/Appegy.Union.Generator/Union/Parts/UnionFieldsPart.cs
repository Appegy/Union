using System.CodeDom.Compiler;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace Appegy.Union.Generator;

public class UnionFieldsPart : GeneratorPart<UnionAttributePartInput>
{
    public override void Generate(IndentedTextWriter codeWriter, UnionAttributePartInput input)
    {
        var ( _, types) = input;

        codeWriter.WriteLine("[global::System.Runtime.InteropServices.FieldOffset(0)]");
        codeWriter.WriteLine("private Kind _type;");

        foreach (var type in types)
        {
            codeWriter.WriteLine("[global::System.Runtime.InteropServices.FieldOffset(1)]");
            codeWriter.Write("private ");
            codeWriter.Write(type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
            codeWriter.Write(' ');
            codeWriter.WriteFieldName(type);
            codeWriter.WriteLine(';');
        }
    }
}