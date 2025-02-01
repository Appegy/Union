using System.Buffers;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Appegy.Union.Generator;

public abstract class ParentScopedPart<T>(IReadOnlyList<GeneratorPart<T>> innerParts) : GeneratorPart<T> where T : struct
{
    public IReadOnlyList<GeneratorPart<T>> InnerParts { get; } = innerParts;
    public override bool NeedNewLine => true;

    protected abstract TypeDeclarationSyntax GetTypeSyntax(T input);

    public override void Generate(IndentedTextWriter codeWriter, T input)
    {
        var syntax = GetTypeSyntax(input);

        var ancestorCount = 0;
        var parent = syntax.Parent;
        while (parent is NamespaceDeclarationSyntax or TypeDeclarationSyntax)
        {
            ancestorCount++;
            parent = parent.Parent;
        }

        var nodes = ArrayPool<SyntaxNode>.Shared.Rent(ancestorCount);
        parent = syntax.Parent;
        for (var i = ancestorCount - 1; i >= 0; i--)
        {
            nodes[i] = parent!;
            parent = parent!.Parent;
        }

        for (var i = 0; i < ancestorCount; i++)
        {
            switch (nodes[i])
            {
                case NamespaceDeclarationSyntax ns:
                    codeWriter.Write("namespace ");
                    codeWriter.WriteLine(ns.Name.ToString());
                    codeWriter.WriteLine("{");
                    codeWriter.Indent++;
                    break;
                case TypeDeclarationSyntax td:
                {
                    if (td.Modifiers.Any(SyntaxKind.StaticKeyword))
                    {
                        codeWriter.Write("static ");
                    }
                    if (td.Modifiers.Any(SyntaxKind.PartialKeyword))
                    {
                        codeWriter.Write("partial ");
                    }
                    codeWriter.Write(td.Keyword.Text + " ");
                    codeWriter.WriteLine(td.Identifier.Text);
                    codeWriter.WriteLine("{");
                    codeWriter.Indent++;
                    break;
                }
            }
        }

        ArrayPool<SyntaxNode>.Shared.Return(nodes);
        codeWriter.AppendParts(InnerParts, input);

        for (var i = 0; i < ancestorCount; i++)
        {
            codeWriter.Indent--;
            codeWriter.WriteLine("}");
        }
    }
}