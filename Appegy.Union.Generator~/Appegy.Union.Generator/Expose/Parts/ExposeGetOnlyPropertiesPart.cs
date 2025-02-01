using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Appegy.Union.Generator;

public class ExposeGetOnlyPropertiesPart : GeneratorPart<ExposeInterfacePartInput>
{
    public override void Generate(IndentedTextWriter codeWriter, ExposeInterfacePartInput input)
    {
        var (_, types, members) = input;

        for (var i = 0; i < members.Count; i++)
        {
            switch (members[i])
            {
                case IPropertySymbol { IsIndexer: false, SetMethod: null } propertySymbol:
                    GeneratePropertyImplementation(codeWriter, propertySymbol, types);
                    break;
            }
            if (i < members.Count - 1)
            {
                codeWriter.WriteLine();
            }
        }
    }

    private static void GeneratePropertyImplementation(IndentedTextWriter codeWriter, IPropertySymbol propertySymbol, IReadOnlyList<INamedTypeSymbol> types)
    {
        codeWriter.Write("public ");
        codeWriter.Write(propertySymbol.Type.ToDisplayString());
        codeWriter.Write(" ");
        codeWriter.Write(propertySymbol.Name);
        codeWriter.WriteLine(" => _type switch");
        codeWriter.WriteLine('{');
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
}