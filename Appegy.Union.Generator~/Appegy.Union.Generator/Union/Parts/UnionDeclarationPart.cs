﻿using System.CodeDom.Compiler;

namespace Appegy.Union.Generator;

public class UnionDeclarationPart : GeneratorPart<UnionAttributePartInput>
{
    public override void Generate(IndentedTextWriter codeWriter, UnionAttributePartInput input)
    {
        var (syntax, types) = input;

        codeWriter.WriteLine(AttributesSource.GeneratedCodeAttribute);
        codeWriter.WriteLine("[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Explicit, Pack = 1)]");
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