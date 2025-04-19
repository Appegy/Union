using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Appegy.Union.Generator;

public class ImplementMethods : ExposeInterfacePart.Implementation
{
    public override bool TryGenerateMember(IndentedTextWriter codeWriter, ISymbol member, IReadOnlyList<ExposeTypeInfo> types)
    {
        switch (member)
        {
            case IMethodSymbol { AssociatedSymbol: null } methodSymbol:
                GenerateMethod(codeWriter, methodSymbol, types);
                return true;
            default:
                return false;
        }
    }

    private static void GenerateMethod(IndentedTextWriter codeWriter, IMethodSymbol methodSymbol, IReadOnlyList<ExposeTypeInfo> types)
    {
        GenerateMethodHeader(codeWriter, methodSymbol);

        codeWriter.WriteLine("{");
        codeWriter.Indent++;

        GenerateMethodBody(codeWriter, methodSymbol, types);

        codeWriter.Indent--;
        codeWriter.WriteLine("}");
    }

    private static void GenerateMethodHeader(IndentedTextWriter codeWriter, IMethodSymbol methodSymbol)
    {
        codeWriter.Write("public ");
        codeWriter.Write(methodSymbol.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        codeWriter.Write(" ");
        codeWriter.Write(methodSymbol.Name);

        if (methodSymbol.TypeParameters.Length > 0)
        {
            codeWriter.Write("<");
            for (var i = 0; i < methodSymbol.TypeParameters.Length; i++)
            {
                if (i > 0) codeWriter.Write(", ");
                codeWriter.Write(methodSymbol.TypeParameters[i].Name);
            }
            codeWriter.Write(">");
        }

        codeWriter.Write("(");
        for (var i = 0; i < methodSymbol.Parameters.Length; i++)
        {
            if (i > 0) codeWriter.Write(", ");

            var param = methodSymbol.Parameters[i];
            if (param.RefKind != RefKind.None)
            {
                switch (param.RefKind)
                {
                    case RefKind.Ref:
                        codeWriter.Write("ref ");
                        break;
                    case RefKind.Out:
                        codeWriter.Write("out ");
                        break;
                    case RefKind.In:
                        codeWriter.Write("in ");
                        break;
                }
            }

            codeWriter.Write(param.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
            codeWriter.Write(" ");
            codeWriter.Write(param.Name);
        }
        codeWriter.Write(")");

        codeWriter.Indent++;
        foreach (var typeParameter in methodSymbol.TypeParameters)
        {
            var hasConstraints = false;

            void addConstraint(string constraint)
            {
                if (hasConstraints) codeWriter.Write(", ");
                codeWriter.Write(constraint);
                hasConstraints = true;
            }

            if (typeParameter.ConstraintTypes.Length > 0 ||
                typeParameter.HasConstructorConstraint ||
                typeParameter.HasValueTypeConstraint ||
                typeParameter.HasUnmanagedTypeConstraint ||
                typeParameter.HasNotNullConstraint)
            {
                codeWriter.WriteLine();
                codeWriter.Write(" where ");
                codeWriter.Write(typeParameter.Name);
                codeWriter.Write(" : ");
            }
            else
            {
                continue;
            }

            if (typeParameter.HasUnmanagedTypeConstraint)
            {
                addConstraint("unmanaged");
            }
            else if (typeParameter.HasValueTypeConstraint)
            {
                addConstraint("struct");
            }
            if (typeParameter.HasNotNullConstraint)
            {
                addConstraint("notnull");
            }
            foreach (var constraint in typeParameter.ConstraintTypes)
            {
                addConstraint(constraint.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
            }
            if (typeParameter.HasConstructorConstraint)
            {
                addConstraint("new()");
            }
        }
        codeWriter.Indent--;

        codeWriter.WriteLine();
    }

    private static void GenerateMethodBody(IndentedTextWriter codeWriter, IMethodSymbol methodSymbol, IReadOnlyList<ExposeTypeInfo> types)
    {
        codeWriter.WriteLine("switch (_type)");
        codeWriter.WriteLine("{");
        codeWriter.Indent++;

        foreach (var type in types)
        {
            codeWriter.Write("case Kind.");
            codeWriter.Write(type.Name);
            codeWriter.Write(": ");

            if (!methodSymbol.ReturnsVoid)
            {
                codeWriter.Write("return ");
            }

            codeWriter.Write(type.FieldName);
            codeWriter.Write(".");
            codeWriter.Write(methodSymbol.Name);

            if (methodSymbol.TypeParameters.Length > 0)
            {
                codeWriter.Write("<");
                for (var i = 0; i < methodSymbol.TypeParameters.Length; i++)
                {
                    if (i > 0) codeWriter.Write(", ");
                    codeWriter.Write(methodSymbol.TypeParameters[i].Name);
                }
                codeWriter.Write(">");
            }

            codeWriter.Write("(");
            GenerateMethodArguments(codeWriter, methodSymbol);
            codeWriter.WriteLine(");");
        }

        codeWriter.WriteLine("default: throw new global::System.InvalidOperationException($\"Unknown type of union: {_type}\");");

        codeWriter.Indent--;
        codeWriter.WriteLine("}");
    }

    private static void GenerateMethodArguments(IndentedTextWriter codeWriter, IMethodSymbol methodSymbol)
    {
        for (var i = 0; i < methodSymbol.Parameters.Length; i++)
        {
            if (i > 0) codeWriter.Write(", ");

            var param = methodSymbol.Parameters[i];

            if (param.RefKind != RefKind.None)
            {
                codeWriter.Write(param.RefKind.ToString().ToLower());
                codeWriter.Write(" ");
            }

            codeWriter.Write(param.Name);
        }
    }
}