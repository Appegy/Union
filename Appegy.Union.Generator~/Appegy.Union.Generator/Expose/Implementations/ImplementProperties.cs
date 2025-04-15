using System.CodeDom.Compiler;
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
            GenerateGetterBody(codeWriter, propertySymbol, types);
        }
        if (propertySymbol.SetMethod != null)
        {
            GenerateSetterBody(codeWriter, propertySymbol, types);
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

    private static void GenerateGetterBody(IndentedTextWriter codeWriter, IPropertySymbol propertySymbol, IReadOnlyList<INamedTypeSymbol> types)
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
            codeWriter.Write(": return ");
            codeWriter.WriteFieldName(type);
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

    private static void GenerateSetterBody(IndentedTextWriter codeWriter, IPropertySymbol propertySymbol, IReadOnlyList<INamedTypeSymbol> types)
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
            codeWriter.Write(": ");
            codeWriter.WriteFieldName(type);
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