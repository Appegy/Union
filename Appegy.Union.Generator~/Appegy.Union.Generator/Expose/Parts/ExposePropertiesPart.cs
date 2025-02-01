using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Appegy.Union.Generator;

public class ExposePropertiesPart : GeneratorPart<ExposeInterfacePartInput>
{
    public override void Generate(IndentedTextWriter codeWriter, ExposeInterfacePartInput input)
    {
        var (_, types, members) = input;
        for (var i = 0; i < members.Count; i++)
        {
            switch (members[i])
            {
                case IPropertySymbol { IsIndexer: false, GetMethod: not null, SetMethod: null } propertySymbol:
                    GenerateGetOnlyPropertyImplementation(codeWriter, propertySymbol, types);
                    break;
                case IPropertySymbol { IsIndexer: false, GetMethod: null, SetMethod: not null } propertySymbol:
                    GenerateSetOnlyPropertyImplementation(codeWriter, propertySymbol, types);
                    break;
                case IPropertySymbol { IsIndexer: false, GetMethod: not null, SetMethod: not null } propertySymbol:
                    GenerateGetSetPropertyImplementation(codeWriter, propertySymbol, types);
                    break;
            }
            if (i < members.Count - 1)
            {
                codeWriter.WriteLine();
            }
        }
    }

    private static void GenerateGetOnlyPropertyImplementation(IndentedTextWriter codeWriter, IPropertySymbol propertySymbol, IReadOnlyList<INamedTypeSymbol> types)
    {
        codeWriter.Write("public ");
        codeWriter.Write(propertySymbol.Type.ToDisplayString());
        codeWriter.Write(" ");
        codeWriter.Write(propertySymbol.Name);
        codeWriter.WriteLine(" => _type switch");
        codeWriter.WriteLine("{");
        codeWriter.Indent++;
        foreach (var type in types)
        {
            codeWriter.Write("Kind.");
            codeWriter.Write(type.Name);
            codeWriter.Write(" => _");
            codeWriter.Write(type.Name.ToCamelCase());
            codeWriter.Write(".");
            codeWriter.Write(propertySymbol.Name);
            codeWriter.WriteLine(",");
        }
        codeWriter.WriteLine("_ => throw new InvalidOperationException($\"Unknown type of union: {_type}\")");
        codeWriter.Indent--;
        codeWriter.WriteLine("};");
    }

    private static void GenerateSetOnlyPropertyImplementation(IndentedTextWriter codeWriter, IPropertySymbol propertySymbol, IReadOnlyList<INamedTypeSymbol> types)
    {
        codeWriter.Write("public ");
        codeWriter.Write(propertySymbol.Type.ToDisplayString());
        codeWriter.Write(" ");
        codeWriter.WriteLine(propertySymbol.Name);
        codeWriter.WriteLine("{");
        codeWriter.Indent++;
        codeWriter.WriteLine("set");
        codeWriter.WriteLine("{");
        codeWriter.Indent++;
        codeWriter.WriteLine("switch(_type)");
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
        codeWriter.Indent--;
        codeWriter.WriteLine("}");
    }

    private static void GenerateGetSetPropertyImplementation(IndentedTextWriter codeWriter, IPropertySymbol propertySymbol, IReadOnlyList<INamedTypeSymbol> types)
    {
        codeWriter.Write("public ");
        codeWriter.Write(propertySymbol.Type.ToDisplayString());
        codeWriter.Write(" ");
        codeWriter.WriteLine(propertySymbol.Name);
        codeWriter.WriteLine("{");
        codeWriter.Indent++;
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
        codeWriter.Indent--;
        codeWriter.WriteLine("}");
    }
}