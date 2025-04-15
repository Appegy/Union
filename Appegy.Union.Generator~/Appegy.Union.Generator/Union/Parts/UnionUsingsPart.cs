﻿using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace Appegy.Union.Generator;

public class UnionUsingsPart : GeneratorPart<UnionAttributePartInput>
{
    public override bool NeedNewLine => false;

    public override void Generate(IndentedTextWriter codeWriter, UnionAttributePartInput input)
    {
        var (_, types) = input;

        codeWriter.WriteLine("using System;");
        codeWriter.WriteLine("using System.Runtime.InteropServices;");

        var set = new HashSet<string>();

        foreach (var type in types)
        {
            var nsSymbol = type.ContainingNamespace;
            if (!nsSymbol.IsGlobalNamespace)
            {
                set.Add(nsSymbol.ToDisplayString());
            }
        }

        foreach (var ns in set)
        {
            codeWriter.Write("using ");
            codeWriter.Write(ns);
            codeWriter.WriteLine(";");
        }
    }
}