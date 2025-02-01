using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Appegy.Union.Generator;

public class ExposeInterfacePart(IReadOnlyList<ExposeInterfacePart.Implementation> implementers) : GeneratorPart<ExposeAttributePartInput>
{
    public abstract class Implementation
    {
        public abstract bool TryGenerateMember(IndentedTextWriter codeWriter, ISymbol member, IReadOnlyList<INamedTypeSymbol> types);
    }

    public IReadOnlyList<Implementation> Implementers { get; } = implementers;

    public override void Generate(IndentedTextWriter codeWriter, ExposeAttributePartInput input)
    {
        var (syntax, types, interfaces) = input;

        for (var index = 0; index < interfaces.Count; index++)
        {
            var @interface = interfaces[index];
            codeWriter.Write("#region Implement ");
            codeWriter.WriteLine(@interface.Name);
            codeWriter.WriteLine();

            ImplementInterface(codeWriter, types, @interface.GetMembers());

            codeWriter.Write("#endregion Implement ");
            codeWriter.WriteLine(@interface.Name);

            if (index < interfaces.Count - 1)
            {
                codeWriter.WriteLine();
            }
        }
    }

    private void ImplementInterface(IndentedTextWriter codeWriter, IReadOnlyList<INamedTypeSymbol> types, IReadOnlyList<ISymbol> members)
    {
        var needNewLine = false;
        foreach (var member in members)
        {
            if (needNewLine)
            {
                codeWriter.WriteLine();
                needNewLine = false;
            }

            if (Implementers.Any(implementation => implementation.TryGenerateMember(codeWriter, member, types)))
            {
                needNewLine = true;
            }
        }
    }
}