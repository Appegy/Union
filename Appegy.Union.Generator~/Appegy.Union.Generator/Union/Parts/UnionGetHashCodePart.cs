﻿using System.CodeDom.Compiler;

namespace Appegy.Union.Generator;

public class UnionGetHashCodePart : GeneratorPart<UnionAttributePartInput>
{
    public override void Generate(IndentedTextWriter codeWriter, UnionAttributePartInput input)
    {
        var (_, types) = input;

        codeWriter.WriteLine("public override int GetHashCode() => _type switch");
        codeWriter.WriteLine('{');
        codeWriter.Indent++;
        foreach (var type in types)
        {
            codeWriter.Write("Kind.");
            codeWriter.Write(type.Name);
            codeWriter.Write(" => ");
            codeWriter.Write(type.FieldName);
            codeWriter.WriteLine(".GetHashCode(),");
        }
        codeWriter.WriteLine("_ => throw new global::System.InvalidOperationException($\"Unknown type of union: {_type}\")");
        codeWriter.Indent--;
        codeWriter.WriteLine("};");
    }
}