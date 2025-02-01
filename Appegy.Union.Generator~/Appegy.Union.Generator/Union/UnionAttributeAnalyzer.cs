using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Appegy.Union.Generator.AttributesNames;
using static Appegy.Union.Generator.DiagnosticDescriptors;

namespace Appegy.Union.Generator;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UnionAttributeAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(
            NotPartial,
            NestedNotPartial,
            NoTypesProvided,
            NotStruct,
            DuplicateUnionType);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeAttribute, SyntaxKind.Attribute);
    }

    private void AnalyzeAttribute(SyntaxNodeAnalysisContext context)
    {
        var attributeSyntax = (AttributeSyntax)context.Node;
        var attributeSymbol = context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol as IMethodSymbol;
        if (attributeSymbol?.ContainingType?.ToDisplayString() != UnionAttributeName)
        {
            return;
        }

        VerifyPartialModifier(context, attributeSyntax);
        VerifyParentsPartial(context, attributeSyntax);
        VerifyArgumentsExistence(context, attributeSyntax);
        VerifyAllTypesAreStruct(context, attributeSyntax);
        VerifyNoDuplicate(context, attributeSyntax);
    }

    private static void VerifyPartialModifier(SyntaxNodeAnalysisContext context, AttributeSyntax attributeSyntax)
    {
        if (attributeSyntax.Parent?.Parent is not StructDeclarationSyntax structDeclaration)
        {
            return;
        }

        if (structDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            return;
        }

        var diagnostic = Diagnostic.Create(
            NotPartial,
            structDeclaration.Identifier.GetLocation(),
            structDeclaration.Identifier.Text);

        context.ReportDiagnostic(diagnostic);
    }

    private static void VerifyParentsPartial(SyntaxNodeAnalysisContext context, AttributeSyntax attributeSyntax)
    {
        if (attributeSyntax.Parent?.Parent is not StructDeclarationSyntax structDeclaration)
        {
            return;
        }

        var parent = structDeclaration.Parent;
        while (parent is TypeDeclarationSyntax parentType)
        {
            if (!parentType.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                var diagnostic = Diagnostic.Create(
                    NestedNotPartial,
                    parentType.Identifier.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
            parent = parentType.Parent;
        }
    }

    private void VerifyArgumentsExistence(SyntaxNodeAnalysisContext context, AttributeSyntax attributeSyntax)
    {
        var arguments = attributeSyntax.ArgumentList?.Arguments;
        if (arguments != null && arguments.Value.Count != 0)
        {
            return;
        }

        var diagnostic = Diagnostic.Create(
            NoTypesProvided,
            attributeSyntax.GetLocation());

        context.ReportDiagnostic(diagnostic);
    }

    private void VerifyAllTypesAreStruct(SyntaxNodeAnalysisContext context, AttributeSyntax attributeSyntax)
    {
        var arguments = attributeSyntax.ArgumentList?.Arguments;
        if (arguments == null || arguments.Value.Count == 0)
        {
            return;
        }

        foreach (var argument in arguments)
        {
            if (argument.Expression is not TypeOfExpressionSyntax typeOfExpression)
            {
                continue;
            }

            var typeInfo = context.SemanticModel.GetTypeInfo(typeOfExpression.Type);
            if (typeInfo.Type?.TypeKind != TypeKind.Struct)
            {
                var diagnostic = Diagnostic.Create(
                    NotStruct,
                    typeOfExpression.Type.GetLocation(),
                    typeInfo.Type?.ToDisplayString() ?? "unknown");

                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    private void VerifyNoDuplicate(SyntaxNodeAnalysisContext context, AttributeSyntax attributeSyntax)
    {
        var arguments = attributeSyntax.ArgumentList?.Arguments;
        if (arguments == null || arguments.Value.Count == 0)
        {
            return;
        }

        var uniqueTypes = new HashSet<string>();
        foreach (var argument in arguments)
        {
            if (argument.Expression is not TypeOfExpressionSyntax typeOfExpression)
            {
                continue;
            }

            var typeInfo = context.SemanticModel.GetTypeInfo(typeOfExpression.Type);
            if (typeInfo.Type?.TypeKind == TypeKind.Struct && !uniqueTypes.Add(typeInfo.Type.ToDisplayString()))
            {
                var diagnostic = Diagnostic.Create(
                    DuplicateUnionType,
                    typeOfExpression.Type.GetLocation(),
                    typeInfo.Type.ToDisplayString());

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}