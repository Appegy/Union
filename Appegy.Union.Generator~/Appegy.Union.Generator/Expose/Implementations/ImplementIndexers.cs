using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Appegy.Union.Generator;

public class ImplementIndexers : ExposeInterfacePart.Implementation
{
    public override bool TryGenerateMember(IndentedTextWriter codeWriter, ISymbol member, IReadOnlyList<ExposeTypeInfo> types)
    {
        switch (member)
        {
            case IPropertySymbol { IsIndexer: true } propertySymbol:
                GenerateIndexer(codeWriter, propertySymbol, types);
                return true;
            default:
                return false;
        }
    }

    private static void GenerateIndexer(IndentedTextWriter codeWriter, IPropertySymbol propertySymbol, IReadOnlyList<ExposeTypeInfo> types)
    {
        GenerateIndexerHeader(codeWriter, propertySymbol);
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

    private static void GenerateIndexerHeader(IndentedTextWriter codeWriter, IPropertySymbol propertySymbol)
    {
        codeWriter.Write("public ");
        codeWriter.Write(propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        codeWriter.Write(" this[");
        var parameters = propertySymbol.Parameters;
        for (var i = 0; i < parameters.Length; i++)
        {
            if (i > 0)
            {
                codeWriter.Write(", ");
            }
            codeWriter.Write(parameters[i].Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
            codeWriter.Write(" ");
            codeWriter.Write(parameters[i].Name);
        }
        codeWriter.WriteLine("]");
    }

    private static void GenerateGetterBody(IndentedTextWriter codeWriter, IPropertySymbol propertySymbol, IReadOnlyList<ExposeTypeInfo> types)
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
            codeWriter.Write(type.FieldName);
            codeWriter.Write("[");
            GenerateIndexerArguments(codeWriter, propertySymbol);
            codeWriter.WriteLine("];");
        }
        codeWriter.WriteLine("default: throw new global::System.InvalidOperationException($\"Unknown type of union: {_type}\");");
        codeWriter.Indent--;
        codeWriter.WriteLine("}");
        codeWriter.Indent--;
        codeWriter.WriteLine("}");
    }

    private static void GenerateSetterBody(IndentedTextWriter codeWriter, IPropertySymbol propertySymbol, IReadOnlyList<ExposeTypeInfo> types)
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
            codeWriter.Write(type.FieldName);
            codeWriter.Write("[");
            GenerateIndexerArguments(codeWriter, propertySymbol);
            codeWriter.Write("] = value; break;");
            codeWriter.WriteLine();
        }
        codeWriter.WriteLine("default: throw new global::System.InvalidOperationException($\"Unknown type of union: {_type}\");");
        codeWriter.Indent--;
        codeWriter.WriteLine("}");
        codeWriter.Indent--;
        codeWriter.WriteLine("}");
    }

    private static void GenerateIndexerArguments(IndentedTextWriter codeWriter, IPropertySymbol propertySymbol)
    {
        var parameters = propertySymbol.Parameters;
        for (var i = 0; i < parameters.Length; i++)
        {
            if (i > 0)
            {
                codeWriter.Write(", ");
            }
            codeWriter.Write(parameters[i].Name);
        }
    }
}