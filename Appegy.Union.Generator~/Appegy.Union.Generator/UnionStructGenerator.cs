using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Appegy.Union.Generator.DiagnosticDescriptors;

namespace Appegy.Union.Generator;

[Generator]
public class UnionStructGenerator : IIncrementalGenerator
{
    private const string UnionAttributeName = "Appegy.Union.UnionAttribute";
    private const string ExposeAttributeName = "Appegy.Union.ExposeAttribute";

    private readonly struct StructDeclarationContext(
        StructDeclarationSyntax syntax,
        IReadOnlyList<INamedTypeSymbol> types,
        IReadOnlyList<INamedTypeSymbol> interfaces)
    {
        public readonly StructDeclarationSyntax Syntax = syntax;
        public readonly IReadOnlyList<INamedTypeSymbol> Types = types;
        public readonly IReadOnlyList<INamedTypeSymbol> Interfaces = interfaces;
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider
            .CreateSyntaxProvider((s, _) => s is StructDeclarationSyntax, (ctx, _) => GetStructDeclarationContext(ctx))
            .Where(t => t.AttributeFound)
            .Select((t, _) => t.StructContext);

        context.RegisterSourceOutput(provider, GenerateUnionStruct);
    }

    private void GenerateUnionStruct(SourceProductionContext sourceContext, StructDeclarationContext structContext)
    {
        var syntax = structContext.Syntax;
        var types = structContext.Types;
        var interfaces = structContext.Interfaces;

        var isPartial = syntax.Modifiers.Any(SyntaxKind.PartialKeyword);
        if (!isPartial)
        {
            sourceContext.ReportDiagnostic(Diagnostic.Create(NotPartial, syntax.GetLocation(), syntax.Identifier.Text));
        }

        if (types.Count == 0)
        {
            sourceContext.ReportDiagnostic(Diagnostic.Create(NoTypesProvided, syntax.GetLocation()));
            return;
        }

        foreach (var type in types)
        {
            foreach (var interfaceType in interfaces)
            {
                if (!type.AllInterfaces.Contains(interfaceType))
                {
                    sourceContext.ReportDiagnostic(Diagnostic.Create(
                        TypeDoesNotImplementInterface,
                        syntax.GetLocation(),
                        type.ToDisplayString(),
                        interfaceType.ToDisplayString()));
                }
            }
        }

        using var memoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);
        using var codeWriter = new IndentedTextWriter(streamWriter, "    ");

        GenerateHeader(syntax, codeWriter);
        GenerateDeclaration(syntax, codeWriter, interfaces);
        GenerateTypeEnum(codeWriter, types);
        GenerateFields(codeWriter, types);
        GenerateProperties(codeWriter, types);
        GenerateConstructors(syntax, codeWriter, types);
        GenerateImplementations(interfaces, codeWriter, types);
        GenerateOperators(types, codeWriter, syntax);
        GenerateStructureClose(syntax, codeWriter);

        streamWriter.Flush();
        sourceContext.AddSource($"{syntax.Identifier.Text}.g.cs", SourceText.From(memoryStream, Encoding.UTF8, canBeEmbedded: true));
    }

    private static (bool AttributeFound, StructDeclarationContext StructContext) GetStructDeclarationContext(GeneratorSyntaxContext context)
    {
        var structDeclarationSyntax = (StructDeclarationSyntax)context.Node;
        var (attributeFound, types) = GetTypesFromAttribute(context, structDeclarationSyntax, UnionAttributeName);

        if (!attributeFound)
        {
            return (false, default);
        }

        var (_, interfaces) = GetTypesFromAttribute(context, structDeclarationSyntax, ExposeAttributeName);

        return (true, new StructDeclarationContext(structDeclarationSyntax, types, interfaces));
    }

    private static (bool AttributeFound, IReadOnlyList<INamedTypeSymbol> Types) GetTypesFromAttribute(
        GeneratorSyntaxContext context,
        StructDeclarationSyntax structDeclarationSyntax,
        string attributeName)
    {
        foreach (var attributeSyntax in structDeclarationSyntax.AttributeLists.SelectMany(attributeListSyntax => attributeListSyntax.Attributes))
        {
            if (ModelExtensions.GetSymbolInfo(context.SemanticModel, attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
            {
                continue;
            }

            var currentAttributeName = attributeSymbol.ContainingType.ToDisplayString();

            if (currentAttributeName != attributeName)
            {
                continue;
            }

            var types = new List<INamedTypeSymbol>();

            if (attributeSyntax.ArgumentList == null || attributeSyntax.ArgumentList.Arguments.Count == 0)
            {
                continue;
            }

            foreach (var argument in attributeSyntax.ArgumentList.Arguments)
            {
                if (argument.Expression is not TypeOfExpressionSyntax typeOfExpression)
                {
                    continue;
                }

                if (ModelExtensions.GetTypeInfo(context.SemanticModel, typeOfExpression.Type).Type is INamedTypeSymbol typeSymbol)
                {
                    types.Add(typeSymbol);
                }
            }

            return (true, types);
        }

        return (false, Array.Empty<INamedTypeSymbol>());
    }

    private static void GenerateHeader(StructDeclarationSyntax syntax, IndentedTextWriter codeWriter)
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

    private static void GenerateDeclaration(StructDeclarationSyntax syntax, IndentedTextWriter codeWriter, IReadOnlyList<INamedTypeSymbol> interfaces)
    {
        codeWriter.WriteLine("[StructLayout(LayoutKind.Explicit, Pack = 1)]");
        codeWriter.Write("public partial struct ");
        codeWriter.Write(syntax.Identifier.Text);

        if (interfaces is { Count: > 0 })
        {
            codeWriter.WriteLine(" :");
            codeWriter.Indent++;
            for (var i = 0; i < interfaces.Count; i++)
            {
                var @interface = interfaces[i];
                codeWriter.Write("global::");
                codeWriter.Write(@interface.ToDisplayString());
                if (i != interfaces.Count - 1)
                {
                    codeWriter.WriteLine(",");
                }
            }
            codeWriter.Indent--;
        }

        codeWriter.WriteLine();
        codeWriter.WriteLine('{');
        codeWriter.Indent++;
    }

    private static void GenerateTypeEnum(IndentedTextWriter codeWriter, IReadOnlyList<INamedTypeSymbol> types)
    {
        codeWriter.WriteLine("[Serializable]");
        codeWriter.WriteLine("public enum Kind : byte");
        codeWriter.WriteLine('{');
        codeWriter.Indent++;
        foreach (var type in types)
        {
            codeWriter.WriteLine(type.Name + ",");
        }
        codeWriter.Indent--;
        codeWriter.WriteLine('}');
        codeWriter.WriteLine();
    }

    private static void GenerateFields(IndentedTextWriter codeWriter, IReadOnlyList<INamedTypeSymbol> types)
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

    private static void GenerateProperties(IndentedTextWriter codeWriter, IReadOnlyList<INamedTypeSymbol> types)
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
            codeWriter.WriteLine(" because current type is '{Type}'.\") : " + fieldName + ";");
            codeWriter.WriteLine("set");
            codeWriter.WriteLine('{');
            codeWriter.Indent++;
            codeWriter.Write("_type = Kind.");
            codeWriter.WriteLine(typeName + ";");
            codeWriter.WriteLine(fieldName + " = value;");
            codeWriter.Indent--;
            codeWriter.WriteLine('}');
            codeWriter.Indent--;
            codeWriter.WriteLine('}');
            codeWriter.WriteLine();
        }
    }

    private static void GenerateConstructors(StructDeclarationSyntax syntax, IndentedTextWriter codeWriter, IReadOnlyList<INamedTypeSymbol> types)
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
            codeWriter.WriteLine(typeName + ";");
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
            codeWriter.WriteLine(fieldName + " = cell;");
            codeWriter.Indent--;
            codeWriter.WriteLine('}');
            codeWriter.WriteLine();
        }
    }

    private static void GenerateImplementations(IReadOnlyList<INamedTypeSymbol> interfaces, IndentedTextWriter codeWriter, IReadOnlyList<INamedTypeSymbol> types)
    {
        if (interfaces is not { Count: > 0 })
        {
            return;
        }

        foreach (var interfaceType in interfaces)
        {
            foreach (var member in interfaceType.GetMembers())
            {
                switch (member)
                {
                    case IPropertySymbol { IsIndexer: true } propertySymbol:
                        GenerateIndexerImplementation(propertySymbol, codeWriter, types);
                        break;

                    case IPropertySymbol propertySymbol:
                        GeneratePropertyImplementation(propertySymbol, codeWriter, types);
                        break;

                    case IMethodSymbol { MethodKind: MethodKind.Ordinary } methodSymbol:
                        GenerateMethodImplementation(methodSymbol, codeWriter, types);
                        break;

                    case IEventSymbol eventSymbol:
                        GenerateEventImplementation(eventSymbol, codeWriter, types);
                        break;

                    case IFieldSymbol fieldSymbol:
                        GenerateFieldImplementation(fieldSymbol, codeWriter, types);
                        break;
                }
            }
        }
    }

    private static void GeneratePropertyImplementation(
        IPropertySymbol propertySymbol,
        IndentedTextWriter codeWriter,
        IReadOnlyList<INamedTypeSymbol> types)
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

        codeWriter.WriteLine("_ => throw new ArgumentOutOfRangeException(nameof(Type), $\"Unknown cell type: {_type}\")");
        codeWriter.Indent--;
        codeWriter.WriteLine("};");
        codeWriter.WriteLine();
    }

    private static void GenerateMethodImplementation(
        IMethodSymbol methodSymbol,
        IndentedTextWriter codeWriter,
        IReadOnlyList<INamedTypeSymbol> types)
    {
        codeWriter.Write("public ");
        codeWriter.Write(methodSymbol.ReturnType.ToDisplayString());
        codeWriter.Write(" ");
        codeWriter.Write(methodSymbol.Name);
        codeWriter.Write("(");

        for (int i = 0; i < methodSymbol.Parameters.Length; i++)
        {
            var parameter = methodSymbol.Parameters[i];
            codeWriter.Write(parameter.Type.ToDisplayString());
            codeWriter.Write(" ");
            codeWriter.Write(parameter.Name);
            if (i < methodSymbol.Parameters.Length - 1)
            {
                codeWriter.Write(", ");
            }
        }

        codeWriter.WriteLine(") => _type switch");
        codeWriter.WriteLine('{');
        codeWriter.Indent++;

        foreach (var type in types)
        {
            codeWriter.Write("Kind.");
            codeWriter.Write(type.Name);
            codeWriter.Write(" => _");
            codeWriter.Write(type.Name.ToCamelCase());
            codeWriter.Write(".");
            codeWriter.Write(methodSymbol.Name);
            codeWriter.Write("(");

            for (int i = 0; i < methodSymbol.Parameters.Length; i++)
            {
                codeWriter.Write(methodSymbol.Parameters[i].Name);
                if (i < methodSymbol.Parameters.Length - 1)
                {
                    codeWriter.Write(", ");
                }
            }

            codeWriter.WriteLine("),");
        }

        codeWriter.WriteLine("_ => throw new ArgumentOutOfRangeException(nameof(Type), $\"Unknown cell type: {_type}\")");
        codeWriter.Indent--;
        codeWriter.WriteLine("};");
        codeWriter.WriteLine();
    }

    private static void GenerateEventImplementation(
        IEventSymbol eventSymbol,
        IndentedTextWriter codeWriter,
        IReadOnlyList<INamedTypeSymbol> types)
    {
        codeWriter.Write("public event ");
        codeWriter.Write(eventSymbol.Type.ToDisplayString());
        codeWriter.Write(" ");
        codeWriter.Write(eventSymbol.Name);
        codeWriter.WriteLine(" { add => _type switch {");
        codeWriter.Indent++;

        foreach (var type in types)
        {
            codeWriter.Write("Kind.");
            codeWriter.Write(type.Name);
            codeWriter.Write(" => _");
            codeWriter.Write(type.Name.ToCamelCase());
            codeWriter.Write(".");
            codeWriter.Write(eventSymbol.Name);
            codeWriter.WriteLine(" += value,");
        }

        codeWriter.WriteLine("_ => throw new ArgumentOutOfRangeException(nameof(Type), $\"Unknown cell type: {_type}\")");
        codeWriter.Indent--;
        codeWriter.WriteLine("}; remove => _type switch {");
        codeWriter.Indent++;

        foreach (var type in types)
        {
            codeWriter.Write("Kind.");
            codeWriter.Write(type.Name);
            codeWriter.Write(" => _");
            codeWriter.Write(type.Name.ToCamelCase());
            codeWriter.Write(".");
            codeWriter.Write(eventSymbol.Name);
            codeWriter.WriteLine(" -= value,");
        }

        codeWriter.WriteLine("_ => throw new ArgumentOutOfRangeException(nameof(Type), $\"Unknown cell type: {_type}\")");
        codeWriter.Indent--;
        codeWriter.WriteLine("}; }");
        codeWriter.WriteLine();
    }

    private static void GenerateFieldImplementation(
        IFieldSymbol fieldSymbol,
        IndentedTextWriter codeWriter,
        IReadOnlyList<INamedTypeSymbol> types)
    {
        codeWriter.Write("public ");
        codeWriter.Write(fieldSymbol.Type.ToDisplayString());
        codeWriter.Write(" ");
        codeWriter.Write(fieldSymbol.Name);
        codeWriter.WriteLine(" => _type switch {");
        codeWriter.Indent++;

        foreach (var type in types)
        {
            codeWriter.Write("Kind.");
            codeWriter.Write(type.Name);
            codeWriter.Write(" => _");
            codeWriter.Write(type.Name.ToCamelCase());
            codeWriter.Write(".");
            codeWriter.Write(fieldSymbol.Name);
            codeWriter.WriteLine(",");
        }

        codeWriter.WriteLine("_ => throw new ArgumentOutOfRangeException(nameof(Type), $\"Unknown cell type: {_type}\")");
        codeWriter.Indent--;
        codeWriter.WriteLine("};");
        codeWriter.WriteLine();
    }

    private static void GenerateIndexerImplementation(
        IPropertySymbol indexerSymbol,
        IndentedTextWriter codeWriter,
        IReadOnlyList<INamedTypeSymbol> types)
    {
        codeWriter.Write("public ");
        codeWriter.Write(indexerSymbol.Type.ToDisplayString());
        codeWriter.Write(" this[");
        for (int i = 0; i < indexerSymbol.Parameters.Length; i++)
        {
            var parameter = indexerSymbol.Parameters[i];
            codeWriter.Write(parameter.Type.ToDisplayString());
            codeWriter.Write(" ");
            codeWriter.Write(parameter.Name);
            if (i < indexerSymbol.Parameters.Length - 1)
            {
                codeWriter.Write(", ");
            }
        }
        codeWriter.WriteLine("] => _type switch {");
        codeWriter.Indent++;

        foreach (var type in types)
        {
            codeWriter.Write("Kind.");
            codeWriter.Write(type.Name);
            codeWriter.Write(" => _");
            codeWriter.Write(type.Name.ToCamelCase());
            codeWriter.Write("[");
            for (int i = 0; i < indexerSymbol.Parameters.Length; i++)
            {
                codeWriter.Write(indexerSymbol.Parameters[i].Name);
                if (i < indexerSymbol.Parameters.Length - 1)
                {
                    codeWriter.Write(", ");
                }
            }
            codeWriter.WriteLine("],");
        }

        codeWriter.WriteLine("_ => throw new ArgumentOutOfRangeException(nameof(Type), $\"Unknown cell type: {_type}\")");
        codeWriter.Indent--;
        codeWriter.WriteLine("};");
        codeWriter.WriteLine();
    }

    private static void GenerateOperators(IReadOnlyList<INamedTypeSymbol> types, IndentedTextWriter codeWriter, StructDeclarationSyntax syntax)
    {
        for (var i = 0; i < types.Count; i++)
        {
            var type = types[i];
            var typeName = type.Name;

            codeWriter.Write("public static implicit operator ");
            codeWriter.Write(type.ToDisplayString());
            codeWriter.Write("(");
            codeWriter.Write(syntax.Identifier.Text);
            codeWriter.WriteLine(" cell) => cell." + typeName + ";");

            codeWriter.Write("public static implicit operator ");
            codeWriter.Write(syntax.Identifier.Text);
            codeWriter.Write("(");
            codeWriter.Write(type.ToDisplayString());
            codeWriter.WriteLine(" cell) => new " + syntax.Identifier.Text + "(cell);");

            if (i < types.Count - 1)
            {
                codeWriter.WriteLine();
            }
        }
    }

    private static void GenerateStructureClose(StructDeclarationSyntax syntax, IndentedTextWriter codeWriter)
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