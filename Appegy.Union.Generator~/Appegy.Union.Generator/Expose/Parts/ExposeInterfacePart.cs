using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace Appegy.Union.Generator;

public class ExposeInterfacePart(IReadOnlyList<GeneratorPart<ExposeInterfacePartInput>> innerParts) : GeneratorPart<ExposeAttributePartInput>
{
    public IReadOnlyList<GeneratorPart<ExposeInterfacePartInput>> InnerParts { get; } = innerParts;

    public override void Generate(IndentedTextWriter codeWriter, ExposeAttributePartInput input)
    {
        var (syntax, types, interfaces) = input;

        for (var index = 0; index < interfaces.Count; index++)
        {
            var @interface = interfaces[index];
            codeWriter.Write("#region Implement ");
            codeWriter.WriteLine(@interface.Name);
            codeWriter.WriteLine();

            var members = @interface.GetMembers();
            codeWriter.AppendParts(InnerParts, new ExposeInterfacePartInput(syntax, types, members));

            codeWriter.Write("#endregion Implement ");
            codeWriter.WriteLine(@interface.Name);

            if (index < interfaces.Count - 1)
            {
                codeWriter.WriteLine();
            }
        }
    }
}