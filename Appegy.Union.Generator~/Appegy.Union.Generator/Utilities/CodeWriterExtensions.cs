using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Appegy.Union.Generator;

internal static class CodeWriterExtensions
{
    public static void AppendParts<T>(this IndentedTextWriter codeWriter, IReadOnlyList<GeneratorPart<T>> parts, T input)
        where T : struct
    {
        for (var i = 0; i < parts.Count; i++)
        {
            var part = parts[i];

            part.Generate(codeWriter, input);

            if (i < parts.Count - 1 && parts[i + 1].NeedNewLine)
            {
                codeWriter.WriteLine();
            }
        }
    }

    public static void WriteFieldName(this IndentedTextWriter codeWriter, ISymbol symbol)
    {
        var name = symbol.Name;
        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        codeWriter.Write("_");
        codeWriter.Write(char.ToLower(name[0]));
        for (var i = 1; i < name.Length; i++)
        {
            codeWriter.Write(name[i]);
        }
    }

    public static void AppendFullTypeName(this IndentedTextWriter codeWriter, TypeDeclarationSyntax classDeclarationSyntax)
    {
        var ancestorCount = 0;
        var parent = classDeclarationSyntax.Parent;
        while (parent is BaseNamespaceDeclarationSyntax or BaseTypeDeclarationSyntax)
        {
            ancestorCount++;
            parent = parent.Parent;
        }
        parent = classDeclarationSyntax.Parent;

        var names = new string[ancestorCount];
        var currentAncestor = ancestorCount - 1;
        while (parent is BaseNamespaceDeclarationSyntax or BaseTypeDeclarationSyntax)
        {
            switch (parent)
            {
                case BaseTypeDeclarationSyntax parentClass:
                    names[currentAncestor] = parentClass.Identifier.Text;
                    break;
                case BaseNamespaceDeclarationSyntax parentNamespace:
                    names[currentAncestor] = parentNamespace.Name.ToString();
                    break;
            }

            currentAncestor--;
            parent = parent.Parent;
        }

        codeWriter.Write("global::");
        foreach (var name in names)
        {
            codeWriter.Write(name);
            codeWriter.Write('.');
        }
        codeWriter.Write(classDeclarationSyntax.Identifier.Text);
    }
}