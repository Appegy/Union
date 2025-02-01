﻿using System.CodeDom.Compiler;

namespace Appegy.Union.Generator;

public class UnionFieldsRegion : TypePartRegion<UnionAttributePartInput>
{
    public override string Name => "Private fields";

    public override void Generate(IndentedTextWriter codeWriter, UnionAttributePartInput input)
    {
        var ( _, types) = input;

        codeWriter.WriteLine("[FieldOffset(0)]");
        codeWriter.WriteLine("private Kind _type;");
        codeWriter.WriteLine();

        foreach (var type in types)
        {
            codeWriter.WriteLine("[FieldOffset(1)]");
            codeWriter.Write("private ");
            codeWriter.Write(type.ToDisplayString());
            codeWriter.Write(" _");
            codeWriter.Write(type.Name.ToCamelCase());
            codeWriter.WriteLine(";");
        }
        codeWriter.WriteLine();
    }
}