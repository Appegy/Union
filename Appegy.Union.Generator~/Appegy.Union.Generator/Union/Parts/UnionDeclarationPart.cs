using System.CodeDom.Compiler;

namespace Appegy.Union.Generator;

public class UnionDeclarationPart : GeneratorPart<UnionAttributePartInput>
{
    public override void Generate(IndentedTextWriter codeWriter, UnionAttributePartInput input)
    {
        var syntax = input.Syntax;
        var types = input.Types;

        codeWriter.WriteLine(AttributesNames.GeneratedCodeAttribute);
        codeWriter.Write("partial struct ");
        codeWriter.WriteLine(syntax.Identifier.Text);

        codeWriter.Write("    : global::System.IEquatable<");
        codeWriter.Write(syntax.Identifier.Text);
        codeWriter.WriteLine(">");

        foreach (var type in types)
        {
            codeWriter.Write("    , global::System.IEquatable<");
            codeWriter.Write(type.FullName);
            codeWriter.WriteLine(">");
        }
    }
}