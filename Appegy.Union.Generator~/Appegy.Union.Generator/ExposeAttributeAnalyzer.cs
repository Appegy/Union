using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Appegy.Union.Generator.AttributesNames;
using static Appegy.Union.Generator.DiagnosticDescriptors;

namespace Appegy.Union.Generator;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ExposeAttributeAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(
            ExposeRequiresUnion,
            ExposeTypeMustBeInterface,
            NoInterfacesProvided,
            DuplicateExposeAttribute,
            TypeDoesNotImplementInterface
        );

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeAttribute, SyntaxKind.Attribute);
    }

    private void AnalyzeAttribute(SyntaxNodeAnalysisContext context)
    {
        var attributeSyntax = (AttributeSyntax)context.Node;

        // Check if this is ExposeAttribute
        var attributeSymbol = context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol as IMethodSymbol;
        if (attributeSymbol?.ContainingType?.ToDisplayString() != ExposeAttributeName)
        {
            return;
        }

        VerifyUnionAttribute(context, attributeSyntax);
        VerifyArgumentsExistence(context, attributeSyntax);
        VerifyAllTypesAreInterfaces(context, attributeSyntax);
        VerifyNoDuplicate(context, attributeSyntax);
        VerifyUnionTypesImplementExposedInterfaces(context, attributeSyntax);
    }

    private void VerifyUnionAttribute(SyntaxNodeAnalysisContext context, AttributeSyntax attributeSyntax)
    {
        // Ensure that ExposeAttribute is used together with UnionAttribute
        if (attributeSyntax.Parent?.Parent is not TypeDeclarationSyntax structDeclaration)
        {
            return;
        }

        var hasUnionAttribute = false;

        // Iterate through all attribute lists in the struct
        foreach (var attributeList in structDeclaration.AttributeLists)
        {
            // Iterate through all attributes in the current attribute list
            foreach (var attribute in attributeList.Attributes)
            {
                // Get the symbol of the attribute
                var otherAttributeSyntax = context.SemanticModel.GetSymbolInfo(attribute).Symbol as IMethodSymbol;
                if (otherAttributeSyntax?.ContainingType?.ToDisplayString() == UnionAttributeName)
                {
                    hasUnionAttribute = true;
                    break;
                }
            }

            if (hasUnionAttribute)
            {
                break;
            }
        }

        if (!hasUnionAttribute)
        {
            // ExposeAttribute is meaningless without UnionAttribute
            var diagnostic = Diagnostic.Create(
                ExposeRequiresUnion,
                attributeSyntax.GetLocation());

            context.ReportDiagnostic(diagnostic);
        }
    }

    private void VerifyArgumentsExistence(SyntaxNodeAnalysisContext context, AttributeSyntax attributeSyntax)
    {
        var arguments = attributeSyntax.ArgumentList?.Arguments;
        if (arguments == null || arguments.Value.Count == 0)
        {
            // No interfaces provided
            var diagnostic = Diagnostic.Create(
                NoInterfacesProvided,
                attributeSyntax.GetLocation());

            context.ReportDiagnostic(diagnostic);
        }
    }

    private void VerifyAllTypesAreInterfaces(SyntaxNodeAnalysisContext context, AttributeSyntax attributeSyntax)
    {
        var arguments = attributeSyntax.ArgumentList?.Arguments;
        if (arguments == null || arguments.Value.Count == 0)
        {
            return;
        }

        // Check for duplicate interfaces
        foreach (var argument in arguments)
        {
            if (argument.Expression is not TypeOfExpressionSyntax typeOfExpression)
            {
                continue;
            }

            var typeInfo = context.SemanticModel.GetTypeInfo(typeOfExpression.Type);
            if (typeInfo.Type?.TypeKind != TypeKind.Interface)
            {
                // Type is not an interface
                var diagnostic = Diagnostic.Create(
                    ExposeTypeMustBeInterface,
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

        // Check for duplicate interfaces
        var uniqueInterfaces = new HashSet<string>();
        foreach (var argument in arguments)
        {
            if (argument.Expression is not TypeOfExpressionSyntax typeOfExpression)
            {
                continue;
            }

            var typeInfo = context.SemanticModel.GetTypeInfo(typeOfExpression.Type);
            if (typeInfo.Type?.TypeKind == TypeKind.Interface && !uniqueInterfaces.Add(typeInfo.Type.ToDisplayString()))
            {
                // Duplicate interface
                var diagnostic = Diagnostic.Create(
                    DuplicateExposeAttribute,
                    typeOfExpression.Type.GetLocation(),
                    typeInfo.Type.ToDisplayString());

                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    private void VerifyUnionTypesImplementExposedInterfaces(SyntaxNodeAnalysisContext context, AttributeSyntax exposeAttribute)
    {
        if (exposeAttribute.Parent?.Parent is not TypeDeclarationSyntax structDeclaration)
        {
            return;
        }

        var interfaces = GetExposedInterfaces(context, exposeAttribute);
        if (interfaces.Count == 0)
        {
            return;
        }

        var unionTypes = GetUnionTypes(context, structDeclaration);
        if (unionTypes.Length == 0)
        {
            return;
        }

        foreach (var (type, location) in unionTypes)
        {
            VerifyTypeImplementsAllInterfaces(context, type, location, interfaces);
        }
    }

    private ImmutableHashSet<INamedTypeSymbol> GetExposedInterfaces(SyntaxNodeAnalysisContext context, AttributeSyntax exposeAttribute)
    {
        var interfaces = ImmutableHashSet.CreateBuilder<INamedTypeSymbol>(SymbolEqualityComparer.Default);

        if (exposeAttribute.ArgumentList?.Arguments is not { } arguments)
        {
            return interfaces.ToImmutable();
        }

        foreach (var argument in arguments)
        {
            if (argument.Expression is not TypeOfExpressionSyntax typeOfExpression)
            {
                continue;
            }

            var typeInfo = context.SemanticModel.GetTypeInfo(typeOfExpression.Type);
            if (typeInfo.Type is INamedTypeSymbol { TypeKind: TypeKind.Interface } interfaceSymbol)
            {
                interfaces.Add(interfaceSymbol);
            }
        }

        return interfaces.ToImmutable();
    }

    private ImmutableArray<(INamedTypeSymbol TypeSymbol, Location Location)> GetUnionTypes(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDeclaration)
    {
        var unionTypes = ImmutableArray.CreateBuilder<(INamedTypeSymbol, Location)>();

        foreach (var attribute in typeDeclaration.AttributeLists.SelectMany(c => c.Attributes))
        {
            var attributeSymbol = context.SemanticModel.GetSymbolInfo(attribute).Symbol as IMethodSymbol;
            if (attributeSymbol?.ContainingType?.ToDisplayString() != UnionAttributeName)
            {
                continue;
            }

            if (attribute.ArgumentList?.Arguments is not { } arguments)
            {
                continue;
            }

            foreach (var argument in arguments)
            {
                if (argument.Expression is not TypeOfExpressionSyntax typeOfExpression)
                {
                    continue;
                }

                var typeInfo = context.SemanticModel.GetTypeInfo(typeOfExpression.Type);
                if (typeInfo.Type is INamedTypeSymbol typeSymbol)
                {
                    unionTypes.Add((typeSymbol, typeOfExpression.Type.GetLocation()));
                }
            }
        }

        return unionTypes.ToImmutable();
    }

    private void VerifyTypeImplementsAllInterfaces(SyntaxNodeAnalysisContext context, INamedTypeSymbol type, Location location, ImmutableHashSet<INamedTypeSymbol> interfaces)
    {
        foreach (var interfaceSymbol in interfaces)
        {
            if (!TypeImplementsInterface(type, interfaceSymbol))
            {
                ReportInterfaceImplementationError(context, type, location, interfaceSymbol);
            }
        }
    }

    private bool TypeImplementsInterface(INamedTypeSymbol typeSymbol, INamedTypeSymbol interfaceSymbol)
    {
        foreach (var implementedInterface in typeSymbol.AllInterfaces)
        {
            if (SymbolEqualityComparer.Default.Equals(implementedInterface, interfaceSymbol))
            {
                return true;
            }
        }
        return false;
    }

    private void ReportInterfaceImplementationError(SyntaxNodeAnalysisContext context, INamedTypeSymbol type, Location location, INamedTypeSymbol @interface)
    {
        var diagnostic = Diagnostic.Create(
            TypeDoesNotImplementInterface,
            location,
            type.ToDisplayString(),
            @interface.ToDisplayString());

        context.ReportDiagnostic(diagnostic);
    }
}