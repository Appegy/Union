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
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(NotStruct, NotPartial, NoTypesProvided);

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

        if (attributeSyntax.Parent?.Parent is not StructDeclarationSyntax structDeclaration)
        {
            return;
        }

        if (!structDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            var diagnostic = Diagnostic.Create(
                NotPartial,
                structDeclaration.Identifier.GetLocation(),
                structDeclaration.Identifier.Text);

            context.ReportDiagnostic(diagnostic);
        }

        var arguments = attributeSyntax.ArgumentList?.Arguments;
        if (arguments == null || arguments.Value.Count == 0)
        {
            var diagnostic = Diagnostic.Create(
                NoTypesProvided,
                attributeSyntax.GetLocation());

            context.ReportDiagnostic(diagnostic);
            return;
        }

        foreach (var argument in arguments)
        {
            if (argument.Expression is TypeOfExpressionSyntax typeOfExpression)
            {
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
    }
}