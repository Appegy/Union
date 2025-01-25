using Microsoft.CodeAnalysis;

namespace Appegy.Union.Generator;

public static class DiagnosticDescriptors
{
    private const string Category = "OneOf";

    public static DiagnosticDescriptor NotStruct { get; } = new(
        id: "UNION001",
        title: "Type has no be struct",
        messageFormat: "The type '{0}' must be a struct to use [ExclusiveOneOf].",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor NotPartial { get; } = new(
        id: "UNION002",
        title: "Struct is not partial",
        messageFormat: "The struct '{0}' must be marked as 'partial' to use [ExclusiveOneOf].",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor NestedNotPartial { get; } = new(
        id: "UNION003",
        title: "Nested struct is not partial",
        messageFormat: "The nested types must be marked as 'partial' to use [ExclusiveOneOf].",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor NoTypesProvided { get; } = new(
        id: "UNION004",
        title: "No types provided in [ExclusiveOneOf] attribute",
        messageFormat: "The [ExclusiveOneOf] attribute must specify at least one type.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor DuplicateImplementInterfaceAttribute { get; } = new(
        id: "UNION005",
        title: "Duplicate [Implements] attribute",
        messageFormat: "The interface '{0}' is already specified in [Implements].",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor ImplementInterfaceTypeMustBeInterface { get; } = new(
        id: "UNION006",
        title: "Invalid type in [Implements] attribute",
        messageFormat: "The type '{0}' must be an interface to be used in [Implements].",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor TypeDoesNotImplementInterface { get; } = new(
        id: "UNION007",
        title: "Type does not implement interface",
        messageFormat: "The type '{0}' does not implement the interface '{1}' specified in [Implements].",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}