using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Appegy.Union.Generator;

public abstract class ParentScopedPart<T>(IReadOnlyList<GeneratorPart<T>> innerParts) : ScopedPart<T>(innerParts) where T : struct
{
    public override bool NeedNewLine => true;

    protected abstract TypeDeclarationSyntax GetTypeSyntax(T input);

    public override void Generate(IndentedTextWriter codeWriter, T input)
    {
        var syntax = GetTypeSyntax(input);

        if (syntax.Parent is NamespaceDeclarationSyntax namespaceDeclaration)
        {
            codeWriter.Write("namespace ");
            codeWriter.WriteLine(namespaceDeclaration.Name);
            base.Generate(codeWriter, input);
        }
    }
}