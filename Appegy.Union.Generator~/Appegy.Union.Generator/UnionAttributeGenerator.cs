﻿using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Appegy.Union.Generator.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Appegy.Union.Generator.AttributesNames;

namespace Appegy.Union.Generator;

[Generator]
public class UnionAttributeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var sources = context
            .SyntaxProvider
            .ForAttributeWithMetadataName(
                UnionAttributeName,
                predicate: static (syntax, _) => syntax is StructDeclarationSyntax,
                transform: static (ctx, _) =>
                {
                    var syntax = (StructDeclarationSyntax)ctx.TargetNode;
                    var attribute = ctx.Attributes.First();
                    var types = attribute.GetTypesFromConstructor(TypeKind.Struct);
                    return (syntax, types);
                });

        context.RegisterSourceOutput(sources, static (ctx, input) =>
        {
            var (syntax, types) = input;

            if (!syntax.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                return;
            }

            if (types.Count == 0)
            {
                return;
            }

            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);
            using var codeWriter = new IndentedTextWriter(streamWriter, "    ");

            GenerateHeader(codeWriter, syntax);
            GenerateDeclaration(codeWriter, syntax);
            GenerateTypeEnum(codeWriter, types);
            GenerateFields(codeWriter, types);
            GenerateProperties(codeWriter, types);
            GenerateConstructors(codeWriter, syntax, types);
            GenerateOperators(codeWriter, syntax, types);
            GenerateStructureClose(codeWriter, syntax);

            streamWriter.Flush();
            ctx.AddSource($"{syntax.Identifier.Text}_Union.g.cs", SourceText.From(memoryStream, Encoding.UTF8, canBeEmbedded: true));
        });
    }

    private static void GenerateHeader(IndentedTextWriter codeWriter, StructDeclarationSyntax syntax)
    {
        codeWriter.WriteLine("// <auto-generated/>");
        codeWriter.WriteLine("using System;");
        codeWriter.WriteLine("using System.Runtime.InteropServices;");
        codeWriter.WriteLine();

        if (syntax.Parent is NamespaceDeclarationSyntax namespaceDeclaration)
        {
            codeWriter.Write("namespace ");
            codeWriter.WriteLine(namespaceDeclaration.Name);
            codeWriter.WriteLine('{');
            codeWriter.Indent++;
        }
    }

    private static void GenerateDeclaration(IndentedTextWriter codeWriter, StructDeclarationSyntax syntax)
    {
        codeWriter.WriteLine("[StructLayout(LayoutKind.Explicit, Pack = 1)]");
        codeWriter.Write("public partial struct ");
        codeWriter.Write(syntax.Identifier.Text);

        codeWriter.WriteLine();
        codeWriter.WriteLine('{');
        codeWriter.Indent++;
    }

    private static void GenerateTypeEnum(IndentedTextWriter codeWriter, ImmutableList<INamedTypeSymbol> types)
    {
        codeWriter.WriteLine("[Serializable]");
        codeWriter.WriteLine("public enum Kind : byte");
        codeWriter.WriteLine('{');
        codeWriter.Indent++;
        foreach (var type in types)
        {
            codeWriter.Write(type.Name);
            codeWriter.WriteLine(",");
        }
        codeWriter.Indent--;
        codeWriter.WriteLine('}');
        codeWriter.WriteLine();
    }

    private static void GenerateFields(IndentedTextWriter codeWriter, ImmutableList<INamedTypeSymbol> types)
    {
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

    private static void GenerateProperties(IndentedTextWriter codeWriter, ImmutableList<INamedTypeSymbol> types)
    {
        codeWriter.WriteLine("public Kind Type => _type;");
        codeWriter.WriteLine();

        foreach (var type in types)
        {
            var typeName = type.Name;
            var fieldName = "_" + typeName.ToCamelCase();

            codeWriter.Write("public ");
            codeWriter.Write(type.ToDisplayString());
            codeWriter.Write(" ");
            codeWriter.WriteLine(typeName);
            codeWriter.WriteLine('{');
            codeWriter.Indent++;
            codeWriter.Write("get => Type != Kind.");
            codeWriter.Write(typeName);
            codeWriter.Write(" ? throw new Exception($\"Can't get ");
            codeWriter.Write(typeName);
            codeWriter.Write(" because current type is '{Type}'.\") : ");
            codeWriter.Write(fieldName);
            codeWriter.WriteLine(";");
            codeWriter.WriteLine("set");
            codeWriter.WriteLine('{');
            codeWriter.Indent++;
            codeWriter.Write("_type = Kind.");
            codeWriter.Write(typeName);
            codeWriter.WriteLine(";");
            codeWriter.Write(fieldName);
            codeWriter.WriteLine(" = value;");
            codeWriter.Indent--;
            codeWriter.WriteLine('}');
            codeWriter.Indent--;
            codeWriter.WriteLine('}');
            codeWriter.WriteLine();
        }
    }

    private static void GenerateConstructors(IndentedTextWriter codeWriter, StructDeclarationSyntax syntax, ImmutableList<INamedTypeSymbol> types)
    {
        foreach (var type in types)
        {
            var typeName = type.Name;
            var fieldName = "_" + typeName.ToCamelCase();

            codeWriter.Write("public ");
            codeWriter.Write(syntax.Identifier.Text);
            codeWriter.Write("(");
            codeWriter.Write(type.ToDisplayString());
            codeWriter.WriteLine(" cell)");
            codeWriter.WriteLine('{');
            codeWriter.Indent++;
            codeWriter.Write("_type = Kind.");
            codeWriter.Write(typeName);
            codeWriter.WriteLine(";");
            foreach (var otherType in types)
            {
                if (SymbolEqualityComparer.Default.Equals(otherType, type))
                {
                    continue;
                }
                codeWriter.Write("_");
                codeWriter.Write(otherType.Name.ToCamelCase());
                codeWriter.WriteLine(" = default;");
            }
            codeWriter.Write(fieldName);
            codeWriter.WriteLine(" = cell;");
            codeWriter.Indent--;
            codeWriter.WriteLine('}');
            codeWriter.WriteLine();
        }
    }

    private static void GenerateOperators(IndentedTextWriter codeWriter, StructDeclarationSyntax syntax, ImmutableList<INamedTypeSymbol> types)
    {
        for (var i = 0; i < types.Count; i++)
        {
            var type = types[i];
            var typeName = type.Name;

            codeWriter.Write("public static implicit operator ");
            codeWriter.Write(type.ToDisplayString());
            codeWriter.Write("(");
            codeWriter.Write(syntax.Identifier.Text);
            codeWriter.Write(" cell) => cell.");
            codeWriter.Write(typeName);
            codeWriter.WriteLine(";");

            codeWriter.Write("public static implicit operator ");
            codeWriter.Write(syntax.Identifier.Text);
            codeWriter.Write("(");
            codeWriter.Write(type.ToDisplayString());

            codeWriter.Write(" cell) => new ");
            codeWriter.Write(syntax.Identifier.Text);
            codeWriter.WriteLine("(cell);");

            if (i < types.Count - 1)
            {
                codeWriter.WriteLine();
            }
        }
    }

    private static void GenerateStructureClose(IndentedTextWriter codeWriter, StructDeclarationSyntax syntax)
    {
        codeWriter.Indent--;
        codeWriter.WriteLine('}');

        if (syntax.Parent is NamespaceDeclarationSyntax)
        {
            codeWriter.Indent--;
            codeWriter.WriteLine('}');
        }
    }
}