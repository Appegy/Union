﻿using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace Appegy.Union.Generator;

public class ExposeUsingsPart: GeneratorPart<ExposeAttributePartInput>
{
    public override bool NeedNewLine => false;

    public override void Generate(IndentedTextWriter codeWriter, ExposeAttributePartInput input)
    {
        var (_, types, interfaces) = input;

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

        foreach (var type in interfaces)
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