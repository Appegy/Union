﻿using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Appegy.Union.Generator;

public class ImplementProperties : ExposeInterfacePart.Implementation
{
    public override bool TryGenerateMember(IndentedTextWriter codeWriter, ISymbol member, IReadOnlyList<INamedTypeSymbol> types)
    {
        switch (member)
        {
            case IPropertySymbol { IsIndexer: false } propertySymbol:
                GenerateProperty(codeWriter, propertySymbol, types);
                return true;
            default:
                return false;
        }
    }

    private static void GenerateProperty(IndentedTextWriter codeWriter, IPropertySymbol propertySymbol, IReadOnlyList<INamedTypeSymbol> types)
    {
        GeneratePropertyHeader(codeWriter, propertySymbol);
        codeWriter.WriteLine("{");
        codeWriter.Indent++;
        if (propertySymbol.GetMethod != null)
        {
            GenerateGetter(codeWriter, propertySymbol, types);
        }
        if (propertySymbol.SetMethod != null)
        {
            GenerateSetter(codeWriter, propertySymbol, types);
        }
        codeWriter.Indent--;
        codeWriter.WriteLine("}");
    }

    private static void GeneratePropertyHeader(IndentedTextWriter codeWriter, IPropertySymbol propertySymbol)
    {
        codeWriter.Write("public ");
        codeWriter.Write(propertySymbol.Type.ToDisplayString());
        codeWriter.Write(" ");
        codeWriter.WriteLine(propertySymbol.Name);
    }

    private static void GenerateGetter(IndentedTextWriter codeWriter, IPropertySymbol propertySymbol, IReadOnlyList<INamedTypeSymbol> types)
    {
        codeWriter.WriteLine("get");
        codeWriter.WriteLine("{");
        codeWriter.Indent++;
        codeWriter.WriteLine("switch (_type)");
        codeWriter.WriteLine("{");
        codeWriter.Indent++;
        foreach (var type in types)
        {
            codeWriter.Write("case Kind.");
            codeWriter.Write(type.Name);
            codeWriter.Write(": return _");
            codeWriter.Write(type.Name.ToCamelCase());
            codeWriter.Write(".");
            codeWriter.Write(propertySymbol.Name);
            codeWriter.WriteLine(";");
        }
        codeWriter.WriteLine("default: throw new InvalidOperationException($\"Unknown type of union: {_type}\");");
        codeWriter.Indent--;
        codeWriter.WriteLine("}");
        codeWriter.Indent--;
        codeWriter.WriteLine("}");
    }

    private static void GenerateSetter(IndentedTextWriter codeWriter, IPropertySymbol propertySymbol, IReadOnlyList<INamedTypeSymbol> types)
    {
        codeWriter.WriteLine("set");
        codeWriter.WriteLine("{");
        codeWriter.Indent++;
        codeWriter.WriteLine("switch (_type)");
        codeWriter.WriteLine("{");
        codeWriter.Indent++;
        foreach (var type in types)
        {
            codeWriter.Write("case Kind.");
            codeWriter.Write(type.Name);
            codeWriter.Write(": _");
            codeWriter.Write(type.Name.ToCamelCase());
            codeWriter.Write(".");
            codeWriter.Write(propertySymbol.Name);
            codeWriter.Write(" = value; break;");
            codeWriter.WriteLine();
        }
        codeWriter.WriteLine("default: throw new InvalidOperationException($\"Unknown type of union: {_type}\");");
        codeWriter.Indent--;
        codeWriter.WriteLine("}");
        codeWriter.Indent--;
        codeWriter.WriteLine("}");
    }
}