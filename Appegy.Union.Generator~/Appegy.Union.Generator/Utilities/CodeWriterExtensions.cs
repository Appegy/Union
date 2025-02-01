using System.CodeDom.Compiler;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Appegy.Union.Generator;

internal static class CodeWriterExtensions
{
    public static void AppendParts<T>(this IndentedTextWriter codeWriter, ImmutableArray<GeneratorPart<T>> parts, T input)
        where T : struct
    {
        for (var i = 0; i < parts.Length; i++)
        {
            var part = parts[i];
            if (part.InRegion)
            {
                codeWriter.Write("#region ");
                codeWriter.WriteLine(part.Description);
                codeWriter.WriteLine();
            }

            part.Generate(codeWriter, input);

            if (part.InRegion)
            {
                codeWriter.WriteLine();
                codeWriter.Write("#endregion ");
                codeWriter.WriteLine(part.Description);
            }

            if (i < parts.Length - 1 && part.NewLineAtEnd)
            {
                codeWriter.WriteLine();
            }
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