using System.IO;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Appegy.Union.Generator.Utilities;

internal static class CodeWriterExtensions
{
    public static void AppendFullTypeName(this TextWriter codeWriter, TypeDeclarationSyntax classDeclarationSyntax)
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